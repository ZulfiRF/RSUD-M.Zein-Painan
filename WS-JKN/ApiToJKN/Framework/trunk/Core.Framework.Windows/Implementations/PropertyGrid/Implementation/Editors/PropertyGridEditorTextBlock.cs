using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorTextBlock : TextBlock
    {
        public PropertyGridEditorTextBlock()
        {
            Style = (Style)Application.Current.Resources["PropertyGridEditorTextBlockStyle"];
        }
        static PropertyGridEditorTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBlock), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBlock)));
        }
    }
}