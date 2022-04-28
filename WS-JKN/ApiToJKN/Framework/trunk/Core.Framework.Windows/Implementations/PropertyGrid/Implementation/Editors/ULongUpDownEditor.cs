using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class ULongUpDownEditor : UpDownEditor<ULongUpDown, ulong?>
    {
        protected override ULongUpDown CreateEditor()
        {
            return new PropertyGridEditorULongUpDown();
        }
    }
}