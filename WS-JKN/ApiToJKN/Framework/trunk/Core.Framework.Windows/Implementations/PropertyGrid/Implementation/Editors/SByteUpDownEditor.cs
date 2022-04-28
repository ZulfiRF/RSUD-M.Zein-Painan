using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    internal class SByteUpDownEditor : UpDownEditor<SByteUpDown, sbyte?>
    {
        protected override SByteUpDown CreateEditor()
        {
            return new PropertyGridEditorSByteUpDown();
        }
    }
}