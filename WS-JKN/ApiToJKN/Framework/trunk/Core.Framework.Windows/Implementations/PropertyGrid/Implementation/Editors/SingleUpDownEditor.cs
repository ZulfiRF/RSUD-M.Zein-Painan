using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class SingleUpDownEditor : UpDownEditor<SingleUpDown, float?>
    {
        protected override SingleUpDown CreateEditor()
        {
            return new PropertyGridEditorSingleUpDown();
        }

        protected override void SetControlProperties()
        {
            base.SetControlProperties();
            Editor.AllowInputSpecialValues = AllowedSpecialValues.Any;
        }
    }
}