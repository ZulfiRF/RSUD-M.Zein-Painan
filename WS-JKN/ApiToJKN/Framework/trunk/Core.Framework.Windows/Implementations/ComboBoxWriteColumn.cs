using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Core.Framework.Helper;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class ComboBoxWriteColumn : WriteDataGridColumn
    {
        public event EventHandler<ItemEventArgs<CoreComboBox>> InitializeCombo;

        protected virtual void OnInitializeCombo(ItemEventArgs<CoreComboBox> e)
        {
            EventHandler<ItemEventArgs<CoreComboBox>> handler = InitializeCombo;
            if (handler != null) handler(this, e);
        }

        public string ValuePath { get; set; }
        public override FrameworkElement CreateElement(TextBlock textBlock, DataGridCell dataGridCell)
        {
            try
            {
                var binding = this.Binding as Binding;
                if (binding != null) textBlock.Tag = binding.Path.Path;
                var coreTextBox = new CoreComboBox();
                coreTextBox.Temp = DataGridOwner.SelectedItem;
                coreTextBox.DisplayMemberPath = DisplayMemberPath;
                coreTextBox.ValuePath = ValueDisplayPath;
                coreTextBox.DomainNameSpaces = DomainNameSpaces;
                OnInitializeCombo(new ItemEventArgs<CoreComboBox>(coreTextBox));
                coreTextBox.KeyUp += delegate(object o, KeyEventArgs args)
                {
                    if (args.Key == Key.Escape)
                    {
                        var text = o as CoreComboBox;
                        if (text == null) return;
                        HideControl(text);
                    }
                };
                coreTextBox.KeyUp += delegate(object o, KeyEventArgs args)
                {
                    if (args.Key != Key.Return) return;
                    var text = o as CoreComboBox;
                    if (text != null)
                    {
                        ComboBoxWriteColumn.IsBusy = true;
                        var type = coreTextBox.Temp.GetType();
                        type.GetProperty(((text.Tag as KeyValue).Key as TextBlock).Tag.ToString())
                            .SetValue(coreTextBox.Temp, text.Text, null);
                        type.GetProperty(ValuePath)
                            .SetValue(coreTextBox.Temp, text.Value, null);
                        ((text.Tag as KeyValue).Key as TextBlock).Text = text.Text;
                        ((text.Tag as KeyValue).Value as DataGridCell).Content = (text.Tag as KeyValue).Key;
                        ((text.Tag as KeyValue).Value as DataGridCell).Focus();
                        Manager.Timeout(Dispatcher, () => ComboBoxWriteColumn.IsBusy = false);
                    }
                };
                coreTextBox.Tag = new KeyValue()
                {
                    Key = textBlock,
                    Value = dataGridCell
                };
                coreTextBox.Value = DataGridOwner.SelectedItem.GetType().GetProperty(ValuePath).GetValue(DataGridOwner.SelectedItem, null);
                Manager.Timeout(Dispatcher, () => coreTextBox.Focus());
                return coreTextBox;
            }
            catch (Exception)
            {
                return new CoreComboBox();
            }
        }

        private void HideControl(CoreComboBox text)
        {
            var keyValue = text.Tag as KeyValue;
            if (keyValue == null) return;
            ((TextBlock)keyValue.Key).Text = text.Text;
            ((DataGridCell)keyValue.Value).Content = (text.Tag as KeyValue).Key;
            ((DataGridCell)keyValue.Value).Focus();
            Manager.Timeout(Dispatcher, () => IsBusy = false);
        }

        public string DisplayMemberPath { get; set; }

        public string DomainNameSpaces { get; set; }

        public string ValueDisplayPath { get; set; }
    }
}