namespace Core.Framework.Windows.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    public class RequiredGrid : FormGrid
    {
        public RequiredGrid() : base()
        {

        }
        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(RequiredGrid),
            new UIPropertyMetadata(false));

        #endregion

        #region Public Properties

        public bool IsRequired
        {
            get
            {
                return (bool)this.GetValue(IsRequiredProperty);
            }
            set
            {
                this.SetValue(IsRequiredProperty, value);

                if (!value)
                {
                    IEnumerable<IValidateControl> children =
                        Manager.FindVisualChildren<FrameworkElement>(this).OfType<IValidateControl>();
                    foreach (IValidateControl validateControl in children)
                    {
                        validateControl.IsError = false;
                    }
                }
            }
        }

        #endregion
    }
}