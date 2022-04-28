using System;
using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public class TreeListView : TreeView
    {
        public GridViewColumnCollection ColumnCollection
        {
            get { return (GridViewColumnCollection)GetValue(ColumnCollectionProperty); }
            set { SetValue(ColumnCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCollectionProperty =
            DependencyProperty.Register("ColumnCollection", typeof(GridViewColumnCollection), typeof(TreeListView), new PropertyMetadata(new GridViewColumnCollection(), ColumnCollectionChanged));

        private static void ColumnCollectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }

        protected override DependencyObject 
                           GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool 
                           IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
        public TreeListView()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.TreeListView;component/Resources/TreeListView.xaml")
                });
            this.ColumnCollection = new GridViewColumnCollection();
        }
    }
}
