using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Helper.DragDropFramework;

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    /// <summary>
    ///     This data consumer looks for TreeViewItems.
    ///     The TreeViewItem is added as either a sibling or
    ///     a child, depending on the state of the Shift key.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class TreeViewDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl where TObject : ItemsControl
    {
        #region Constructors and Destructors

        public TreeViewDataConsumer(string[] dataFormats)
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
        ///     Note that the source object cannot be an ancestor of the drop target.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as TreeViewDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                var dragSourceContainer = dataProvider.SourceContainer as TContainer;
                var dragSourceObject = dataProvider.SourceObject as TreeViewItem;
                Debug.Assert(dragSourceContainer != null);
                Debug.Assert(dragSourceObject != null);

                var dropContainer = Utility.FindParentControlIncludingMe<TContainer>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                var dropTarget = e.Source as TObject;

                if (dropTarget == null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();
                        dropContainer.Items.Add(dragSourceObject);
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
#if PRINT2OUTPUT
                    Debug.WriteLine("  Move0");
#endif
                }
                else
                {
                    bool IsAncestor = dragSourceObject.IsAncestorOf(dropTarget);
                    if ((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0)
                    {
                        var shiftDropTarget = Utility.FindParentControlIncludingMe<ItemsControl>(dropTarget);
                        Debug.Assert(shiftDropTarget != null);
                        if (!IsAncestor)
                        {
                            if (bDrop)
                            {
                                dataProvider.Unparent();
                                Debug.Assert(shiftDropTarget != null);
                                shiftDropTarget.Items.Insert(
                                    shiftDropTarget.Items.IndexOf(dropTarget),
                                    dragSourceObject);
                            }
                            e.Effects = DragDropEffects.Link;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  Link1");
#endif
                        }
                        else
                        {
                            e.Effects = DragDropEffects.None;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  None1");
#endif
                        }
                    }
                    else
                    {
                        if (!IsAncestor && (dragSourceObject != dropTarget))
                        {
                            if (bDrop)
                            {
                                dataProvider.Unparent();
                                dropTarget.Items.Add(dragSourceObject);
                            }
                            e.Effects = DragDropEffects.Move;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  Move2");
#endif
                        }
                        else
                        {
                            e.Effects = DragDropEffects.None;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  None2");
#endif
                        }
                    }
                }
                if (bDrop && e.Handled && (e.Effects != DragDropEffects.None))
                {
                    dragSourceObject.IsSelected = true;
                    dragSourceObject.BringIntoView();
                }
            }
        }

        #endregion
    }
}