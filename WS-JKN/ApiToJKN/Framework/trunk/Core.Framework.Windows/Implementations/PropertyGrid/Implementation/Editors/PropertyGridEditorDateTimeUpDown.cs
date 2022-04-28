using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorDateTimeUpDown : Core.Framework.Windows.Implementations.DateTimeUpDown.Implementation.DateTimeUpDown
    {
        public PropertyGridEditorDateTimeUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorDateTimeUpDownStyle"] as Style;
        }
        static PropertyGridEditorDateTimeUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDateTimeUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDateTimeUpDown)));
        }
    }
}