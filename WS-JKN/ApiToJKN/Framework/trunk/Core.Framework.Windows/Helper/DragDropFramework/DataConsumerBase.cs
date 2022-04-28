#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFramework
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    ///     This class provides some default implementations for
    ///     IDataConsumer that can be used by derived classes.
    ///     This class represents dragged data that can be consumed.
    /// </summary>
    public abstract class DataConsumerBase : IDataConsumer
    {
        #region Fields

        /// <summary>
        ///     A list of formats this data object consumer supports
        /// </summary>
        private readonly string[] _dataFormats;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Create a Data Consumer that supports
        ///     the specified data formats
        /// </summary>
        /// <param name="dataFormats">Data formats supported by this data consumer</param>
        public DataConsumerBase(string[] dataFormats)
        {
            this._dataFormats = dataFormats;
            Debug.Assert((dataFormats != null) && (dataFormats.Length > 0), "Must have at least one format string");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Returns the actions supported by this data object consumer
        /// </summary>
        public abstract DataConsumerActions DataConsumerActions { get; //{
            //    return
            //        DataConsumerActions.DragEnter |
            //        DataConsumerActions.DragOver |
            //        DataConsumerActions.Drop |
            //        DataConsumerActions.DragLeave |
            //        DataConsumerActions.None;
            //}
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Occurs when mouse enters the area occupied
        ///     by the dropTarget (specified in the constructor).
        ///     Provide your own method if you wish; making sure
        ///     to define DragEnter in DataConsumerActions.
        ///     See DropTarget_DragEnter in DropManager for additional comments.
        /// </summary>
        /// <param name="sender">Drag event <code>sender</code></param>
        /// <param name="e">DragEnter event arguments</param>
        public virtual void DropTargetDragEnter(object sender, DragEventArgs e)
        {
            throw new NotImplementedException("DragEnter not implemented");
        }

        /// <summary>
        ///     Occurs when mouse leaves the area occupied
        ///     by the dropTarget (specified in the constructor).
        ///     Provide your own method if you wish; making sure
        ///     to define DragEnter in DataConsumerActions.
        ///     See DropTarget_DragLeave in DropManager for additional comments.
        /// </summary>
        /// <param name="sender">Drag event <code>sender</code></param>
        /// <param name="e">DragLeave event arguments</param>
        public virtual void DropTargetDragLeave(object sender, DragEventArgs e)
        {
            throw new NotImplementedException("DragLeave not implemented");
        }

        /// <summary>
        ///     Occurs when mouse is over the area occupied
        ///     by the dropTarget (specified in the constructor).
        ///     You must likely will provide your own method; make sure
        ///     to define DragOver in DataConsumerActions.
        /// </summary>
        /// <param name="sender">Drag event <code>sender</code></param>
        /// <param name="e">DragOver event arguments</param>
        public virtual void DropTargetDragOver(object sender, DragEventArgs e)
        {
            throw new NotImplementedException("DragOver not implemented");
        }

        /// <summary>
        ///     Occurs when the left mouse button is released in the area
        ///     occupied by the dropTarget (specified in the constructor).
        ///     You must likely will provide your own method; make sure
        ///     to define Drop in DataConsumerActions.
        ///     See DropTarget_DragEnter in DropManager for additional comments.
        /// </summary>
        /// <param name="sender">Drag event <code>sender</code></param>
        /// <param name="e">Drop event arguments</param>
        public virtual void DropTargetDrop(object sender, DragEventArgs e)
        {
            throw new NotImplementedException("Drop not implemented");
        }

        /// <summary>
        ///     Search the available data formats for a
        ///     supported data format and return the first match
        /// </summary>
        /// <param name="e">DragEventArgs from one of the four Drag events</param>
        /// <returns>Returns first available/supported data object match; null when no match is found</returns>
        public virtual object GetData(DragEventArgs e)
        {
            object data = null;
            string[] dataFormats = e.Data.GetFormats();
            foreach (string dataFormat in dataFormats)
            {
                foreach (string dataFormatString in this._dataFormats)
                {
                    if (dataFormat.Equals(dataFormatString))
                    {
                        try
                        {
                            data = e.Data.GetData(dataFormat);
                        }
                        catch /*(COMException e2)*/
                        {
                            ;
                        }
                    }
                    if (data != null)
                    {
                        return data;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}