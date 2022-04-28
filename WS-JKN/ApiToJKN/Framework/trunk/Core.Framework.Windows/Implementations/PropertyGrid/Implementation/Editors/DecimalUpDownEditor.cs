using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class DecimalUpDownEditor : UpDownEditor<DecimalUpDown, decimal?>
    {
        protected override DecimalUpDown CreateEditor()
        {
            return new PropertyGridEditorDecimalUpDown();
        }
    }
}