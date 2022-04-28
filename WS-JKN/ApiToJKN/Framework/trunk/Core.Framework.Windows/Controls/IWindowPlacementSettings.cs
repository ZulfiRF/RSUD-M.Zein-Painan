using Core.Framework.Windows.Native;

namespace Core.Framework.Windows.Controls
{
    public interface IWindowPlacementSettings
    {
        #region Public Properties

        WINDOWPLACEMENT? Placement { get; set; }

        #endregion

        #region Public Methods and Operators

        void Reload();

        void Save();

        #endregion
    }
}