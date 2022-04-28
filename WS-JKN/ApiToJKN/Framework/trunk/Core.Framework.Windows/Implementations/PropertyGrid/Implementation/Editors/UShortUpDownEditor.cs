using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class UShortUpDownEditor : UpDownEditor<UShortUpDown, ushort?>
    {
        protected override UShortUpDown CreateEditor()
        {
            return new PropertyGridEditorUShortUpDown();
        }
    }
}