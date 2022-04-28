using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class PropertyGridEditorSByteUpDown : SByteUpDown
    {
        public PropertyGridEditorSByteUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorSByteUpDownStyle"] as Style;
        }
        static PropertyGridEditorSByteUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorSByteUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorSByteUpDown)));
        }
    }
}