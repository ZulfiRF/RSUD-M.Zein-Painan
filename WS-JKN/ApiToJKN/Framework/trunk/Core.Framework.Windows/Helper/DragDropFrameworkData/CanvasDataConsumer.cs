using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Helper.DragDropFramework;

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    /// <summary>
    ///     This data consumer looks for drag data coming from
    ///     a canvas (of type TContainer) and
    ///     a drag source data object of type TObject.
    ///     When dropped, it moves the data object to the
    ///     mouse drop location.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class CanvasDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : Canvas
        where TObject : UIElement
    {
        #region Constructors and Destructors

        public CanvasDataConsumer(string[] dataFormats)
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
        ///     Second determine what operation to do (copy, move, link).
        ///     And finally handle the actual drop when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as CanvasDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                var dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);

                var dropContainer = sender as TContainer;

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();
                        dropContainer.Children.Add(dragSourceObject);

                        Point dropPosition = e.GetPosition(dropContainer);
                        Point objectOrigin = dataProvider.StartPosition;
                        if (dragSourceObject is IMoveObject)
                        {
                            ((IMoveObject)dragSourceObject).WhenDrop(dropPosition.X - objectOrigin.X, dropPosition.Y - objectOrigin.Y);
                        }
                        Canvas.SetLeft(dragSourceObject, dropPosition.X - objectOrigin.X);
                        Canvas.SetTop(dragSourceObject, dropPosition.Y - objectOrigin.Y);
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