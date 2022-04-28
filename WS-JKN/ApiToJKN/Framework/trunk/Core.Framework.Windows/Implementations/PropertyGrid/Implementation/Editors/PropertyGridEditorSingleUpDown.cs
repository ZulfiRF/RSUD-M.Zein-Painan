using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorSingleUpDown : SingleUpDown
    {
        public PropertyGridEditorSingleUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorSingleUpDownStyle"] as Style;
        }
        static PropertyGridEditorSingleUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorSingleUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorSingleUpDown)));
        }
    }
}