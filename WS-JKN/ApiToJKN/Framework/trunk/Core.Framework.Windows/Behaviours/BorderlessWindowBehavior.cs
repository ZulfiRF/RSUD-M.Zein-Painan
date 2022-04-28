namespace Core.Framework.Windows.Behaviours
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Native;

    public class BorderlessWindowBehavior : Behavior<Window>
    {
        #region Static Fields

        public static DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register(
            "AutoSizeToContent",
            typeof(bool),
            typeof(BorderlessWindowBehavior),
            new PropertyMetadata(false));

        public static DependencyProperty ResizeWithGripProperty = DependencyProperty.Register(
            "ResizeWithGrip",
            typeof(bool),
            typeof(BorderlessWindowBehavior),
            new PropertyMetadata(true));

        #endregion

        #region Fields

        private readonly SolidColorBrush _borderColour =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));

        private IntPtr _mHWND;

        private HwndSource _mHWNDSource;

        #endregion

        #region Public Properties

        public bool AutoSizeToContent
        {
            get
            {
                return (bool)this.GetValue(AutoSizeToContentProperty);
            }
            set
            {
                this.SetValue(AutoSizeToContentProperty, value);
            }
        }

        public Border Border { get; set; }

        public bool ResizeWithGrip
        {
            get
            {
                return (bool)this.GetValue(ResizeWithGripProperty);
            }
            set
            {
                this.SetValue(ResizeWithGripProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(this.AssociatedObject) != null)
            {
                this.AddHwndHook();
            }
            else
            {
                this.AssociatedObject.SourceInitialized += this.AssociatedObject_SourceInitialized;
            }

            this.AssociatedObject.WindowStyle = WindowStyle.None;
            this.AssociatedObject.StateChanged += this.AssociatedObjectStateChanged;

            if (this.AssociatedObject is MetroWindow)
            {
                var window = ((MetroWindow)this.AssociatedObject);
                //MetroWindow already has a border we can use
                this.AssociatedObject.Loaded += (s, e) =>
                {
                    var ancestors = window.GetPart<Border>("PART_Border");
                    this.Border = ancestors;
                    if (this.ShouldHaveBorder())
                    {
                        this.AddBorder();
                    }
                };

                switch (this.AssociatedObject.ResizeMode)
                {
                    case ResizeMode.NoResize:
                        window.ShowMaxRestoreButton = false;
                        window.ShowMinButton = false;
                        this.ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanMinimize:
                        window.ShowMaxRestoreButton = false;
                        this.ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanResize:
                        this.ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanResizeWithGrip:
                        this.ResizeWithGrip = true;
                        break;
                }
            }
            else
            {
                //Other windows may not, easiest to just inject one!
                var content = (UIElement)this.AssociatedObject.Content;
                this.AssociatedObject.Content = null;

                this.Border = new Border { Child = content, BorderBrush = new SolidColorBrush(Colors.Black) };

                this.AssociatedObject.Content = this.Border;
            }

            if (this.ResizeWithGrip)
            {
                this.AssociatedObject.ResizeMode = ResizeMode.CanResizeWithGrip;
            }

            if (this.AutoSizeToContent)
            {
                this.AssociatedObject.Loaded += (s, e) =>
                {
                    //Temp fix, thanks @lynnx
                    this.AssociatedObject.SizeToContent = SizeToContent.Height;
                    this.AssociatedObject.SizeToContent = this.AutoSizeToContent
                        ? SizeToContent.WidthAndHeight
                        : SizeToContent.Manual;
                };
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.RemoveHwndHook();
            base.OnDetaching();
        }

        private static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
            {
                return UnsafeNativeMethods.SetClassLongPtr64(hWnd, nIndex, dwNewLong);
            }

            return new IntPtr(UnsafeNativeMethods.SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private void AddBorder()
        {
            if (this.Border == null)
            {
                return;
            }

            this.Border.BorderThickness = new Thickness(1);
            this.Border.BorderBrush = this._borderColour;
        }

        private void AddHwndHook()
        {
            this._mHWNDSource = PresentationSource.FromVisual(this.AssociatedObject) as HwndSource;
            if (this._mHWNDSource != null)
            {
                this._mHWNDSource.AddHook(this.HwndHook);
            }

            this._mHWND = new WindowInteropHelper(this.AssociatedObject).Handle;
        }

        private void AssociatedObjectStateChanged(object sender, EventArgs e)
        {
            if (this.AssociatedObject.WindowState == WindowState.Maximized)
            {
                this.HandleMaximize();
            }
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.AddHwndHook();
            this.SetDefaultBackgroundColor();
        }

        private void HandleMaximize()
        {
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(this._mHWND, Constants.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                bool ignoreTaskBar = this.AssociatedObject as MetroWindow != null
                                     && ((MetroWindow)this.AssociatedObject).IgnoreTaskbarOnMaximize;
                int x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                int y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                int cx = ignoreTaskBar ? monitorInfo.rcWork.right : Math.Abs(monitorInfo.rcWork.right - x);
                int cy = ignoreTaskBar ? monitorInfo.rcMonitor.bottom : Math.Abs(monitorInfo.rcWork.bottom - y);
                UnsafeNativeMethods.SetWindowPos(this._mHWND, new IntPtr(-2), x, y, cx, cy - 1, 0x0040);
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr returnval = IntPtr.Zero;
            switch (message)
            {
                case Constants.WM_NCCALCSIZE:
                    /* Hides the border */
                    handled = true;
                    break;
                case Constants.WM_NCPAINT:
                {
                    if (!this.ShouldHaveBorder())
                    {
                        var w = this.AssociatedObject as MetroWindow;
                        if (!(w != null && w.GlowBrush != null))
                        {
                            int val = 2;
                            UnsafeNativeMethods.DwmSetWindowAttribute(this._mHWND, 2, ref val, 4);
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            UnsafeNativeMethods.DwmExtendFrameIntoClientArea(this._mHWND, ref m);
                        }

                        if (this.Border != null)
                        {
                            this.Border.BorderThickness = new Thickness(0);
                        }
                    }
                    else
                    {
                        this.AddBorder();
                    }
                    handled = true;
                }
                    break;
                case Constants.WM_NCACTIVATE:
                {
                    /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                    returnval = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));

                    if (!this.ShouldHaveBorder())
                    {
                        if (wParam == IntPtr.Zero)
                        {
                            this.AddBorder();
                        }
                        else
                        {
                            this.RemoveBorder();
                        }
                    }

                    handled = true;
                }
                    break;
                case Constants.WM_GETMINMAXINFO:
                    /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(hWnd, lParam);

                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
                case Constants.WM_NCHITTEST:

                    // don't process the message on windows that can't be resized
                    ResizeMode resizeMode = this.AssociatedObject.ResizeMode;
                    if (resizeMode == ResizeMode.CanMinimize || resizeMode == ResizeMode.NoResize
                        || this.AssociatedObject.WindowState == WindowState.Maximized)
                    {
                        break;
                    }

                    // get X & Y out of the message                   
                    var screenPoint = new Point(
                        UnsafeNativeMethods.GET_X_LPARAM(lParam),
                        UnsafeNativeMethods.GET_Y_LPARAM(lParam));

                    // convert to window coordinates
                    Point windowPoint = this.AssociatedObject.PointFromScreen(screenPoint);
                    Size windowSize = this.AssociatedObject.RenderSize;
                    var windowRect = new Rect(windowSize);
                    windowRect.Inflate(-6, -6);

                    // don't process the message if the mouse is outside the 6px resize border
                    if (windowRect.Contains(windowPoint))
                    {
                        break;
                    }

                    var windowHeight = (int)windowSize.Height;
                    var windowWidth = (int)windowSize.Width;

                    // create the rectangles where resize arrows are shown
                    var topLeft = new Rect(0, 0, 6, 6);
                    var top = new Rect(6, 0, windowWidth - 12, 6);
                    var topRight = new Rect(windowWidth - 6, 0, 6, 6);

                    var left = new Rect(0, 6, 6, windowHeight - 12);
                    var right = new Rect(windowWidth - 6, 6, 6, windowHeight - 12);

                    var bottomLeft = new Rect(0, windowHeight - 6, 6, 6);
                    var bottom = new Rect(6, windowHeight - 6, windowWidth - 12, 6);
                    var bottomRight = new Rect(windowWidth - 6, windowHeight - 6, 6, 6);

                    // check if the mouse is within one of the rectangles
                    if (topLeft.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTTOPLEFT;
                    }
                    else if (top.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTTOP;
                    }
                    else if (topRight.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTTOPRIGHT;
                    }
                    else if (left.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTLEFT;
                    }
                    else if (right.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTRIGHT;
                    }
                    else if (bottomLeft.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTBOTTOMLEFT;
                    }
                    else if (bottom.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTBOTTOM;
                    }
                    else if (bottomRight.Contains(windowPoint))
                    {
                        returnval = (IntPtr)Constants.HTBOTTOMRIGHT;
                    }

                    if (returnval != IntPtr.Zero)
                    {
                        handled = true;
                    }

                    break;

                case Constants.WM_INITMENU:
                    var window = this.AssociatedObject as MetroWindow;

                    if (window != null)
                    {
                        if (!window.ShowMaxRestoreButton)
                        {
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MAXIMIZE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }
                        else if (window.WindowState == WindowState.Maximized)
                        {
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MAXIMIZE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_RESTORE,
                                Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MOVE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }
                        else
                        {
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MAXIMIZE,
                                Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_RESTORE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MOVE,
                                Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                        }

                        if (!window.ShowMinButton)
                        {
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_MINIMIZE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }

                        if (this.AssociatedObject.ResizeMode == ResizeMode.NoResize
                            || window.WindowState == WindowState.Maximized)
                        {
                            UnsafeNativeMethods.EnableMenuItem(
                                UnsafeNativeMethods.GetSystemMenu(hWnd, false),
                                Constants.SC_SIZE,
                                Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }
                    }
                    break;
            }

            return returnval;
        }

        private void RemoveBorder()
        {
            if (this.Border == null)
            {
                return;
            }

            this.Border.BorderThickness = new Thickness(0);
            this.Border.BorderBrush = null;
        }

        private void RemoveHwndHook()
        {
            this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
            this._mHWNDSource.RemoveHook(this.HwndHook);
        }

        private void SetDefaultBackgroundColor()
        {
            var bgSolidColorBrush = this.AssociatedObject.Background as SolidColorBrush;

            if (bgSolidColorBrush != null)
            {
                int rgb = bgSolidColorBrush.Color.R | (bgSolidColorBrush.Color.G << 8)
                          | (bgSolidColorBrush.Color.B << 16);

                // set the default background color of the window -> this avoids the black stripes when resizing
                IntPtr hBrushOld = SetClassLong(
                    this._mHWND,
                    Constants.GCLP_HBRBACKGROUND,
                    UnsafeNativeMethods.CreateSolidBrush(rgb));

                if (hBrushOld != IntPtr.Zero)
                {
                    UnsafeNativeMethods.DeleteObject(hBrushOld);
                }
            }
        }

        private bool ShouldHaveBorder()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return true;
            }

            if (!UnsafeNativeMethods.DwmIsCompositionEnabled())
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}