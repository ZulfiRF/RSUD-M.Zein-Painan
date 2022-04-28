#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This data consumer looks for drag data coming from
    ///     a drag source container of type TContainer and
    ///     a drag source data object of type TObject.
    ///     It creates a new button using the contents of the
    ///     old button and adds the new button to the
    ///     drop target's container.
    /// </summary>
    /// <typeparam name="TContainer">Drag data source container type</typeparam>
    /// <typeparam name="TObject">Drag data source object type</typeparam>
    public class CanvasButtonToToolbarButton<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : Canvas where TObject : Button
    {
        #region Constructors and Destructors

        public CanvasButtonToToolbarButton(string[] dataFormats)
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
                       | //Ddf.DragDropDataConsumerActions.DragLeave |
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
        ///     Note that a new button needs to be created for the toolbar.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as CanvasDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                var dragSourceContainer = dataProvider.SourceContainer as TContainer;
                var dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                var dropContainer = sender as ItemsControl;
                var dropTarget = e.Source as TObject;
                if (dropTarget == null)
                {
                    dropTarget = Utility.FindParentControlExcludingMe<TObject>(e.Source as DependencyObject);
                }

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();
                        Button button;
#if REUSE_SAME_BUTTON //|| true
                        button = dragSourceObject as Button;
#else
                        Button oldButton = dragSourceObject;
                        button = new Button();
                        button.Content = Utility.CloneElement(oldButton.Content);
                        button.ToolTip = oldButton.ToolTip;
#endif
                        if (dropTarget == null)
                        {
                            dropContainer.Items.Add(button);
                        }
                        else
                        {
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), button);
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