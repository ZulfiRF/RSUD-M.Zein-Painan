using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Helper.DragDropFramework;

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    /// <summary>
    ///     This data consumer looks for ListBoxItems.
    ///     The ListBoxItem is inserted before the
    ///     target ListBoxItem or at the end of the
    ///     list if dropped on empty space.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class ListBoxDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl where TObject : FrameworkElement
    {
        #region Constructors and Destructors

        public ListBoxDataConsumer(string[] dataFormats)
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
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as ListBoxDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                var dragSourceContainer = dataProvider.SourceContainer as TContainer;
                var dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                var dropContainer = Utility.FindParentControlIncludingMe<TContainer>(e.Source as DependencyObject);
                var dropTarget = e.Source as TObject;

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();
                        if (dropTarget == null)
                        {
                            dropContainer.Items.Add(dragSourceObject);
                        }
                        else
                        {
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), dragSourceObject);
                        }

                        //dragSourceObject.IsSelected = true;
                        dragSourceObject.BringIntoView();
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