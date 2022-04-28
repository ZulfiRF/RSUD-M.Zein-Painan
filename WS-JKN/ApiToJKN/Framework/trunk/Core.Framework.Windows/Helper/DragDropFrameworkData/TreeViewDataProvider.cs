#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This Data Provider represents TreeViewItems.
    ///     Note that a TreeViewItem's container can be
    ///     either a TreeView or another TreeViewItem.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class TreeViewDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl where TObject : ItemsControl
    {
        #region Constructors and Destructors

        public TreeViewDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        #endregion

        #region Public Properties

        public override DragDropEffects AllowedEffects
        {
            get
            {
                return //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move | // Move TreeItem
                    DragDropEffects.Link | // Move TreeItem as sibling
                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions
        {
            get
            {
                return DataProviderActions.QueryContinueDrag | // Need Shift key info
                       DataProviderActions.GiveFeedback | //DragDropDataProviderActions.DoDragDrop_Done |
                       DataProviderActions.None;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Move)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            else if (e.Effects == DragDropEffects.Link)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override void Unparent()
        {
            var item = this.SourceObject as TObject;
            // 'container' can be either TreeView or another TreeViewItem
            var container = Utility.FindParentControlIncludingMe<TContainer>(item);

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(container != null, "Unparent expects a non-null container");

            if ((container != null) && (item != null))
            {
                container.Items.Remove(item);
            }
        }

        #endregion
    }
}