using System.Collections.Generic;

namespace Core.Framework.Helper.Collections
{
    public class TreeNode<T>
    {
        public readonly List<TreeNode<T>> Children;

        T Item { get; set; }

        public TreeNode(T item)
        {
            Item = item;
            Children = new List<TreeNode<T>>();
        }

        public TreeNode<T> AddChild(T item)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }

        public List<TreeNode<T>> GetChildren()
        {
            return Children;
        }
    }
}