namespace Core.Framework.Windows.Implementations
{
    using System.Windows.Controls;
    using Core.Framework.Windows.Helper;

    public class CoreTabControl : TabControl
    {
        #region Methods

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is TabItem)
            {
                if ((this.SelectedItem as TabItem).Tag is string)
                {
                    Manager.ExecuteGenericModule((this.SelectedItem as TabItem).Tag.ToString());
                }
            }

            base.OnSelectionChanged(e);
        }

        #endregion
    }
}