using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class TextWriteColumn : WriteDataGridColumn
    {
        public override FrameworkElement CreateElement(TextBlock textBlock, DataGridCell dataGridCell)
        {
            var binding = this.Binding as Binding;
            if (binding != null) textBlock.Tag = binding.Path.Path;
            var coreTextBox = new CoreTextBox();
            coreTextBox.Text = textBlock.Text;
            coreTextBox.LostFocus += delegate(object o, RoutedEventArgs args)
            {
                var text = o as CoreTextBox;
                if (text != null)
                {
                    DataGridOwner.SelectedItem.GetType()
                        .GetProperty(((text.Tag as KeyValue).Key as TextBlock).Tag.ToString())
                        .SetValue(DataGridOwner.SelectedItem, text.Text, null);
                    ((text.Tag as KeyValue).Key as TextBlock).Text = text.Text;
                    ((text.Tag as KeyValue).Value as DataGridCell).Content = (text.Tag as KeyValue).Key;
                    ((text.Tag as KeyValue).Value as DataGridCell).Focus();
                }
            };
            coreTextBox.KeyDown += delegate(object o, KeyEventArgs args)
            {
                if (args.Key != Key.Return) return;
                var text = o as CoreTextBox;
                if (text != null)
                {
                    WriteDataGridColumn.IsBusy = true;
                    DataGridOwner.SelectedItem.GetType()
                        .GetProperty(((text.Tag as KeyValue).Key as TextBlock).Tag.ToString())
                        .SetValue(DataGridOwner.SelectedItem, text.Text, null);
                    ((text.Tag as KeyValue).Key as TextBlock).Text = text.Text;
                    ((text.Tag as KeyValue).Value as DataGridCell).Content = (text.Tag as KeyValue).Key;
                    ((text.Tag as KeyValue).Value as DataGridCell).Focus();
                    Manager.Timeout(Dispatcher,()=>WriteDataGridColumn.IsBusy=false);
                }
            };
            coreTextBox.Tag = new KeyValue()
            {
                Key = textBlock,
                Value = dataGridCell
            };
            coreTextBox.SelectionStart = coreTextBox.Text.Length;
            Manager.Timeout(Dispatcher, () => coreTextBox.Focus());
            return coreTextBox;
        }
    }
}