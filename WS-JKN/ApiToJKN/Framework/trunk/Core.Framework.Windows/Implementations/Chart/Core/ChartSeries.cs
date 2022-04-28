using System.Windows.Controls;
using System.Windows;

namespace Core.Framework.Windows.Implementations.Chart.Core
{
    #if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Data;

#else

#endif

    public class ChartSeries : ItemsControl
    { 
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register("DisplayMember",
            typeof(string),
            typeof(ChartSeries),
            new PropertyMetadata(null));
        public static readonly DependencyProperty ValueMemberProperty =
            DependencyProperty.Register("ValueMember",
            typeof(string),
            typeof(ChartSeries),
            new PropertyMetadata(null));
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption",
            typeof(string),
            typeof(ChartSeries),
            new PropertyMetadata("WpfSimpleChart"));
        
        public ChartSeries()
        {   
        }

        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public string DisplayMember
        {
            get
            {
                return (string)GetValue(DisplayMemberProperty);
            }
            set
            {
                SetValue(DisplayMemberProperty, value);
            }
        }

        public string ValueMember
        {
            get
            {
                return (string)GetValue(ValueMemberProperty);
            }
            set
            {
                SetValue(ValueMemberProperty, value);
            }
        }
    }
}
