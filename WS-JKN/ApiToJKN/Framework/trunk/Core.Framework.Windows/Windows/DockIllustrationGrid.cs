///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Windows
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Dock illustration grid
    /// </summary>
    public class DockIllustrationGrid : Grid
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="DockIllustrationGrid" /> class.
        /// </summary>
        static DockIllustrationGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DockIllustrationGrid),
                new FrameworkPropertyMetadata(typeof(DockIllustrationGrid)));
        }

        #endregion
    }
}