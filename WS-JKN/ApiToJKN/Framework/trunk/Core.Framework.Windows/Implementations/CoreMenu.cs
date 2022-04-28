namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;

    public class CoreMenu : Menu
    {
        #region Constructors and Destructors

        public CoreMenu()
        {
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var style = Application.Current.Resources["StandardMenu"] as Style;
            this.Style = style;
        }

        #endregion
    }
}