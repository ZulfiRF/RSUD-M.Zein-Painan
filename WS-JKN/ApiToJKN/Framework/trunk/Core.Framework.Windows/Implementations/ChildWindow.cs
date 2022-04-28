namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;

    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Native;

    /// <summary>
    ///     Interaction logic for ChildWindow.xaml
    /// </summary>
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public partial class ChildWindow : Window
    {
        #region Constants

        private const string PART_Close = "PART_Close";

        private const string PART_TitleBar = "PART_TitleBar";

        private const string PART_WindowCommands = "PART_WindowCommands";

        #endregion

        #region Fields

        protected MetroWindow parentWindow;

        private bool isDragging;

        #endregion

        #region Constructors and Destructors

        static ChildWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ChildWindow),
                new FrameworkPropertyMetadata(typeof(ChildWindow)));
        }

        public ChildWindow()
        {
            this.InitializeComponent();
            this.parentWindow = Application.Current.MainWindow as MetroWindow;
            this.Closing += this.ChildWindowClosing;
        }

        #endregion

        #region Public Properties

        public WindowCommands WindowCommands { get; set; }

        #endregion

        #region Properties

        protected Button PartCloseBtn { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.WindowCommands == null)
            {
                this.WindowCommands = new WindowCommands();
            }

            var titleBar = this.GetTemplateChild(PART_TitleBar) as UIElement;

            this.PartCloseBtn = this.GetTemplateChild(PART_Close) as Button;
            if (this.PartCloseBtn != null)
            {
                this.PartCloseBtn.Click += this.PartCloseBtnOnClick;
            }

            if (titleBar != null)
            {
                titleBar.MouseDown += this.TitleBarMouseDown;
                titleBar.MouseUp += this.TitleBarMouseUp;
                titleBar.MouseMove += this.TitleBarMouseMove;
            }
            else
            {
                this.MouseDown += this.TitleBarMouseDown;
                this.MouseUp += this.TitleBarMouseUp;
                this.MouseMove += this.TitleBarMouseMove;
            }
        }

        #endregion

        #region Methods

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            var frameworkElement = newContent as FrameworkElement;
            if (frameworkElement != null)
            {
                this.Width = frameworkElement.Width;
                this.Height = frameworkElement.Height + 60;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowCommands != null)
            {
                //  WindowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = true;
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null)
            {
                return;
            }

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
            {
                return;
            }

            IntPtr hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            uint cmd = UnsafeNativeMethods.TrackPopupMenuEx(
                hmenu,
                Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD,
                (int)physicalScreenLocation.X,
                (int)physicalScreenLocation.Y,
                hwnd,
                IntPtr.Zero);
            if (0 != cmd)
            {
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        private void ChildWindowClosing(object sender, CancelEventArgs e)
        {
            this.parentWindow.IsActiveWin = true;
        }

        private void PartCloseBtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Close();
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.isDragging = false;
            }

            if (this.isDragging)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = this.PointToScreen(Mouse.GetPosition(this));
                double width = this.RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Check if the mouse is at the top of the screen if TitleBar is not visible

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                Point mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                this.Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                this.Left = left;

                // Restore window to normal state.

                this.DragMove();
            }
        }

        #endregion
    }
}