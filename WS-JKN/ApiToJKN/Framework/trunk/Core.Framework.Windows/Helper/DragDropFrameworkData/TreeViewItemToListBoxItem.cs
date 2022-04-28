#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This data consumer looks for TreeViewItems.
    ///     The item is inserted before the
    ///     target ListBoxItem or at the end of the
    ///     list if dropped on empty space.
    ///     Note that only TreeViewItems with no children can be moved.
    /// </summary>
    /// <typeparam name="TSourceContainer">Drag data source container type</typeparam>
    /// <typeparam name="TSourceObject">Drag data source object type</typeparam>
    public class TreeViewItemToListBoxItem<TSourceContainer, TSourceObject> : DataConsumerBase, IDataConsumer
        where TSourceContainer : ItemsControl where TSourceObject : TreeViewItem
    {
        #region Constructors and Destructors

        public TreeViewItemToListBoxItem(string[] dataFormats)
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
        ///     Insert the item before the drop target.  When there is no drop
        ///     target (dropped on empty space), add to the end of the items.
        ///     Note that only TreeViewItems with no children can be moved.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as TreeViewDataProvider<TSourceContainer, TSourceObject>;
            if (dataProvider != null)
            {
                var dragSourceObject = dataProvider.SourceObject as TSourceObject;
                var dragSourceContainer = dataProvider.SourceContainer as TSourceContainer;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                var dropContainer = Utility.FindParentControlIncludingMe<ListBox>(e.Source as DependencyObject);
                var dropTarget = Utility.FindParentControlIncludingMe<ListBoxItem>(e.Source as DependencyObject);

                if (!dragSourceObject.HasItems)
                {
                    // TreeViewItem must be a leaf
                    if (bDrop)
                    {
                        dataProvider.Unparent();

                        var item = new ListBoxItem();
                        item.Content = dragSourceObject.Header;
                        item.ToolTip = dragSourceObject.ToolTip;
                        if (dropTarget == null)
                        {
                            dropContainer.Items.Add(item);
                        }
                        else
                        {
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), item);
                        }

                        item.IsSelected = true;
                        item.BringIntoView();
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}