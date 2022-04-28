using Core.Framework.Windows.Implementations.NumericUpDown.Implementation;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class DoubleUpDownEditor : UpDownEditor<DoubleUpDown, double?>
    {
        protected override DoubleUpDown CreateEditor()
        {
            return new PropertyGridEditorDoubleUpDown();
        }

        protected override void SetControlProperties()
        {
            base.SetControlProperties();
            Editor.AllowInputSpecialValues = AllowedSpecialValues.Any;
        }
    }
}