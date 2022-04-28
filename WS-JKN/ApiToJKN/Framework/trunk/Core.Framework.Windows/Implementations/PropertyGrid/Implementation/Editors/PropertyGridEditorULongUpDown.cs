using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class PropertyGridEditorULongUpDown : ULongUpDown
    {
        public PropertyGridEditorULongUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorULongUpDownStyle"] as Style;
        }
        static PropertyGridEditorULongUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorULongUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorULongUpDown)));
        }
    }
}