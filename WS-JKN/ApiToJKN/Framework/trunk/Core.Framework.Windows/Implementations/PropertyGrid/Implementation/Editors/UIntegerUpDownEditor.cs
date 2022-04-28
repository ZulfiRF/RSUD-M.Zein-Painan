using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class UIntegerUpDownEditor : UpDownEditor<UIntegerUpDown, uint?>
    {
        protected override UIntegerUpDown CreateEditor()
        {
            return new PropertyGridEditorUIntegerUpDown();
        }
    }
}