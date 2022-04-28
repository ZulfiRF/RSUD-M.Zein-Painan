using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridTemplateColumn:DataGridTemplateColumn,IMultipleHeader
    {
        public void SetHeader()
        {
            if (Header is string) if (Header.ToString().Contains("^"))
            {
                var arr = Header.ToString().Split(new[] { '^' });
                var stackPanel = new StackPanel();
                foreach (var s in arr)
                {
                    var text = new TextBlock();
                    text.Text = s.ToUpper();
                    text.FontWeight = FontWeights.SemiBold;
                    text.Margin = new Thickness(0);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(text);
                }
                Header = stackPanel;
            }
        }
    }
}