using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Helper.DragDropFramework;

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    /// <summary>
    ///     This data consumer looks for Buttons coming from a ToolBar.
    ///     When dropped, it either inserts the button (if drop target
    ///     is a button) or moves the button to the end of the ToolBar.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class ToolBarDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl where TObject : UIElement
    {
        #region Constructors and Destructors

        public ToolBarDataConsumer(string[] dataFormats)
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
        ///     Second determine what operation to do (move, link).
        ///     And finally handle the actual drop when <code>bDrop</code> is true.
        ///     Insert the button if the target is another button, otherwise
        ///     just add it to the end of the list.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as ToolBarDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                var dragSourceContainer = dataProvider.SourceContainer as TContainer;
                var dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                var dropContainer = sender as TContainer;
                var dropTarget = e.Source as TObject;
                if (dropTarget == null)
                {
                    dropTarget = Utility.FindParentControlIncludingMe<TObject>(e.Source as DependencyObject);
                }

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
                    }
                    e.Effects = (dropTarget == null) ? DragDropEffects.Move : DragDropEffects.Link;
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