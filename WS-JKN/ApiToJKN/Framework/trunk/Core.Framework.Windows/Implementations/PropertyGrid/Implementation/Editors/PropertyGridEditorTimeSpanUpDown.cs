using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorTimeSpanUpDown : Core.Framework.Windows.Implementations.TimeSpanUpDown.Implementation.TimeSpanUpDown
    {
        public PropertyGridEditorTimeSpanUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorTimeSpanUpDownStyle"] as Style;
        }
        static PropertyGridEditorTimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTimeSpanUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTimeSpanUpDown)));
        }
    }
}