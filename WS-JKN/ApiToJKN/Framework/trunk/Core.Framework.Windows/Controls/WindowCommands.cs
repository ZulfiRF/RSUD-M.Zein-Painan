namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class WindowCommands : ItemsControl
    {
        #region Constructors and Destructors

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WindowCommands),
                new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion
    }
}