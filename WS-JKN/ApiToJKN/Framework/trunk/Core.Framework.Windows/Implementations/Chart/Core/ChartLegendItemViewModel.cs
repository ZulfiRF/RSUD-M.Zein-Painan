using System.Windows.Media;

namespace Core.Framework.Windows.Implementations.Chart.Core
{
    #if NETFX_CORE

    using Windows.UI.Xaml.Media;

#else

#endif

    /// <summary>
    /// we cannot use the ChartSeries directly because we will join the data to internal Chartseries
    /// </summary>
    public class ChartLegendItemViewModel
    {
        public string Caption { get; set; }

        public Brush ItemBrush { get; set; }
    }
}
