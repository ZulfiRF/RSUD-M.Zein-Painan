using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorComboBox : System.Windows.Controls.ComboBox
    {
        public PropertyGridEditorComboBox()
        {
            Style = (Style)Application.Current.Resources["PropertyGridEditorComboBoxStyle"];
        }
        static PropertyGridEditorComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorComboBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorComboBox)));
        }
    }
}