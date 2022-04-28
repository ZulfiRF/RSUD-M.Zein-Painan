using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorShortUpDown : ShortUpDown
    {
        public PropertyGridEditorShortUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorShortUpDownStyle"] as Style;
        }
        static PropertyGridEditorShortUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorShortUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorShortUpDown)));
        }
    }
}