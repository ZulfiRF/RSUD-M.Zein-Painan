using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorIntegerUpDown : IntegerUpDown
    {
        public PropertyGridEditorIntegerUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorIntegerUpDownStyle"] as Style;
        }
        static PropertyGridEditorIntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorIntegerUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorIntegerUpDown)));
        }
    }
}