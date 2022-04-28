namespace Core.Framework.Windows.Behaviours
{
    using System.Windows.Interactivity;

    using Core.Framework.Windows.Controls;

    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        #region Methods

        protected override void OnAttached()
        {
            if (this.AssociatedObject != null && this.AssociatedObject.SaveWindowPosition)
            {
                // save with custom settings class or use the default way
                IWindowPlacementSettings windowPlacementSettings = this.AssociatedObject.WindowPlacementSettings
                                                                   ?? new WindowApplicationSettings(
                                                                       this.AssociatedObject);
                WindowSettings.SetSave(this.AssociatedObject, windowPlacementSettings);
            }
        }

        #endregion
    }
}