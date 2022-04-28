using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core.Framework.Windows.Implementations
{
    public class CoreScrollViewer : ScrollViewer
    {
        #region Fields

        private bool autoHideScroll;

        private ScrollBarVisibility tempHorizontalScroll;

        private ScrollBarVisibility tempVerticalScroll;

        #endregion

        #region Constructors and Destructors

        public CoreScrollViewer()
        {
            this.Loaded += this.OnLoaded;
            this.MouseEnter += this.OnMouseEnter;
            this.MouseLeave += this.OnMouseLeave;
        }

        #endregion

        #region Public Properties

        public bool AutoHideScrollViewer
        {
            get
            {
                return this.autoHideScroll;
            }
            set
            {
                this.autoHideScroll = value;
                if (this.autoHideScroll)
                {
                    this.tempHorizontalScroll = this.HorizontalScrollBarVisibility;
                    this.tempVerticalScroll = this.VerticalScrollBarVisibility;
                }
                else
                {
                    this.HorizontalScrollBarVisibility = this.tempHorizontalScroll;
                    this.VerticalScrollBarVisibility = this.tempVerticalScroll;
                }
            }
        }

        #endregion

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        private void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this.AutoHideScrollViewer)
            {
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this.AutoHideScrollViewer)
            {
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        #endregion

        public bool IsFreeze { get; set; }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!IsFreeze)
                base.OnMouseWheel(e);
        }
    }
}