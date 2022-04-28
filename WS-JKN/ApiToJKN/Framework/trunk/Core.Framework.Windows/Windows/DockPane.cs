namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Core.Framework.Windows.Adorners;
    using Core.Framework.Windows.Commands;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Extensions;

    /// <summary>
    ///     DockPanel class
    /// </summary>
    [TemplatePart(Name = "PART_DOCK_PANE_HEADER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PIN", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_DOCK_PANE_MENU", Type = typeof(Button))]
    public class DockPane : HeaderedContentControl, ICloseControl
    {
        #region Static Fields

        /// <summary>
        ///     Close event
        /// </summary>
        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(DockPane));

        /// <summary>
        ///     Header drag event
        /// </summary>
        public static readonly RoutedEvent HeaderDragEvent = EventManager.RegisterRoutedEvent(
            "HeaderDrag",
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(DockPane));

        /// <summary>
        ///     Toggle pin event
        /// </summary>
        public static readonly RoutedEvent TogglePinEvent = EventManager.RegisterRoutedEvent(
            "TogglePin",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(DockPane));

        /// <summary>
        ///     Condenced dock panel template property
        /// </summary>
        public static DependencyProperty CondencedDockPanelTemplateProperty =
            DependencyProperty.Register("CondencedDockPanelTemplate", typeof(DataTemplate), typeof(DockPane));

        /// <summary>
        ///     DockPaneState Property
        /// </summary>
        public static DependencyProperty DockPaneStateProperty = DependencyProperty.Register(
            "DockPaneState",
            typeof(DockPaneState),
            typeof(DockPane),
            new PropertyMetadata(OnDockPaneStateChanged));

        /// <summary>
        ///     Icon Property
        /// </summary>
        public static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(string),
            typeof(DockPane));

        #endregion

        #region Fields

        private ICommand CloseAllTabCommand;

        /// <summary>
        ///     Close command
        /// </summary>
        private ICommand CloseCommand;

        /// <summary>
        ///     Condenced Dock Pane
        /// </summary>
        private FrameworkElement CondencedDockPaneInstance;

        /// <summary>
        ///     Dock pane header
        /// </summary>
        private UIElement DockPaneHeader;

        /// <summary>
        ///     Pin toggle command
        /// </summary>
        private ICommand TogglePinCommand;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="DockPane" /> class.
        /// </summary>
        static DockPane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPane), new FrameworkPropertyMetadata(typeof(DockPane)));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockPane" /> class.
        /// </summary>
        public DockPane()
        {
            this.CreateCommands();
            this.Loaded += this.InitializeDockPane;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when close button is clicked
        /// </summary>
        public event RoutedEventHandler Close
        {
            add
            {
                this.AddHandler(CloseEvent, value);
            }
            remove
            {
                this.RemoveHandler(CloseEvent, value);
            }
        }

        /// <summary>
        ///     Occurs when header is dragged
        /// </summary>
        public event MouseButtonEventHandler HeaderDrag
        {
            add
            {
                this.AddHandler(HeaderDragEvent, value);
            }
            remove
            {
                this.RemoveHandler(HeaderDragEvent, value);
            }
        }

        /// <summary>
        ///     Occurs when dock pane's pin is toggled
        /// </summary>
        public event RoutedEventHandler TogglePin
        {
            add
            {
                this.AddHandler(TogglePinEvent, value);
            }
            remove
            {
                this.RemoveHandler(TogglePinEvent, value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Condenced dock panel
        /// </summary>
        public FrameworkElement CondencedDockPanel
        {
            get
            {
                return this.CondencedDockPaneInstance
                       ?? (this.CondencedDockPaneInstance = this.CreateCondencedDockPane());
            }
        }

        /// <summary>
        ///     Condenced dock panel template
        /// </summary>
        public DataTemplate CondencedDockPanelTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(CondencedDockPanelTemplateProperty);
            }
            set
            {
                this.SetValue(CondencedDockPanelTemplateProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating dock pane state
        /// </summary>
        /// <value>State of dock pane</value>
        public DockPaneState DockPaneState
        {
            get
            {
                return (DockPaneState)this.GetValue(DockPaneStateProperty);
            }
            set
            {
                this.SetValue(DockPaneStateProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get
            {
                return this.GetValue(IconProperty) as string;
            }
            set
            {
                this.SetValue(IconProperty, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the dragged pane.
        /// </summary>
        /// <value>The dragged pane.</value>
        private static DockPane DraggedPane { get; set; }

        /// <summary>
        ///     Close button
        /// </summary>
        private Button CloseButton { get; set; }

        /// <summary>
        ///     Drag start point
        /// </summary>
        private Point DragStartPoint { get; set; }

        /// <summary>
        ///     Pin button
        /// </summary>
        private ToggleButton PinButton { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code
        ///     or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AttachToVisualTree();
        }

        #endregion

        #region Explicit Interface Methods

        void ICloseControl.CloseControl()
        {
            (this.Tag as DocumentContent).DocumentContainer.RemoveDocument((this.Tag as DocumentContent));
            if (CloseButton != null)
                CloseButton.Command.Execute(Tag);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when dock pane state has changed
        /// </summary>
        /// <param name="pane">Dock pane.</param>
        /// <param name="state">New state (may not be current yet)</param>
        private static void OnDockPaneStateChange(DockPane pane, DockPaneState state)
        {
            paneState = state;
            switch (state)
            {
                case DockPaneState.Docked:

                    pane.ClearAdornerLayer();

                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Visible;
                        pane.PinButton.IsChecked = false;
                    }
                    break;

                case DockPaneState.AutoHide:

                    pane.ClearAdornerLayer();

                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Visible;
                        pane.PinButton.IsChecked = true;
                    }
                    break;

                case DockPaneState.Floating:

                    pane.AddResizingAdorner();
                    pane.AttachPinButton();
                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Collapsed;
                    }
                    break;

                case DockPaneState.Content:

                    pane.ClearAdornerLayer();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        ///     Called when state is changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnDockPaneStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pane = d as DockPane;
            var state = (DockPaneState)e.NewValue;
            OnDockPaneStateChange(pane, state);
        }

        /// <summary>
        ///     Adds the resizing adorner to the dock pane
        /// </summary>
        private void AddResizingAdorner()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null)
            {
                layer.Add(new ResizingAdorner(this));
            }
        }

        /// <summary>
        ///     Attaches the close button
        /// </summary>
        private void AttachCloseButton()
        {
            if (this.CloseButton != null)
            {
                this.CloseButton.Command = null;
            }

            var closeButton = this.GetTemplateChild("PART_CLOSE") as Button;
            if (closeButton != null)
            {
                closeButton.Command = this.CloseCommand;
                this.CloseButton = closeButton;
            }
        }

        /// <summary>
        ///     Attaches the dock pane header
        /// </summary>
        private void AttachDockPaneHeader()
        {
            if (this.DockPaneHeader != null)
            {
                this.DockPaneHeader.MouseLeftButtonDown -= this.OnHeaderLeftMouseButtonDown;
                this.DockPaneHeader.MouseMove -= this.OnHeaderMouseMove;
            }

            this.DockPaneHeader = this.GetTemplateChild("PART_DOCK_PANE_HEADER") as UIElement;

            if (this.DockPaneHeader != null)
            {
                this.DockPaneHeader.MouseLeftButtonDown += this.OnHeaderLeftMouseButtonDown;
                this.DockPaneHeader.MouseMove += this.OnHeaderMouseMove;
            }
        }

        /// <summary>
        ///     Attaches the pin button
        /// </summary>
        private void AttachPinButton()
        {
            if (this.PinButton != null)
            {
                this.PinButton.Command = null;
            }

            var pinButton = this.GetTemplateChild("PART_PIN") as ToggleButton;
            if (pinButton != null)
            {
                pinButton.Command = this.TogglePinCommand;
                this.PinButton = pinButton;
                if (paneState == DockPaneState.Floating)
                    this.PinButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Attaches to visual tree to the template
        /// </summary>
        private void AttachToVisualTree()
        {
            this.AttachDockPaneHeader();
            this.AttachCloseButton();
            this.AttachPinButton();
        }

        /// <summary>
        ///     Creates the commands
        /// </summary>
        private void CreateCommands()
        {
            this.CloseAllTabCommand = new CommandBase(arg => this.RaiseEvent(new RoutedEventArgs(CloseEvent)));
            this.CloseCommand = new CommandBase(arg => this.RaiseEvent(new RoutedEventArgs(CloseEvent)));
            this.TogglePinCommand =
                new CommandBase(
                    arg =>
                    {
                        this.DockPaneState = this.PinButton.IsChecked.Value
                            ? DockPaneState.AutoHide
                            : DockPaneState.Docked;
                    });
        }

        /// <summary>
        ///     Creates the condenced dock pane
        /// </summary>
        private FrameworkElement CreateCondencedDockPane()
        {
            var content = new ContentControl();
            content.ContentTemplate = this.CondencedDockPanelTemplate;
            content.Content = new { this.Header, this.Icon };
            content.DataContext = this;
            return content;
        }

        private void Execute(object o)
        {
        }

        /// <summary>
        ///     Initializes the dock pane.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void InitializeDockPane(object sender, RoutedEventArgs e)
        {
            // Refresh state to ensure that proper controls (e.g. resizing adorners) are initialized since
            // before adorner layer was not present.
            //  OnDockPaneStateChange(this, this.DockPaneState);
        }

        /// <summary>
        ///     Called when left mouse button is down on header
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnHeaderLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DraggedPane = this;
            this.DragStartPoint = e.GetPosition(this);
        }

        /// <summary>
        ///     Called when mouse moves on the header
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private void OnHeaderMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton != MouseButtonState.Pressed) || ((DraggedPane != null) && (DraggedPane != this)))
            {
                return;
            }

            // Check for minimum distance in order to start drag
            Vector distance = e.GetPosition(this) - this.DragStartPoint;

            if ((Math.Abs(distance.X) < SystemParameters.MinimumHorizontalDragDistance)
                && (Math.Abs(distance.Y) < SystemParameters.MinimumVerticalDragDistance))
            {
                return;
            }

            RoutedEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
            args.RoutedEvent = HeaderDragEvent;
            this.RaiseEvent(args);
        }

        #endregion

        public static Windows.DockPaneState paneState { get; set; }
    }
}