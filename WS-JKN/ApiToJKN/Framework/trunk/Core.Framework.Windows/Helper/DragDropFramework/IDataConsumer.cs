using System.Windows;

namespace Core.Framework.Windows.Helper.DragDropFramework
{
    /// <summary>
    ///     A declaration of actions that can be performed on dragged data
    /// </summary>
    public interface IDataConsumer
    {
        #region Public Properties

        DataConsumerActions DataConsumerActions { get; }

        #endregion

        #region Public Methods and Operators

        void DropTargetDragEnter(object sender, DragEventArgs e);

        void DropTargetDragLeave(object sender, DragEventArgs e);

        void DropTargetDragOver(object sender, DragEventArgs e);

        void DropTargetDrop(object sender, DragEventArgs e);

        #endregion
    }
}