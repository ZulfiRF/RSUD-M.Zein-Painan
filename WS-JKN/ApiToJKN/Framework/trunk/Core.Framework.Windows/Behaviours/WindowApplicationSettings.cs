using System.Configuration;
using System.Windows;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Native;

namespace Core.Framework.Windows.Behaviours
{
    internal class WindowApplicationSettings : ApplicationSettingsBase, IWindowPlacementSettings
    {
        #region Constructors and Destructors

        public WindowApplicationSettings(Window window)
            : base(window.GetType().FullName)
        {
        }

        #endregion

        #region Public Properties

        [UserScopedSetting]
        public WINDOWPLACEMENT? Placement
        {
            get
            {
                if (this["Placement"] != null)
                {
                    return ((WINDOWPLACEMENT)this["Placement"]);
                }
                return null;
            }
            set
            {
                this["Placement"] = value;
            }
        }

        #endregion
    }
}