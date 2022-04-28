using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public abstract class WriteDataGridColumn : DataGridTextColumn
    {
        public abstract FrameworkElement CreateElement(TextBlock textBlock, DataGridCell dataGridCell);


        public static bool IsBusy { get; set; }
    }
}