using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Ribbon.UI
{
    /// <summary>
    /// Interaction logic for ApplicationMenuItem.xaml
    /// </summary>
    public class ApplicationMenuItem : TabItem
    {

        #region Construction

        static ApplicationMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenuItem),
                new FrameworkPropertyMetadata(typeof(ApplicationMenuItem)));
        }



        public bool CanBinding
        {
            get { return (bool)GetValue(CanBindingProperty); }
            set { SetValue(CanBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBindingProperty =
            DependencyProperty.Register("CanBinding", typeof(bool), typeof(ApplicationMenuItem), new UIPropertyMetadata(true));


        public ApplicationMenuItem()
        {
            if (Application.Current != null)
                Style = Application.Current.Resources["ApplicationMenuItemStyle"] as Style;
        }
        #endregion
    }
}
