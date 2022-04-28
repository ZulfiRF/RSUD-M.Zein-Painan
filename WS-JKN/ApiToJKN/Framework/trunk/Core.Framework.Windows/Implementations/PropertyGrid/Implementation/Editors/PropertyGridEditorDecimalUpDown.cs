using System.Windows;
using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class PropertyGridEditorDecimalUpDown : DecimalUpDown
    {
        public PropertyGridEditorDecimalUpDown()
        {
            Style = Application.Current.Resources["PropertyGridEditorDecimalUpDownStyle"] as Style;
        }
        static PropertyGridEditorDecimalUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDecimalUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDecimalUpDown)));
        }
    }
}