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
#else

#endif

   
    public class PieChart : ChartBase
    {
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="PieChart"/> class.
        /// </summary>
        static PieChart()
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT

#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(typeof(PieChart)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart"/> class.
        /// </summary>
        public PieChart()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(PieChart);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(PieChart);
#endif
        }

        #endregion Constructors

        protected override double GridLinesMaxValue
        {
            get
            {
                return 0.0;
            }
        }
    }
}