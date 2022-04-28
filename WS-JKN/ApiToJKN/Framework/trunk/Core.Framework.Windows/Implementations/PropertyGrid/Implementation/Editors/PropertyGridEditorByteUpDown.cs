using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorByteUpDown : ByteUpDown
    {
        public PropertyGridEditorByteUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorByteUpDownStyle"] as Style;
        }
        static PropertyGridEditorByteUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorByteUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorByteUpDown)));
        }
    }
}