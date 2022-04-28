using System;

namespace Core.Framework.Windows.Implementations
{
    internal interface ISelectionStrategy : IDisposable
    {
        event EventHandler<PreviewSelectionChangedEventArgs> PreviewSelectionChanged;

        void ApplyTemplate();
        bool SelectCore(MultiSelectTreeViewItem owner);
        bool Deselect(MultiSelectTreeViewItem item, bool bringIntoView = false);
        bool SelectPreviousFromKey();
        bool SelectNextFromKey();
        bool SelectFirstFromKey();
        bool SelectLastFromKey();
        bool SelectPageUpFromKey();
        bool SelectPageDownFromKey();
        bool SelectAllFromKey();
        bool SelectParentFromKey();
        bool SelectCurrentBySpace();
        bool Select(MultiSelectTreeViewItem treeViewItem);
    }
}