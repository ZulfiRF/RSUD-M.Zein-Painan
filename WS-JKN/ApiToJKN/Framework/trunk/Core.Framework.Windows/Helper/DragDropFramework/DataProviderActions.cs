using System;

namespace Core.Framework.Windows.Helper.DragDropFramework
{
    [Flags]
    public enum DataProviderActions
    {
        QueryContinueDrag = 0x01, // Call IDataProvider.DragSource_QueryContinueDrag

        GiveFeedback = 0x02, // Call IDataProvider.DragSource_GiveFeedback

        DoDragDropDone = 0x04, // Call IDataProvider.DoDragDrop_Done

        None = 0x00,
    }
}