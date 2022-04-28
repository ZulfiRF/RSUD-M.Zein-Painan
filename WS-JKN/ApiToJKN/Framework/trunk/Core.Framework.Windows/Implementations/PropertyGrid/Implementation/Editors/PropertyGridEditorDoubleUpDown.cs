using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorDoubleUpDown : DoubleUpDown
    {
        public PropertyGridEditorDoubleUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorDoubleUpDownStyle"] as Style;
        }
        static PropertyGridEditorDoubleUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDoubleUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDoubleUpDown)));
        }
    }
}