///
/// Copyright(C) MixModes Inc. 2010
/// 

using Core.Framework.Windows.Converters;

namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    [TemplatePart(Name = "PART_TITLEBAR", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_MINIMIZE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MAXIMIZE_RESTORE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LEFT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_RIGHT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_TOP_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_BOTTOM_BORDER", Type = typeof(UIElement))]
    /// <summary>
    /// Custom Window
    /// </summary>
    public class CustomWindow : Window
    {
        // Private members
        #region Static Fields

        /// <summary>
        ///     Maximized property
        /// </summary>
        public static DependencyProperty MaximizedProperty = MaximizedPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey MaximizedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Maximized",
                typeof(bool),
                typeof(CustomWindow),
                new PropertyMetadata(false));

        #endregion

        #region Fields

        /// <summary>
        ///     Maximize / Restore command
        /// </summary>
        private readonly RoutedUICommand MaximizeRestoreCommand = new RoutedUICommand(
            "MaximizeRestore",
            "MaximizeRestore",
            typeof(CustomWindow));

        /// <summary>
        ///     Minimize Command
        /// </summary>
        private readonly RoutedUICommand MinimizeCommand = new RoutedUICommand(
            "Minimize",
            "Minimize",
            typeof(CustomWindow));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomWindow" /> class.
        /// </summary>
        public CustomWindow()
        {
            this.CreateCommandBindings();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CustomWindow" /> is maximized
        /// </summary>
        /// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
        public bool Maximized
        {
            get
            {
                return (bool)this.GetValue(MaximizedProperty);
            }
            private set
            {
                if (value)
                {
                    this.UpdateRestoreBounds();

                    // Maximize hides taskbar, hence workaround
                    this.Top = this.Left = 0;
                    this.Height = SystemParameters.MaximizedPrimaryScreenHeight - SystemParameters.BorderWidth * 2;
                    this.Width = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.BorderWidth * 2;
                }
                else
                {
                    this.ApplyRestoreBounds();
                }

                Visibility sizerVisibility = value ? Visibility.Hidden : Visibility.Visible;
                this.UpdateBorderVisibility(this.RightBorder, sizerVisibility);
                this.UpdateBorderVisibility(this.TopBorder, sizerVisibility);
                this.UpdateBorderVisibility(this.BottomBorder, sizerVisibility);
                this.UpdateBorderVisibility(this.LeftBorder, sizerVisibility);

                this.SetValue(MaximizedPropertyKey, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Bottom border
        /// </summary>
        private UIElement BottomBorder { get; set; }

        /// <summary>
        ///     Close button
        /// </summary>
        private Button CloseButton { get; set; }

        /// <summary>
        ///     Indicates whether window is currently resizing
        /// </summary>
        /// <value>
        ///     <c>true</c> If window is currently resizing; otherwise, <c>false</c>.
        /// </value>
        private bool IsResizing { get; set; }

        /// <summary>
        ///     Left border
        /// </summary>
        private UIElement LeftBorder { get; set; }

        /// <summary>
        ///     Maximize / restore button
        /// </summary>
        /// <value>The maximize restore button.</value>
        private Button MaximizeRestoreButton { get; set; }

        /// <summary>
        ///     Minimize button
        /// </summary>
        private Button MinimizeButton { get; set; }

        /// <summary>
        ///     Gets the size and location of a window before being either minimized or maximized.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     A <see cref="T:System.Windows.Rect" /> that specifies the size and location of a window before being either
        ///     minimized or maximized.
        /// </returns>
        private new Rect RestoreBounds { get; set; }

        /// <summary>
        ///     Right border
        /// </summary>
        private UIElement RightBorder { get; set; }

        /// <summary>
        ///     Title bar
        /// </summary>
        private UIElement TitleBar { get; set; }

        /// <summary>
        ///     Top border
        /// </summary>
        private UIElement TopBorder { get; set; }

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

        #region Methods

        /// <summary>
        ///     Applies the restore bounds to the window
        /// </summary>
        private void ApplyRestoreBounds()
        {
            this.Left = this.RestoreBounds.Left;
            this.Top = this.RestoreBounds.Top;
            this.Width = this.RestoreBounds.Width;
            this.Height = this.RestoreBounds.Height;
        }

        /// <summary>
        ///     Attaches the borders to the visual tree
        /// </summary>
        private void AttachBorders()
        {
            this.AttachLeftBorder();
            this.AttachRightBorder();
            this.AttachTopBorder();
            this.AttachBottomBorder();
        }

        /// <summary>
        ///     Attaches the bottom border to the visual tree
        /// </summary>
        private void AttachBottomBorder()
        {
            if (this.BottomBorder != null)
            {
                this.BottomBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                this.BottomBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                this.BottomBorder.MouseMove -= this.OnBottomBorderMouseMove;
            }

            var bottomBorder = this.GetChildControl<UIElement>("PART_BOTTOM_BORDER");
            if (bottomBorder != null)
            {
                this.BottomBorder = bottomBorder;
                bottomBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                bottomBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                bottomBorder.MouseMove += this.OnBottomBorderMouseMove;
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

            var closeButton = this.GetChildControl<Button>("PART_CLOSE");
            if (closeButton != null)
            {
                closeButton.Command = ApplicationCommands.Close;
                this.CloseButton = closeButton;
            }
        }

        /// <summary>
        ///     Attaches the left border to the visual tree
        /// </summary>
        private void AttachLeftBorder()
        {
            if (this.LeftBorder != null)
            {
                this.LeftBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                this.LeftBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                this.LeftBorder.MouseMove -= this.OnLeftBorderMouseMove;
            }

            var leftBorder = this.GetChildControl<UIElement>("PART_LEFT_BORDER");
            if (leftBorder != null)
            {
                this.LeftBorder = leftBorder;
                leftBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                leftBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                leftBorder.MouseMove += this.OnLeftBorderMouseMove;
            }
        }

        /// <summary>
        ///     Attaches the maximize restore button
        /// </summary>
        private void AttachMaximizeRestoreButton()
        {
            if (this.MaximizeRestoreButton != null)
            {
                this.MaximizeRestoreButton.Command = null;
            }

            var maximizeRestoreButton = this.GetChildControl<Button>("PART_MAXIMIZE_RESTORE");
            if (maximizeRestoreButton != null)
            {
                maximizeRestoreButton.Command = this.MaximizeRestoreCommand;
                this.MaximizeRestoreButton = maximizeRestoreButton;
            }
        }

        /// <summary>
        ///     Attaches the minimize button
        /// </summary>
        private void AttachMinimizeButton()
        {
            if (this.MinimizeButton != null)
            {
                this.MinimizeButton.Command = null;
            }

            var minimizeButton = this.GetChildControl<Button>("PART_MINIMIZE");
            if (minimizeButton != null)
            {
                minimizeButton.Command = this.MinimizeCommand;
                this.MinimizeButton = minimizeButton;
            }
        }

        /// <summary>
        ///     Attaches the right border to the visual tree
        /// </summary>
        private void AttachRightBorder()
        {
            if (this.RightBorder != null)
            {
                this.RightBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                this.RightBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                this.RightBorder.MouseMove -= this.OnRightBorderMouseMove;
            }

            var rightBorder = this.GetChildControl<UIElement>("PART_RIGHT_BORDER");
            if (rightBorder != null)
            {
                this.RightBorder = rightBorder;
                rightBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                rightBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                rightBorder.MouseMove += this.OnRightBorderMouseMove;
            }
        }

        /// <summary>
        ///     Attaches the title bar to visual tree
        /// </summary>
        private void AttachTitleBar()
        {
            if (this.TitleBar != null)
            {
                this.TitleBar.RemoveHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnTitlebarClick));
            }

            var titleBar = this.GetChildControl<UIElement>("PART_TITLEBAR");
            if (titleBar != null)
            {
                this.TitleBar = titleBar;
                titleBar.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnTitlebarClick));
            }
        }

        /// <summary>
        ///     Attaches to visual tree to the template
        /// </summary>
        private void AttachToVisualTree()
        {
            this.AttachCloseButton();
            this.AttachMinimizeButton();
            this.AttachMaximizeRestoreButton();
            this.AttachTitleBar();
            this.AttachBorders();
        }

        /// <summary>
        ///     Attaches the top border to the visual tree
        /// </summary>
        private void AttachTopBorder()
        {
            if (this.TopBorder != null)
            {
                this.TopBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                this.TopBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                this.TopBorder.MouseMove -= this.OnRightBorderMouseMove;
            }

            var topBorder = this.GetChildControl<UIElement>("PART_TOP_BORDER");
            if (topBorder != null)
            {
                this.TopBorder = topBorder;
                topBorder.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonDown));
                topBorder.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(this.OnBorderMouseLeftButtonUp));
                topBorder.MouseMove += this.OnTopBorderMouseMove;
            }
        }

        /// <summary>
        ///     Creates the command bindings
        /// </summary>
        private void CreateCommandBindings()
        {
            // Command binding for close button
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (a, b) => { this.Close(); }));

            // Command binding for minimize button
            this.CommandBindings.Add(
                new CommandBinding(this.MinimizeCommand, (a, b) => { this.WindowState = WindowState.Minimized; }));

            // Command binding for maximize / restore button
            this.CommandBindings.Add(
                new CommandBinding(this.MaximizeRestoreCommand, (a, b) => { this.Maximized = !this.Maximized; }));
        }

        /// <summary>
        ///     Gets the child control from the template
        /// </summary>
        /// <typeparam name="T">Type of control requested</typeparam>
        /// <param name="controlName">Name of the control</param>
        /// <returns>Control instance if there is one with the specified name; null otherwise</returns>
        private T GetChildControl<T>(string controlName) where T : DependencyObject
        {
            var control = this.GetTemplateChild(controlName) as T;
            return control;
        }

        /// <summary>
        ///     Called when mouse left button is down on a border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data</param>
        private void OnBorderMouseLeftButtonDown(object source, MouseButtonEventArgs args)
        {
            this.IsResizing = true;
        }

        /// <summary>
        ///     Called when mouse left button is up on a border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data</param>
        private void OnBorderMouseLeftButtonUp(object source, MouseButtonEventArgs args)
        {
            this.IsResizing = false;
            if (source is UIElement)
            {
                (source as UIElement).ReleaseMouseCapture();
            }
        }

        /// <summary>
        ///     Called when mouse moves over bottom border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data</param>
        private void OnBottomBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!this.BottomBorder.IsMouseCaptured) && this.IsResizing)
            {
                this.BottomBorder.CaptureMouse();
            }

            if (this.IsResizing)
            {
                double position = args.GetPosition(this).Y - this.ActualHeight;

                if (Math.Abs(position) < 10)
                {
                    return;
                }

                if (position > 0)
                {
                    this.Height = this.ActualHeight + position;
                }
                else if ((position < 0) && (this.ActualHeight + position > this.MinHeight))
                {
                    position = (this.ActualHeight + position < this.MinHeight)
                        ? this.MinHeight - this.ActualHeight
                        : position;
                    this.Height = this.ActualHeight + position;
                }
            }
        }

        /// <summary>
        ///     Called when mouse moves over left border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data</param>
        private void OnLeftBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!this.LeftBorder.IsMouseCaptured) && this.IsResizing)
            {
                this.LeftBorder.CaptureMouse();
            }

            if (this.IsResizing)
            {
                double position = args.GetPosition(this).X;

                if (Math.Abs(position) < 10)
                {
                    return;
                }

                if ((position > 0) && ((this.Width - position) > this.MinWidth) && (this.Width > position))
                {
                    this.Left += position;
                    this.Width -= position;
                }
                else if ((position < 0) && (this.Left > 0))
                {
                    position = (this.Left + position > 0) ? position : -1 * this.Left;
                    this.Width = this.ActualWidth - position;
                    this.Left += position;
                }
            }
        }

        /// <summary>
        ///     Called when mouse moves over right border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data</param>
        private void OnRightBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!this.RightBorder.IsMouseCaptured) && this.IsResizing)
            {
                this.RightBorder.CaptureMouse();
            }

            if (this.IsResizing)
            {
                double position = args.GetPosition(this).X;

                if (Math.Abs(position) < 10)
                {
                    return;
                }

                if (position > 0)
                {
                    this.Width = position;
                }
                else if ((position < 0) && (this.ActualWidth > this.MinWidth))
                {
                    position = (this.ActualWidth + position < this.MinWidth)
                        ? this.MinWidth - this.ActualWidth
                        : position;
                    this.Width = this.ActualWidth + position;
                }
            }
        }

        /// <summary>
        ///     Called when titlebar is clicked or double clicked
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseLeftButtonEventArgs" /> instance containing the event data</param>
        private void OnTitlebarClick(object source, MouseButtonEventArgs args)
        {
            switch (args.ClickCount)
            {
                case 1:
                    if (!this.Maximized)
                    {
                        this.DragMove();
                    }
                    break;
                case 2:
                    this.Maximized = !this.Maximized;
                    break;
            }
        }

        /// <summary>
        ///     Called when mouse moves over top border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data</param>
        private void OnTopBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!this.TopBorder.IsMouseCaptured) && this.IsResizing)
            {
                this.TopBorder.CaptureMouse();
            }

            if (this.IsResizing)
            {
                double position = args.GetPosition(this).Y;

                if (Math.Abs(position) < 10)
                {
                    return;
                }

                if (position < 0)
                {
                    position = this.Top + position < 0 ? -this.Top : position;
                    this.Top += position;
                    this.Height = this.ActualHeight - position;
                }
                else if ((position > 0) && (this.ActualHeight - position > this.MinHeight))
                {
                    position = (this.ActualHeight - position < this.MinHeight)
                        ? this.MinHeight - this.ActualHeight
                        : position;
                    this.Height = this.ActualHeight - position;
                    this.Top += position;
                }
            }
        }

        /// <summary>
        ///     Updates the border visibility.
        /// </summary>
        /// <param name="border">Border</param>
        /// <param name="visibility">Visibility</param>
        private void UpdateBorderVisibility(UIElement border, Visibility visibility)
        {
            if (border != null)
            {
                border.Visibility = visibility;
            }
        }

        /// <summary>
        ///     Updates the restore bounds
        /// </summary>
        private void UpdateRestoreBounds()
        {
            this.RestoreBounds = new Rect(
                new Point(this.Left, this.Top),
                new Point(this.Left + this.ActualWidth, this.Top + this.ActualHeight));
        }

        #endregion
    }
}