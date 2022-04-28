namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Animation;

    public class CoreToogleButton : ToggleButton
    {
        #region Constructors and Destructors

        public CoreToogleButton()
        {
            this.Checked += this.OnChecked;
            this.Unchecked += this.OnUnchecked;
        }

        #endregion

        #region Methods

        private void OnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            var storyboard = this.Resources["Storyboard1"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs routedEventArgs)
        {
            var panel = this.GetTemplateChild("grid") as Panel;
            if (panel != null)
            {
                var storyboard = panel.Resources["Storyboard2"] as Storyboard;
                if (storyboard != null)
                {
                    storyboard.Begin();
                }
            }
        }

        #endregion
    }
}