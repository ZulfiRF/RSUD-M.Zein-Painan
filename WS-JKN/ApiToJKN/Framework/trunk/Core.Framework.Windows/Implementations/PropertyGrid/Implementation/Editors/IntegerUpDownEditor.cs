using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class IntegerUpDownEditor : UpDownEditor<IntegerUpDown, int?>
    {
        protected override IntegerUpDown CreateEditor()
        {
            return new PropertyGridEditorIntegerUpDown();
        }
    }
}