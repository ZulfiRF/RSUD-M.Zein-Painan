namespace Core.Framework.Windows.Controls
{
    using System.Windows.Input;

    /// <summary>
    ///     The minimum specification that a tile needs to support
    /// </summary>
    public interface IPanoramaTile
    {
        #region Public Properties

        ICommand TileClickedCommand { get; }

        #endregion
    }
}