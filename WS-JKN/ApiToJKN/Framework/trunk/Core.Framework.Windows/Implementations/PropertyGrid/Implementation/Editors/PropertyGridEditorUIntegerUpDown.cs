using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class PropertyGridEditorUIntegerUpDown : UIntegerUpDown
    {
        public PropertyGridEditorUIntegerUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorUIntegerUpDownStyle"] as Style;
        }
        static PropertyGridEditorUIntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorUIntegerUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorUIntegerUpDown)));
        }
    }
}