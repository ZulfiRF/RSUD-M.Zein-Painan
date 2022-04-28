namespace Core.Framework.Windows.Datagrid.Views
{
    internal enum UpdateMeasureTriggeredAction
    {
        Unspecified,
        ColumnActualWidthChanged,
        ColumnReordering, // FixedColumns, Columns drag and drop
        CurrentItemChanged,
        GroupingChanged,
        ScrollViewerChanged,
        SortingChanged,
        VirtualizationStateChanged, // On / Off Virtualization
        ViewPortWidthChanged, // Parent viewport resized
    }
}