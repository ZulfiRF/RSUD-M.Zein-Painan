using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorTextBox : Core.Framework.Windows.Implementations.WatermarkTextBox.Implementation.WatermarkTextBox
    {
        public PropertyGridEditorTextBox()
        {
            var a = Application.Current.Resources["PropertyGridEditorTextBoxStyle"];
            Style = (Style)a;
        }
        static PropertyGridEditorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
        }
    }
}