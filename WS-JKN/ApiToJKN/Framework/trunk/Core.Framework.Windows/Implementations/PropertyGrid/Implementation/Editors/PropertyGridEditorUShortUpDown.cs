using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class PropertyGridEditorUShortUpDown : UShortUpDown
    {
        public PropertyGridEditorUShortUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorUShortUpDownStyle"] as Style;
        }
        static PropertyGridEditorUShortUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorUShortUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorUShortUpDown)));
        }
    }
}