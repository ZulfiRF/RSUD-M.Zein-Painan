namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class PivotItem : ContentControl
    {
        #region Static Fields

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(PivotItem),
            new PropertyMetadata(default(string)));

        #endregion

        #region Constructors and Destructors

        static PivotItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PivotItem),
                new FrameworkPropertyMetadata(typeof(PivotItem)));
        }

        public PivotItem()
        {
            this.RequestBringIntoView += (s, e) => { e.Handled = true; };
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

        #endregion
    }
}