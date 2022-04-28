namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    [TemplatePart(Name = "PART_Scroll", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Headers", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_Mediator", Type = typeof(ScrollViewerOffsetMediator))]
    public class Pivot : ItemsControl
    {
        #region Static Fields

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(Pivot),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(Pivot));

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(Pivot),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemChanged));

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Pivot));

        #endregion

        #region Fields

        internal int internalIndex;

        private ListView headers;

        private ScrollViewerOffsetMediator mediator;

        private ScrollViewer scroller;

        private PivotItem selectedItem;

        #endregion

        #region Constructors and Destructors

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        #endregion

        #region Public Events

        public event RoutedEventHandler SelectionChanged
        {
            add
            {
                this.AddHandler(SelectionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectionChangedEvent, value);
            }
        }

        #endregion

        #region Public Properties

        public string Header
        {
            get
            {
                return (string)this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HeaderTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return (int)this.GetValue(SelectedIndexProperty);
            }
            set
            {
                this.SetValue(SelectedIndexProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void GoToItem(PivotItem item)
        {
            if (item == null || item == this.selectedItem)
            {
                return;
            }

            double widthToScroll = 0.0;
            int index;
            for (index = 0; index < this.Items.Count; index++)
            {
                if (this.Items[index] == item)
                {
                    this.internalIndex = index;
                    break;
                }
                widthToScroll += ((PivotItem)this.Items[index]).ActualWidth;
            }

            this.mediator.HorizontalOffset = this.scroller.HorizontalOffset;
            var sb = this.mediator.Resources["Storyboard1"] as Storyboard;
            var frame = (EasingDoubleKeyFrame)this.mediator.FindName("edkf");
            frame.Value = widthToScroll;
            sb.Completed -= this.sb_Completed;
            sb.Completed += this.sb_Completed;
            sb.Begin();

            this.RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.scroller = (ScrollViewer)this.GetTemplateChild("PART_Scroll");
            this.headers = (ListView)this.GetTemplateChild("PART_Headers");
            this.mediator = this.GetTemplateChild("PART_Mediator") as ScrollViewerOffsetMediator;

            if (this.scroller != null)
            {
                this.scroller.ScrollChanged += this.scroller_ScrollChanged;
                this.scroller.PreviewMouseWheel += this.scroller_MouseWheel;
            }
            if (this.headers != null)
            {
                this.headers.SelectionChanged += this.headers_SelectionChanged;
            }
        }

        #endregion

        #region Methods

        private static void SelectedItemChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var p = (Pivot)dependencyObject;
            if (p == null)
            {
                return;
            }

            if (p.internalIndex != p.SelectedIndex)
            {
                p.GoToItem((PivotItem)p.Items[(int)dependencyPropertyChangedEventArgs.NewValue]);
            }
        }

        private void headers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GoToItem((PivotItem)this.headers.SelectedItem);
        }

        private void sb_Completed(object sender, EventArgs e)
        {
            this.SelectedIndex = this.internalIndex;
        }

        private void scroller_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.scroller.ScrollToHorizontalOffset(this.scroller.HorizontalOffset + -e.Delta);
        }

        private void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double position = 0.0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                var pivotItem = ((PivotItem)this.Items[i]);
                double widthOfItem = pivotItem.ActualWidth;
                if (e.HorizontalOffset <= (position + widthOfItem - 1))
                {
                    this.selectedItem = pivotItem;
                    if (this.headers.SelectedItem != this.selectedItem)
                    {
                        this.headers.SelectedItem = this.selectedItem;
                        this.internalIndex = i;
                        this.SelectedIndex = i;

                        this.RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
                    }
                    break;
                }
                position += widthOfItem;
            }
        }

        #endregion
    }
}