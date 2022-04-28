using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class ByteUpDownEditor : UpDownEditor<ByteUpDown, byte?>
    {

        protected override ByteUpDown CreateEditor()
        {
            return new PropertyGridEditorByteUpDown();
        }
    }
}