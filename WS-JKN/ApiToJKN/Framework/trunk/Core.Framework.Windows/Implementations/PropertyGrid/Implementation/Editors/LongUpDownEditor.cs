using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class LongUpDownEditor : UpDownEditor<LongUpDown, long?>
    {
        protected override LongUpDown CreateEditor()
        {
            return new PropertyGridEditorLongUpDown();
        }
    }
}