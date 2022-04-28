#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This data consumer looks for a ListBoxItem
    ///     in a ListBox container.
    ///     The ListBoxItem is added as the target's child,
    ///     or it is inserted before the target if the Shift
    ///     key is pressed.  If the ListBoxItem is dropped
    ///     in empty space, it is added to the end of the
    ///     TreeView's items.
    /// </summary>
    /// <typeparam name="TSourceContainer">Drag data source container type</typeparam>
    /// <typeparam name="TSourceObject">Drag data source object type</typeparam>
    public class ListBoxItemToTreeViewItem<TSourceContainer, TSourceObject> : DataConsumerBase, IDataConsumer
        where TSourceContainer : ListBox where TSourceObject : ListBoxItem
    {
        #region Constructors and Destructors

        public ListBoxItemToTreeViewItem(string[] dataFormats)
            : base(dataFormats)
        {
        }

        #endregion

        #region Public Properties

        public override DataConsumerActions DataConsumerActions
        {
            get
            {
                return DataConsumerActions.DragEnter | DataConsumerActions.DragOver | DataConsumerActions.Drop
                       | //DragDropDataConsumerActions.DragLeave |
                       DataConsumerActions.None;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void DropTargetDragEnter(object sender, DragEventArgs e)
        {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTargetDragOver(object sender, DragEventArgs e)
        {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTargetDrop(object sender, DragEventArgs e)
        {
            this.DragOverOrDrop(true, sender, e);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     First determine whether the drag data is supported.
        ///     Finally handle the actual drop when <code>bDrop</code> is true.
        ///     Add the item as the drop target's child when Shift is not pressed,
        ///     or insert the item before the drop target when Shift is pressed.
        ///     When there is no drop target (dropped on empty space),
        ///     add to the end of the items.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as ListBoxDataProvider<TSourceContainer, TSourceObject>;
            if (dataProvider != null)
            {
                var dragSourceContainer = dataProvider.SourceContainer as TSourceContainer;
                var dragSourceObject = dataProvider.SourceObject as TSourceObject;
                Debug.Assert(dragSourceContainer != null);
                Debug.Assert(dragSourceObject != null);

                var dropContainer = Utility.FindParentControlIncludingMe<ItemsControl>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                var dropTarget = e.Source as TreeViewItem;

                TreeViewItem newTvi = null;
                if (bDrop)
                {
                    dataProvider.Unparent();
                    newTvi = new TreeViewItem();
                    newTvi.Header = dragSourceObject.Content;
                }

                if (dropTarget == null)
                {
                    if (bDrop)
                    {
                        dropContainer.Items.Add(newTvi);
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else
                {
                    if ((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0)
                    {
                        // As sibling
                        if (bDrop)
                        {
                            var shiftDropTarget = Utility.FindParentControlIncludingMe<ItemsControl>(dropTarget);
                            Debug.Assert(shiftDropTarget != null);
                            shiftDropTarget.Items.Insert(shiftDropTarget.Items.IndexOf(dropTarget), newTvi);
                        }
                        e.Effects = DragDropEffects.Link;
                        e.Handled = true;
                    }
                    else
                    {
                        // As child
                        if (bDrop)
                        {
                            dropTarget.Items.Add(newTvi);
                        }
                        e.Effects = DragDropEffects.Move;
                        e.Handled = true;
                    }
                }

                if (bDrop)
                {
                    newTvi.IsSelected = true;
                    newTvi.BringIntoView();
                }
            }
        }

        #endregion
    }
}