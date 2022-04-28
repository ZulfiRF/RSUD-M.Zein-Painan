namespace Core.Framework.Windows.Datagrid
{
    internal enum SaveRestoreStateVisitorStatus
    {
        Ready = 0,
        Saving = 1,
        RestorePending = 2,
        Restoring = 3,
        Restored = 4,
        Error = 5
    }
}