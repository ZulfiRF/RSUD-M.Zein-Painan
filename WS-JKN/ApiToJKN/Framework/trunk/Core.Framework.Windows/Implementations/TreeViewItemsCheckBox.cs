using System;
using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{    
    public class TreeViewItemsCheckBox : TreeViewItem
    {
        public TreeViewItemsCheckBox()
        {
            var rDictionary = new ResourceDictionary
            {
                Source = new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/TreeViewItemsCheckBox.xaml"),
                    UriKind.Relative)
            };

            Style = rDictionary["TreeViewItems"] as Style;
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(TreeViewItemsCheckBox), new UIPropertyMetadata(false));

        
    }
}
