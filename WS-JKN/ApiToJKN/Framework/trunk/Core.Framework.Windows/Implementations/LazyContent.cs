using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Helper;
using Core.Framework.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public class LazyContent : TransitioningContentControl
    {
        private static object _objLock = new object();
        public LazyContent()
        {
            Loaded += OnLoaded;
            HorizontalAlignment = HorizontalAlignment.Left;
        }


        protected GridViewColumn CurrentListView
        {
            get { return (GridViewColumn)GetValue(CurrentListViewProperty); }
            set { SetValue(CurrentListViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentListView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentListViewProperty =
            DependencyProperty.Register("CurrentListView", typeof(GridViewColumn), typeof(LazyContent), new UIPropertyMetadata(null));

        
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var viewbox = new Viewbox();
            viewbox.Stretch = Stretch.Fill;
            viewbox.VerticalAlignment = VerticalAlignment.Center;
            viewbox.HorizontalAlignment = HorizontalAlignment.Left;
            viewbox.Height = 24;
            viewbox.MinWidth = 24;
            viewbox.Child = new ProgressRing()
            {
                IsActive = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 60,
                VerticalAlignment = VerticalAlignment.Stretch,
                MinWidth = 60
            };
            Content = viewbox;
            ThreadPool.QueueUserWorkItem(CallBack, new[] { this, this.DataContext, Display });
        }

        private void CallBack(object state)
        {
            var arrObject = state as object[];
            if (arrObject != null)
            {
                lock (_objLock)
                {                   
                    Dispatcher.Invoke((ThreadStart)delegate
                    {
                        var result = HelperManager.BindPengambilanObjekDariSource(arrObject[1],
                       arrObject[2].ToString());
                        (arrObject[0] as LazyContent).Content = new TextBlock()
                        {
                            Text = result == null ? "" : result.ToString()
                        };

                    }, DispatcherPriority.Render);                    
                }
            }
        }

        public string Display
        {
            get { return (string)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Display.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayProperty =
            DependencyProperty.Register("Display", typeof(string), typeof(LazyContent), new UIPropertyMetadata(null));


    }
}