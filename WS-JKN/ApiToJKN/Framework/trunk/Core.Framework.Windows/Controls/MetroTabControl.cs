namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Contracts;

    public class MetroTabControl : TabControl
    {
        #region Static Fields

        public static readonly DependencyProperty TabStripMarginProperty = DependencyProperty.Register(
            "TabStripMargin",
            typeof(Thickness),
            typeof(MetroTabControl),
            new PropertyMetadata(new Thickness(0)));

        #endregion

        #region Constructors and Destructors

        public MetroTabControl()
        {
            this.DefaultStyleKey = typeof(MetroTabControl);
        }

        #endregion

        #region Public Properties

        public Thickness TabStripMargin
        {
            get
            {
                return (Thickness)this.GetValue(TabStripMarginProperty);
            }
            set
            {
                this.SetValue(TabStripMarginProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is IControlToUseGenericCalling)
            {
                (this.SelectedItem as IControlToUseGenericCalling).ExecuteControl();
            }
            base.OnSelectionChanged(e);
        }

        #endregion

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
    }
}