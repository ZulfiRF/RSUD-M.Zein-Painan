using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class ShortUpDownEditor : UpDownEditor<ShortUpDown, short?>
    {
        protected override ShortUpDown CreateEditor()
        {
            return new PropertyGridEditorShortUpDown();
        }
    }
}