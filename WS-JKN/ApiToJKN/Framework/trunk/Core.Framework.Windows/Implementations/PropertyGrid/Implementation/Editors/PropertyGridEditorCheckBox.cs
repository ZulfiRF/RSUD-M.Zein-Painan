using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorCheckBox : CheckBox
    {
        public PropertyGridEditorCheckBox()
        {
            Style = (Style) Application.Current.Resources["PropertyGridEditorCheckBoxStyle"];
        }
        static PropertyGridEditorCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckBox)));
        }
    }
}