using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorColorPicker : Core.Framework.Windows.Implementations.ColorPicker.Implementation.ColorPicker
    {
        public PropertyGridEditorColorPicker()
        {
            Style = (Style)Application.Current.Resources["PropertyGridEditorColorPickerStyle"];
        }
        static PropertyGridEditorColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorColorPicker), new FrameworkPropertyMetadata(typeof(PropertyGridEditorColorPicker)));
        }
    }
}