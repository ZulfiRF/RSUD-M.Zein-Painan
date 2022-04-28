namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Windows;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This data consumer looks for all data formats specified in the constructor.
    ///     When dropped, erase (Unparent) the source object.
    /// </summary>
    public class TrashConsumer : DataConsumerBase, IDataConsumer
    {
        #region Constructors and Destructors

        public TrashConsumer(string[] dataFormats)
            : base(dataFormats)
        {
        }

        #endregion

        #region Public Properties

        public override DataConsumerActions DataConsumerActions
        {
            get
            {
                return //DragDropDataConsumerActions.DragEnter |
                    DataConsumerActions.DragOver | DataConsumerActions.Drop |//DragDropDataConsumerActions.DragLeave |
                    DataConsumerActions.None;
            }
        }

        #endregion

        #region Public Methods and Operators

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
        ///     Finally erase (Unparent) the source object when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider = this.GetData(e) as IDataProvider;
            if (dataProvider != null)
            {
                if (bDrop)
                {
                    dataProvider.Unparent();
                }
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        #endregion
    }
}