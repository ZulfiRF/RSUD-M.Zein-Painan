using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorLongUpDown : LongUpDown
    {
        public PropertyGridEditorLongUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorLongUpDownStyle"] as Style;
        }
        static PropertyGridEditorLongUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorLongUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorLongUpDown)));
        }
    }
}