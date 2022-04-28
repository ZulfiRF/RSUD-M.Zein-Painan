using System.Windows.Controls;
using Core.Framework.Helper.Contracts;


namespace Core.Framework.Windows.Implementations
{
    public class CoreTreeViewItem : TreeViewItem
    {
        public bool IsLazyLoad { get; set; }
        public CoreTreeViewItem()
        {

        }

    }
}