namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;

    using Core.Framework.Windows.Models.Win32;

    partial class GlowWindow : Window
    {
        #region Constants

        private const double edgeSize = 20.0;

        private const double glowSize = 9.0;

        #endregion

        #region Static Fields

        private static double? _dpiFactor;

        #endregion

        #region Fields

        private readonly Func<Point, Cursor> getCursor;

        private readonly Func<double> getHeight;

        private readonly Func<Point, HitTestValues> getHitTestValue;

        private readonly Func<double> getLeft;

        private readonly Func<double> getTop;

        private readonly Func<double> getWidth;

        private readonly Window owner;

        private IntPtr handle;

        private IntPtr ownerHandle;

        #endregion

        #region Constructors and Destructors

        public GlowWindow(Window owner, GlowDirection direction)
        {
            this.InitializeComponent();

            this.owner = owner;
            this.glow.Visibility = Visibility.Collapsed;

            var b = new Binding("GlowBrush.Color");
            b.Source = owner;
            this.glow.SetBinding(Glow.GlowColorProperty, b);

            switch (direction)
            {
                case GlowDirection.Left:
                    this.glow.Orientation = Orientation.Vertical;
                    this.glow.HorizontalAlignment = HorizontalAlignment.Right;
                    this.getLeft = () => Math.Ceiling(owner.Left - glowSize);
                    this.getTop = () => owner.Top - glowSize;
                    this.getWidth = () => glowSize;
                    this.getHeight = () => owner.ActualHeight + glowSize * 2;
                    this.getHitTestValue =
                        p =>
                            new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
                                ? HitTestValues.HTTOPLEFT
                                : new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
                                    ? HitTestValues.HTBOTTOMLEFT
                                    : HitTestValues.HTLEFT;
                    this.getCursor =
                        p =>
                            new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
                                ? Cursors.SizeNWSE
                                : new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
                                    ? Cursors.SizeNESW
                                    : Cursors.SizeWE;
                    break;
                case GlowDirection.Right:
                    this.glow.Orientation = Orientation.Vertical;
                    this.glow.HorizontalAlignment = HorizontalAlignment.Left;
                    this.getLeft = () => owner.Left + owner.ActualWidth;
                    this.getTop = () => owner.Top - glowSize;
                    this.getWidth = () => glowSize;
                    this.getHeight = () => owner.ActualHeight + glowSize * 2;
                    this.getHitTestValue =
                        p =>
                            new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
                                ? HitTestValues.HTTOPRIGHT
                                : new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
                                    ? HitTestValues.HTBOTTOMRIGHT
                                    : HitTestValues.HTRIGHT;
                    this.getCursor =
                        p =>
                            new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
                                ? Cursors.SizeNESW
                                : new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
                                    ? Cursors.SizeNWSE
                                    : Cursors.SizeWE;
                    break;
                case GlowDirection.Top:
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Bottom;
                    this.getLeft = () => owner.Left;
                    this.getTop = () => Math.Ceiling(owner.Top - glowSize);
                    this.getWidth = () => owner.ActualWidth;
                    this.getHeight = () => glowSize;
                    this.getHitTestValue =
                        p =>
                            new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
                                ? HitTestValues.HTTOPLEFT
                                : new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight)
                                    .Contains(p)
                                    ? HitTestValues.HTTOPRIGHT
                                    : HitTestValues.HTTOP;
                    this.getCursor =
                        p =>
                            new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
                                ? Cursors.SizeNWSE
                                : new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight)
                                    .Contains(p)
                                    ? Cursors.SizeNESW
                                    : Cursors.SizeNS;
                    break;
                case GlowDirection.Bottom:
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Top;
                    this.getLeft = () => owner.Left;
                    this.getTop = () => owner.Top + owner.ActualHeight;
                    this.getWidth = () => owner.ActualWidth;
                    this.getHeight = () => glowSize;
                    this.getHitTestValue =
                        p =>
                            new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
                                ? HitTestValues.HTBOTTOMLEFT
                                : new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight)
                                    .Contains(p)
                                    ? HitTestValues.HTBOTTOMRIGHT
                                    : HitTestValues.HTBOTTOM;
                    this.getCursor =
                        p =>
                            new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
                                ? Cursors.SizeNESW
                                : new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight)
                                    .Contains(p)
                                    ? Cursors.SizeNWSE
                                    : Cursors.SizeNS;
                    break;
            }

            owner.ContentRendered += (sender, e) => this.glow.Visibility = Visibility.Visible;
            owner.Activated += (sender, e) => this.Update();
            owner.Activated += (sender, e) => this.glow.IsGlow = true;
            owner.Deactivated += (sender, e) => this.glow.IsGlow = false;
            owner.LocationChanged += (sender, e) => this.Update();
            owner.SizeChanged += (sender, e) => this.Update();
            owner.StateChanged += (sender, e) => this.Update();
            owner.Closed += (sender, e) => this.Close();
        }

        #endregion

        #region Public Properties

        public static double DpiFactor
        {
            get
            {
                if (_dpiFactor == null)
                {
                    PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
                    double dpiX = 96.0, dpiY = 96.0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }
                    if (dpiX == dpiY)
                    {
                        _dpiFactor = dpiX / 96.0;
                    }
                }
                return _dpiFactor.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Update()
        {
            try
            {


                if (this.owner.WindowState == WindowState.Normal)
                {
                    this.Visibility = Visibility.Visible;

                    this.UpdateCore();
                }
                else if (this.owner.Visibility == Visibility.Hidden)
                {
                    this.Visibility = Visibility.Hidden;

                    this.UpdateCore();
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Methods

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var source = (HwndSource)PresentationSource.FromVisual(this);
            WS ws = source.Handle.GetWindowLong();
            WSEX wsex = source.Handle.GetWindowLongEx();

            //ws |= WS.POPUP;
            wsex ^= WSEX.APPWINDOW;
            wsex |= WSEX.NOACTIVATE;

            source.Handle.SetWindowLong(ws);
            source.Handle.SetWindowLongEx(wsex);
            source.AddHook(this.WndProc);

            this.handle = source.Handle;
        }

        private void UpdateCore()
        {
            try
            {


                if (this.ownerHandle == IntPtr.Zero)
                {
                    this.ownerHandle = new WindowInteropHelper(this.owner).Handle;
                }

                NativeMethods.SetWindowPos(
                    this.handle,
                    this.ownerHandle,
                    (int)(this.getLeft() * DpiFactor),
                    (int)(this.getTop() * DpiFactor),
                    (int)(this.getWidth() * DpiFactor),
                    (int)(this.getHeight() * DpiFactor),
                    SWP.NOACTIVATE);
            }
            catch (Exception)
            {
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM.MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(3);
            }

            if (msg == (int)WM.LBUTTONDOWN)
            {
                var pt = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);

                NativeMethods.PostMessage(
                    this.ownerHandle,
                    (uint)WM.NCLBUTTONDOWN,
                    (IntPtr)this.getHitTestValue(pt),
                    IntPtr.Zero);
            }
            if (msg == (int)WM.NCHITTEST)
            {
                var ptScreen = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
                Point ptClient = this.PointFromScreen(ptScreen);
                Cursor cursor = this.getCursor(ptClient);
                if (cursor != this.Cursor)
                {
                    this.Cursor = cursor;
                }
            }

            return IntPtr.Zero;
        }

        #endregion
    }
}