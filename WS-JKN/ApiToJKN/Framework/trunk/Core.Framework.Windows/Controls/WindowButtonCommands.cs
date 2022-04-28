using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Core.Framework.Windows.Native;

namespace Core.Framework.Windows.Controls
{
    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    public class WindowButtonCommands : ContentControl
    {
        #region Static Fields

        private static string closeText;

        private static string maximize;

        private static string minimize;

        private static string restore;

        #endregion

        #region Fields

        private Button close;

        private Button max;

        private Button min;

        private IntPtr user32 = IntPtr.Zero;

        #endregion

        #region Constructors and Destructors

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WindowButtonCommands),
                new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        ~WindowButtonCommands()
        {
            if (this.user32 != IntPtr.Zero)
            {
                UnsafeNativeMethods.FreeLibrary(this.user32);
            }
        }

        #endregion

        #region Delegates

        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        #endregion

        #region Public Events

        public event ClosingWindowEventHandler ClosingWindow;

        #endregion

        #region Public Properties

        public string Close
        {
            get
            {
                if (string.IsNullOrEmpty(closeText))
                {
                    closeText = this.GetCaption(905);
                }
                return closeText;
            }
        }

        public string Maximize
        {
            get
            {
                if (string.IsNullOrEmpty(maximize))
                {
                    maximize = this.GetCaption(901);
                }
                return maximize;
            }
        }

        public string Minimize
        {
            get
            {
                if (string.IsNullOrEmpty(minimize))
                {
                    minimize = this.GetCaption(900);
                }
                return minimize;
            }
        }

        public string Restore
        {
            get
            {
                if (string.IsNullOrEmpty(restore))
                {
                    restore = this.GetCaption(903);
                }
                return restore;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.close = this.Template.FindName("PART_Close", this) as Button;
            if (this.close != null)
            {
                this.close.Click += this.CloseClick;
            }

            this.max = this.Template.FindName("PART_Max", this) as Button;
            if (this.max != null)
            {
                this.max.Click += this.MaximiseClick;
            }

            this.min = this.Template.FindName("PART_Min", this) as Button;
            if (this.min != null)
            {
                this.min.Click += this.MinimiseClick;
            }

            this.RefreshMaximiseIconState();
        }

        public void RefreshMaximiseIconState()
        {
            this.RefreshMaximiseIconState(this.GetParentWindow());
        }

        #endregion

        #region Methods

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            ClosingWindowEventHandler handler = this.ClosingWindow;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            this.OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled)
            {
                return;
            }

            Window parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private string GetCaption(int id)
        {
            if (this.user32 == IntPtr.Zero)
            {
                this.user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            }

            var sb = new StringBuilder(256);
            UnsafeNativeMethods.LoadString(this.user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        private Window GetParentWindow()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentWindow = parent as Window;
            return parentWindow;
        }

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            Window parentWindow = this.GetParentWindow();
            if (parentWindow == null)
            {
                return;
            }

            parentWindow.WindowState = parentWindow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            this.RefreshMaximiseIconState(parentWindow);
        }

        private void MinimiseClick(object sender, RoutedEventArgs e)
        {
            Window parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
        }

        private void RefreshMaximiseIconState(Window parentWindow)
        {
            if (parentWindow == null)
            {
                return;
            }

            if (parentWindow.WindowState == WindowState.Normal)
            {
                var maxpath = (Path)this.max.FindName("MaximisePath");
                maxpath.Visibility = Visibility.Visible;

                var restorepath = (Path)this.max.FindName("RestorePath");
                restorepath.Visibility = Visibility.Collapsed;

                this.max.ToolTip = this.Maximize;
            }
            else
            {
                var restorepath = (Path)this.max.FindName("RestorePath");
                restorepath.Visibility = Visibility.Visible;

                var maxpath = (Path)this.max.FindName("MaximisePath");
                maxpath.Visibility = Visibility.Collapsed;
                this.max.ToolTip = this.Restore;
            }
        }

        #endregion
    }
}