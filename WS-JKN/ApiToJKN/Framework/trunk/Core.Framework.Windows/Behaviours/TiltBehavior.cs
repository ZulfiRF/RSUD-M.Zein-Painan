namespace Core.Framework.Windows.Behaviours
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using Core.Framework.Windows.Controls;

    public class TiltBehavior : Behavior<FrameworkElement>
    {
        #region Static Fields

        public static readonly DependencyProperty KeepDraggingProperty = DependencyProperty.Register(
            "KeepDragging",
            typeof(bool),
            typeof(TiltBehavior),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TiltFactorProperty = DependencyProperty.Register(
            "TiltFactor",
            typeof(Int32),
            typeof(TiltBehavior),
            new PropertyMetadata(20));

        #endregion

        #region Fields

        private bool IsPressed;

        private Thickness OriginalMargin;

        private Size OriginalSize;

        private FrameworkElement attachedElement;

        private Point current = new Point(-99, -99);

        private Panel originalPanel;

        private Int32 times = -1;

        #endregion

        #region Public Properties

        public bool KeepDragging
        {
            get
            {
                return (bool)this.GetValue(KeepDraggingProperty);
            }
            set
            {
                this.SetValue(KeepDraggingProperty, value);
            }
        }

        public Planerator RotatorParent { get; set; }

        public Int32 TiltFactor
        {
            get
            {
                return (Int32)this.GetValue(TiltFactorProperty);
            }
            set
            {
                this.SetValue(TiltFactorProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            this.attachedElement = this.AssociatedObject;
            if (this.attachedElement is ListBox)
            {
                var l = (ListBox)this.attachedElement;
                l.Items.CurrentChanging += (s, e) => { Console.WriteLine("foo"); };
                return;
            }
            if (this.attachedElement as Panel != null)
            {
                var y = (this.attachedElement as ItemsControl);
                y.Items.CurrentChanging += (s, e) => { Console.WriteLine("foo"); };
                (this.attachedElement as Panel).Loaded += (sl, el) =>
                {
                    var elements = new List<UIElement>();

                    foreach (UIElement ui in (this.attachedElement as Panel).Children)
                    {
                        elements.Add(ui);
                    }
                    elements.ForEach(
                        element =>
                            Interaction.GetBehaviors(element)
                                .Add(
                                    new TiltBehavior { KeepDragging = this.KeepDragging, TiltFactor = this.TiltFactor }));
                };

                return;
            }

            this.originalPanel = this.attachedElement.Parent as Panel ?? GetParentPanel(this.attachedElement);

            this.OriginalMargin = this.attachedElement.Margin;
            this.OriginalSize = new Size(this.attachedElement.Width, this.attachedElement.Height);
            double left = Canvas.GetLeft(this.attachedElement);
            double right = Canvas.GetRight(this.attachedElement);
            double top = Canvas.GetTop(this.attachedElement);
            double bottom = Canvas.GetBottom(this.attachedElement);
            int z = Panel.GetZIndex(this.attachedElement);
            VerticalAlignment va = this.attachedElement.VerticalAlignment;
            HorizontalAlignment ha = this.attachedElement.HorizontalAlignment;

            this.RotatorParent = new Planerator
                                 {
                                     Margin = this.OriginalMargin,
                                     Width = this.OriginalSize.Width,
                                     Height = this.OriginalSize.Height,
                                     VerticalAlignment = va,
                                     HorizontalAlignment = ha
                                 };

            this.RotatorParent.SetValue(Canvas.LeftProperty, left);
            this.RotatorParent.SetValue(Canvas.RightProperty, right);
            this.RotatorParent.SetValue(Canvas.TopProperty, top);
            this.RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            this.RotatorParent.SetValue(Panel.ZIndexProperty, z);

            this.originalPanel.Children.Remove(this.attachedElement);
            this.attachedElement.Margin = new Thickness();
            this.attachedElement.Width = double.NaN;
            this.attachedElement.Height = double.NaN;

            this.originalPanel.Children.Add(this.RotatorParent);
            this.RotatorParent.Child = this.attachedElement;

            CompositionTarget.Rendering += this.CompositionTargetRendering;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            CompositionTarget.Rendering -= this.CompositionTargetRendering;
        }

        private static Panel GetParentPanel(DependencyObject element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent is Panel)
            {
                return (Panel)parent;
            }
            return parent == null ? null : GetParentPanel(parent);
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.KeepDragging)
            {
                this.current = Mouse.GetPosition(this.RotatorParent.Child);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (this.current.X > 0 && this.current.X < (this.attachedElement).ActualWidth && this.current.Y > 0
                        && this.current.Y < (this.attachedElement).ActualHeight)
                    {
                        this.RotatorParent.RotationY = -1 * this.TiltFactor
                                                       + this.current.X * 2 * this.TiltFactor
                                                       / (this.attachedElement).ActualWidth;
                        this.RotatorParent.RotationX = -1 * this.TiltFactor
                                                       + this.current.Y * 2 * this.TiltFactor
                                                       / (this.attachedElement).ActualHeight;
                    }
                }
                else
                {
                    this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0
                        ? 0
                        : this.RotatorParent.RotationY - 5;
                    this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0
                        ? 0
                        : this.RotatorParent.RotationX - 5;
                }
            }
            else
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (!this.IsPressed)
                    {
                        this.current = Mouse.GetPosition(this.RotatorParent.Child);
                        if (this.current.X > 0 && this.current.X < (this.attachedElement).ActualWidth
                            && this.current.Y > 0 && this.current.Y < (this.attachedElement).ActualHeight)
                        {
                            this.RotatorParent.RotationY = -1 * this.TiltFactor
                                                           + this.current.X * 2 * this.TiltFactor
                                                           / (this.attachedElement).ActualWidth;
                            this.RotatorParent.RotationX = -1 * this.TiltFactor
                                                           + this.current.Y * 2 * this.TiltFactor
                                                           / (this.attachedElement).ActualHeight;
                        }
                        this.IsPressed = true;
                    }

                    if (this.IsPressed && this.times == 7)
                    {
                        this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0
                            ? 0
                            : this.RotatorParent.RotationY - 5;
                        this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0
                            ? 0
                            : this.RotatorParent.RotationX - 5;
                    }
                    else if (this.IsPressed && this.times < 7)
                    {
                        this.times++;
                    }
                }
                else
                {
                    this.IsPressed = false;
                    this.times = -1;
                    this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0
                        ? 0
                        : this.RotatorParent.RotationY - 5;
                    this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0
                        ? 0
                        : this.RotatorParent.RotationX - 5;
                }
            }
        }

        #endregion
    }
}