namespace Core.Framework.Windows.Actions
{
    using System.Windows;
    using System.Windows.Interactivity;

    using Core.Framework.Windows.Controls;

    public class SetFlyoutOpenAction : TargetedTriggerAction<FrameworkElement>
    {
        #region Static Fields

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(bool),
            typeof(SetFlyoutOpenAction),
            new PropertyMetadata(default(bool)));

        #endregion

        #region Public Properties

        public bool Value
        {
            get
            {
                return (bool)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void Invoke(object parameter)
        {
            ((Flyout)this.TargetObject).IsOpen = this.Value;
        }

        #endregion
    }
}