#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This data consumer looks for Buttons coming from a ToolBar.
    ///     When dropped, the button is moved from the ToolBar
    ///     to the Canvas.
    /// </summary>
    /// <typeparam name="TContainer">Drag data source container type</typeparam>
    /// <typeparam name="TObject">Drag data source object type</typeparam>
    public class ToolbarButtonToCanvasButton<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl where TObject : Button
    {
        #region Constructors and Destructors

        public ToolbarButtonToCanvasButton(string[] dataFormats)
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
        ///     Second determine whether or not a Move can be done.
        ///     And finally handle the actual drop when <code>bDrop</code> is true.
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
                if (dragSourceObject == null)
                {
                    dragSourceObject =
                        Utility.FindParentControlIncludingMe<TObject>(dataProvider.SourceObject as DependencyObject);
                }
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                var dropContainer = sender as Panel;

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();
                        Point containerPoint = e.GetPosition(dropContainer);
                        Point objectPoint = dataProvider.StartPosition;
#if REUSE_SAME_BUTTON //|| true
                        dropContainer.Children.Add(dragSourceObject);
                        Canvas.SetLeft(dragSourceObject, containerPoint.X - objectPoint.X);
                        Canvas.SetTop(dragSourceObject, containerPoint.Y - objectPoint.Y);
#else
                        Button oldButton = dragSourceObject;
                        var newButton = new Button();
                        newButton.Content = Utility.CloneElement(oldButton.Content);
                        newButton.ToolTip = oldButton.ToolTip;
                        dropContainer.Children.Add(newButton);
                        Canvas.SetLeft(newButton, containerPoint.X - objectPoint.X);
                        Canvas.SetTop(newButton, containerPoint.Y - objectPoint.Y);
#endif
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