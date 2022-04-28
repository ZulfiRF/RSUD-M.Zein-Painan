using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Core.Framework.Windows.Framework;
    using Core.Framework.Windows.Utilities;

    /// <summary>
    ///     Interaction logic for WindowsManager.xaml
    /// </summary>
    [StyleTypedProperty(Property = "DockIllustrationContentStyleProperty", StyleTargetType = typeof(TabItem))]
    [StyleTypedProperty(Property = "DockPaneIllustrationStyle", StyleTargetType = typeof(Panel))]
    public partial class WindowsManager
    {
        #region Static Fields

        /// <summary>
        ///     Style property for illustrating how a docked content would look like in tab control
        /// </summary>
        public static readonly DependencyProperty DockIllustrationContentStyleProperty =
            DependencyProperty.Register("DockIllustrationContentStyle", typeof(Style), typeof(WindowsManager));

        /// <summary>
        ///     Style property for illustrating how a docked pane would look like in tab control
        /// </summary>
        public static readonly DependencyProperty DockPaneIllustrationStyleProperty =
            DependencyProperty.Register("DockPaneIllustrationStyle", typeof(Style), typeof(WindowsManager));

        #endregion

        #region Fields

        private readonly ObservableDependencyPropertyCollection<DockPane> _dockPaneStateMonitorList;

        private readonly DispatcherTimer _popupTimer = new DispatcherTimer();

        private bool _dockPaneDragging;

        private Point _dragStartPointOffset;

        private bool _mouseOverPopupPane;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowsManager" /> class.
        /// </summary>
        public WindowsManager()
        {
            this.InitializeComponent();
            this._dockPaneStateMonitorList =
                new ObservableDependencyPropertyCollection<DockPane>(DockPane.DockPaneStateProperty);
            this._dockPaneStateMonitorList.DependencyPropertyChanged += this.OnDockPaneStateChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Active windows manager
        /// </summary>
        public static WindowsManager ActiveWindowsManager { get; private set; }

        /// <summary>
        ///     Dragged pane
        /// </summary>
        /// <remarks>
        ///     Needs to be static since there can be only one dragged pane at any time
        ///     and also since DraggedPanes can change ownership from one WindowsManager instance to another
        /// </remarks>
        public DockPane DraggedPane { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the dock content illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Style GetDockIllustrationContentStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DockIllustrationContentStyleProperty);
        }

        /// <summary>
        ///     Gets the dock pane illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Style GetDockPaneIllustrationStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DockPaneIllustrationStyleProperty);
        }

        /// <summary>
        ///     Sets the dock content illustration style.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockIllustrationContentStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DockIllustrationContentStyleProperty, value);
        }

        /// <summary>
        ///     Sets the dock pane illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockPaneIllustrationStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DockPaneIllustrationStyleProperty, value);
        }

        /// <summary>
        ///     Adds the window in auto hide fashion
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        public void AddAutoHideWindow(DockPane pane, Dock dock)
        {
            this.CondenceDockPanel(pane, dock);

            this.DetachDockPaneEvents(pane);
            this.AttachDockPaneEvents(pane);
        }

        /// <summary>
        ///     Adds the floating window.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void AddFloatingWindow(DockPane pane)
        {
            this.DetachDockPaneEvents(pane);
            this.AttachDockPaneEvents(pane);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Setting state to non-floating so that OnPaneDragStared can 
                // set it to floating and execute related effects
                pane.DockPaneState = DockPaneState.Content;
                this.OnPaneDragStarted(pane, null);
            }
            else
            {
                pane.DockPaneState = DockPaneState.Floating;
                this.FloatingPanel.Children.Add(pane);
                this.MonitorStateChangeForDockPane(pane);
            }
        }

        /// <summary>
        ///     Adds the dock pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        public void AddPinnedWindow(DockPane pane, Dock dock)
        {
            this.AddPinnedWindowInner(pane, dock);

            this.DetachDockPaneEvents(pane);
            this.AttachDockPaneEvents(pane);
        }

        /// <summary>
        ///     Clears the windows manager
        /// </summary>
        public void Clear()
        {
            Action<Panel> clearAction = panel => panel.Children.Clear();
            clearAction(this.TopPinnedWindows);
            clearAction(this.TopWindowHeaders);
            clearAction(this.BottomPinnedWindows);
            clearAction(this.BottomWindowHeaders);
            clearAction(this.LeftPinnedWindows);
            clearAction(this.LeftWindowHeaders);
            clearAction(this.RightPinnedWindows);
            clearAction(this.RightWindowHeaders);
            clearAction(this.FloatingPanel);
            clearAction(this.PopupArea);
            this.DocumentContainer.Content = null;
            this.DocumentContainer.Clear();
            this._dockPaneStateMonitorList.Clear();
            this._popupTimer.Stop();
        }

        /// <summary>
        ///     Removes the dock pane from windows manager alltogether and unsubscribes from all events
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void RemoveDockPane(DockPane pane)
        {
            this.DetachDockPaneEvents(pane);
            this.RemoveCondencedDockPanel(this.DraggedPane.CondencedDockPanel);
            this.RemovePinnedWindow(this.DraggedPane);
            this.FloatingPanel.Children.Remove(pane);
        }

        /// <summary>
        ///     Starts the dock pane state change detection
        /// </summary>
        public void StartDockPaneStateChangeDetection()
        {
            this.MonitorStateChangeForDockPane(this.DraggedPane);
        }

        /// <summary>
        ///     Stops the dock pane state change detection
        /// </summary>
        public void StopDockPaneStateChangeDetection()
        {
            this.IgnoreStateChangeForDockPane(this.DraggedPane);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" />
        ///     attached event reaches an element in its route that is derived from this class.
        ///     Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!this._dockPaneDragging)
            {
                return;
            }

            Point currentMousePosition = e.GetPosition(this);
            double currentPaneXPos = Canvas.GetLeft(this.DraggedPane);
            double currentPaneYPos = Canvas.GetTop(this.DraggedPane);

            Canvas.SetTop(this.DraggedPane, currentMousePosition.Y - this._dragStartPointOffset.Y);
            Canvas.SetLeft(this.DraggedPane, currentMousePosition.X - this._dragStartPointOffset.X);
        }

        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event
        ///     reaches an element in its route that is derived from this class. Implement this method to add class
        ///     handling for this event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data.
        ///     The event data reports that the mouse button was released.
        /// </param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (!this._dockPaneDragging)
            {
                return;
            }

            this.ReleaseMouseCapture();

            this.DraggedPane.Opacity = 1;
            this._dockPaneDragging = false;
            this.DockingPanel.Visibility = Visibility.Collapsed;

            this.DraggedPane.IsHitTestVisible = true;

            this.DraggedPane = null;
            ActiveWindowsManager = null;
        }

        /// <summary>
        ///     Adds the pinned window without manipulating events
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        private void AddPinnedWindowInner(DockPane pane, Dock dock)
        {
            switch (dock)
            {
                case Dock.Bottom:
                    this.AddPinnedWindowToBottom(pane);
                    break;
                case Dock.Left:
                    this.AddPinnedWindowToLeft(pane);
                    break;
                case Dock.Right:
                    this.AddPinnedWindowToRight(pane);
                    break;
                case Dock.Top:
                    this.AddPinnedWindowToTop(pane);
                    break;
                default:
                    break;
            }

            pane.DockPaneState = DockPaneState.Docked;
        }

        /// <summary>
        ///     Adds the pinned window to bottom of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToBottom(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Bottom);
            pane.Width = double.NaN;
            this.BottomPinnedWindows.Children.Add(pane);

            var sizingThumb = new GridSplitter();
            sizingThumb.Height = 4;
            sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, Dock.Bottom);
            this.BottomPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Height.Equals(double.NaN))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                if (pane.Height - b.VerticalChange <= 0)
                {
                    return;
                }

                pane.Height -= b.VerticalChange;
            };
        }

        /// <summary>
        ///     Adds the pinned window to left of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToLeft(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Left);
            pane.Height = double.NaN;
            this.LeftPinnedWindows.Children.Add(pane);

            var sizingThumb = new GridSplitter();
            sizingThumb.Width = 4;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeWE;
            DockPanel.SetDock(sizingThumb, Dock.Left);
            this.LeftPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Width.Equals(double.NaN))
                {
                    pane.Width = pane.DesiredSize.Width;
                }

                if (pane.Width + b.HorizontalChange <= 0)
                {
                    return;
                }

                pane.Width += b.HorizontalChange;
            };
        }

        /// <summary>
        ///     Adds the pinned window to right of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToRight(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Right);
            pane.Height = double.NaN;
            this.RightPinnedWindows.Children.Add(pane);

            var sizingThumb = new GridSplitter();
            sizingThumb.Width = 4;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeWE;
            DockPanel.SetDock(sizingThumb, Dock.Right);
            this.RightPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Width.Equals(double.NaN))
                {
                    pane.Width = pane.DesiredSize.Width;
                }

                if (pane.Width - b.HorizontalChange <= 0)
                {
                    return;
                }

                pane.Width -= b.HorizontalChange;
            };
        }

        /// <summary>
        ///     Adds the pinned window to top of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToTop(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Top);
            pane.Width = double.NaN;
            this.TopPinnedWindows.Children.Add(pane);

            var sizingThumb = new GridSplitter();
            sizingThumb.Height = 4;
            sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, Dock.Top);
            this.TopPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Height.Equals(double.NaN))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                if (pane.Height + b.VerticalChange <= 0)
                {
                    return;
                }

                pane.Height += b.VerticalChange;
            };
        }

        /// <summary>
        ///     Attaches the events to dock pane
        /// </summary>
        /// <param name="pane">Dock pane</param>
        private void AttachDockPaneEvents(DockPane pane)
        {
            pane.Close += this.OnDockPaneClosed;

            this.MonitorStateChangeForDockPane(pane);

            pane.HeaderDrag += this.OnPaneDragStarted;
        }

        /// <summary>
        ///     Attaches the events.
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel.</param>
        private void AttachEvents(FrameworkElement condencedDockPanel)
        {
            condencedDockPanel.MouseEnter += this.OnCondencedDockPanelMouseEnter;
            condencedDockPanel.MouseLeave += this.OnPopupAreaMouseLeave;
        }

        /// <summary>
        ///     Condences the dock panel
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        private void CondenceDockPanel(DockPane pane, Dock dock)
        {
            FrameworkElement condencedDockPanel = pane.CondencedDockPanel;
            condencedDockPanel.LayoutTransform = (dock == Dock.Left) || (dock == Dock.Right)
                ? new RotateTransform(90)
                : null;

            switch (dock)
            {
                case Dock.Bottom:
                    this.BottomWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Left:
                    this.LeftWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Right:
                    this.RightWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Top:
                    this.TopWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                default:
                    break;
            }

            DockPanel.SetDock(pane, dock);
            this.DetachEvents(condencedDockPanel);
            pane.DockPaneState = DockPaneState.AutoHide;
            this.AttachEvents(condencedDockPanel);
        }

        /// <summary>
        ///     Detaches the events from the dock pane
        /// </summary>
        /// <param name="pane">The pane.</param>
        private void DetachDockPaneEvents(DockPane pane)
        {
            pane.Close -= this.OnDockPaneClosed;

            this.IgnoreStateChangeForDockPane(pane);

            pane.HeaderDrag -= this.OnPaneDragStarted;
        }

        /// <summary>
        ///     Detaches the events.
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel.</param>
        private void DetachEvents(FrameworkElement condencedDockPanel)
        {
            condencedDockPanel.MouseEnter -= this.OnCondencedDockPanelMouseEnter;
            condencedDockPanel.MouseLeave -= this.OnPopupAreaMouseLeave;
        }

        /// <summary>
        ///     Enables the popup timer
        /// </summary>
        private void EnablePopupTimer()
        {
            this._popupTimer.Stop();
            this._popupTimer.Interval = TimeSpan.FromMilliseconds(200);
            this._popupTimer.Tick += this.OnPopupTimerElapsed;
            this._popupTimer.Start();
        }

        /// <summary>
        ///     Ignores the state change for dock pane.
        /// </summary>
        /// <param name="pane">Dock pane to ignore state changes from</param>
        private void IgnoreStateChangeForDockPane(DockPane pane)
        {
            if (pane != null)
            {
                this._dockPaneStateMonitorList.Remove(pane);
            }
        }

        /// <summary>
        ///     Monitors the state change for dock pane and ensures that
        ///     only one instance of pane is monitored to prevent redundent events
        /// </summary>
        /// <param name="pane">Pane to monitor</param>
        private void MonitorStateChangeForDockPane(DockPane pane)
        {
            if ((pane != null) && (!this._dockPaneStateMonitorList.Contains(pane)))
            {
                this._dockPaneStateMonitorList.Add(pane);
            }
        }

        /// <summary>
        ///     Called when mouse enters condenced dock panel
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void OnCondencedDockPanelMouseEnter(object source, RoutedEventArgs args)
        {
            var element = source as FrameworkElement;
            Validate.Assert<ArgumentException>(element != null);

            var pane = element.DataContext as DockPane;
            Validate.Assert<ArgumentException>(pane != null);

            this.RemovePopupEvents();
            this.PopupArea.Children.Clear();
            Dock dock = DockPanel.GetDock(pane);

            this.PopupArea.Children.Add(pane);

            bool isLeftOrRightDock = (dock == Dock.Left) || (dock == Dock.Right);

            var sizingThumb = new GridSplitter();

            if (isLeftOrRightDock)
            {
                sizingThumb.Width = 4;
                sizingThumb.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else
            {
                sizingThumb.Height = 4;
                sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            }

            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = isLeftOrRightDock ? Cursors.SizeWE : Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, dock);
            this.PopupArea.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (c, d) =>
            {
                if (isLeftOrRightDock && (pane.Width.Equals(double.NaN)))
                {
                    pane.Width = pane.DesiredSize.Width;
                }
                else if ((!(isLeftOrRightDock)) && (pane.Height.Equals(double.NaN)))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                double result = 0;
                switch (dock)
                {
                    case Dock.Bottom:
                        result = pane.Height - d.VerticalChange;
                        break;
                    case Dock.Left:
                        result = pane.Width + d.HorizontalChange;
                        break;
                    case Dock.Right:
                        result = pane.Width - d.HorizontalChange;
                        break;
                    case Dock.Top:
                        result = pane.Height + d.VerticalChange;
                        break;
                }

                if (result <= 0)
                {
                    return;
                }

                if (isLeftOrRightDock)
                {
                    pane.Width = result;
                }
                else
                {
                    pane.Height = result;
                }
            };

            pane.MouseLeave += this.OnPopupAreaMouseLeave;
            sizingThumb.MouseLeave += this.OnPopupAreaMouseLeave;
            pane.MouseEnter += this.OnPopupAreaMouseEnter;
            sizingThumb.MouseEnter += this.OnPopupAreaMouseEnter;

            this._mouseOverPopupPane = true;

            this.EnablePopupTimer();
        }

        /// <summary>
        ///     Called when dock pane is closed
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void OnDockPaneClosed(object sender, RoutedEventArgs args)
        {
            var pane = args.OriginalSource as DockPane;
            Validate.Assert<InvalidOperationException>(pane != null);

            this.DetachDockPaneEvents(pane);

            // Pane is pinned, parent must be a dock panel)
            if (pane.DockPaneState == DockPaneState.Docked)
            {
                this.RemovePinnedWindow(pane);
            }
            else if (pane.DockPaneState == DockPaneState.AutoHide)
            {
                this.PopupArea.Children.Remove(pane);
                this.RemoveCondencedDockPanel(pane.CondencedDockPanel);
            }
            else if (pane.DockPaneState == DockPaneState.Floating)
            {
                this.FloatingPanel.Children.Remove(pane);
            }
        }

        /// <summary>
        ///     Called when dock pane's state is changed
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private void OnDockPaneStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var pane = sender as DockPane;
                Validate.Assert<ArgumentException>(pane != null);

                this.IgnoreStateChangeForDockPane(pane);

                var state = (DockPaneState)e.NewValue;

                if (state == DockPaneState.AutoHide)
                {
                    var logicalParentDockPanel = LogicalTreeHelper.GetParent(pane) as DockPanel;
                    this.RemovePinnedWindow(pane);
                    this.CondenceDockPanel(pane, DockPanel.GetDock(logicalParentDockPanel));
                }
                else if (state == DockPaneState.Docked)
                {
                    this.PopupArea.Children.Remove(pane);
                    this.RemoveCondencedDockPanel(pane.CondencedDockPanel);

                    this.AddPinnedWindowInner(pane, DockPanel.GetDock(pane));
                }

                this.MonitorStateChangeForDockPane(pane);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Called when dock pane drag has started
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnPaneDragStarted(object sender, MouseButtonEventArgs e)
        {
            ActiveWindowsManager = this;
            this.DraggedPane = sender as DockPane;
            this._dockPaneDragging = true;
            this._dragStartPointOffset = (e != null) ? e.GetPosition(this.DraggedPane) : Mouse.GetPosition(this);

            // Show the visibility
            this.DockingPanel.Visibility = Visibility.Visible;

            this.RemoveCondencedDockPanel(this.DraggedPane.CondencedDockPanel);
            this.RemovePinnedWindow(this.DraggedPane);

            // Dim the dragged window
            this.DraggedPane.Opacity = 0.25;

            if (this.DraggedPane.DockPaneState != DockPaneState.Floating)
            {
                this.IgnoreStateChangeForDockPane(this.DraggedPane);

                Point panePosition = e != null ? e.GetPosition(this) : Mouse.GetPosition(this);

                Canvas.SetTop(this.DraggedPane, panePosition.Y);
                Canvas.SetLeft(this.DraggedPane, panePosition.X);
                this.FloatingPanel.Children.Add(this.DraggedPane);
                this.MonitorStateChangeForDockPane(this.DraggedPane);

                // Set this last since until the DraggedPane is not added
                // to the FloatingPanel it will not be in the visual tree
                // and hence things like adorner layer will not be present
                this.DraggedPane.DockPaneState = DockPaneState.Floating;
            }

            this.DraggedPane.IsHitTestVisible = false;

            // This is necessary or when tab items are floated again
            // mouse up event never fires
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        ///     Called when mouse enters in current popup area (condenced dock panel or dock panel)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private void OnPopupAreaMouseEnter(object sender, MouseEventArgs e)
        {
            this._mouseOverPopupPane = true;
        }

        /// <summary>
        ///     Called when mouse leaves current popup area (condenced dock panel or dock panel)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private void OnPopupAreaMouseLeave(object sender, MouseEventArgs e)
        {
            this._mouseOverPopupPane = false;
        }

        /// <summary>
        ///     Called when popup timer has elapsed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnPopupTimerElapsed(object sender, EventArgs e)
        {
            this._popupTimer.Tick -= this.OnPopupTimerElapsed;
            this._popupTimer.Stop();

            if (!this._mouseOverPopupPane)
            {
                this.PopupArea.Children.Clear();
            }
            else
            {
                this.EnablePopupTimer();
            }
        }

        /// <summary>
        ///     Removes the condenced dock panel
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel</param>
        private void RemoveCondencedDockPanel(FrameworkElement condencedDockPanel)
        {
            this.BottomWindowHeaders.Children.Remove(condencedDockPanel);
            this.LeftWindowHeaders.Children.Remove(condencedDockPanel);
            this.RightWindowHeaders.Children.Remove(condencedDockPanel);
            this.TopWindowHeaders.Children.Remove(condencedDockPanel);
            this.DetachEvents(condencedDockPanel);
        }

        /// <summary>
        ///     Removes the pinned window
        /// </summary>
        /// <param name="pane">Dock pane</param>
        private void RemovePinnedWindow(DockPane pane)
        {
            var logicalParentDockPanel = LogicalTreeHelper.GetParent(pane) as DockPanel;

            if (logicalParentDockPanel == null)
            {
                return;
            }

            int indexOfPane = logicalParentDockPanel.Children.IndexOf(pane);

            var splitter = logicalParentDockPanel.Children[indexOfPane + 1] as GridSplitter;
            logicalParentDockPanel.Children.Remove(pane);
            logicalParentDockPanel.Children.Remove(splitter);
        }

        /// <summary>
        ///     Removes the popup events
        /// </summary>
        private void RemovePopupEvents()
        {
            if (this.PopupArea.Children.Count == 0)
            {
                return;
            }

            var pane = this.PopupArea.Children[0] as DockPane;

            if (pane != null)
            {
                pane.MouseEnter -= this.OnPopupAreaMouseEnter;
                pane.MouseLeave -= this.OnPopupAreaMouseLeave;
            }
        }

        #endregion
    }
}