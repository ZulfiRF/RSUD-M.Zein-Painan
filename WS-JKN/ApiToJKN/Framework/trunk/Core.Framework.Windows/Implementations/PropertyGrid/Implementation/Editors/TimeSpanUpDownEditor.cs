using System;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class TimeSpanUpDownEditor : UpDownEditor<Core.Framework.Windows.Implementations.TimeSpanUpDown.Implementation.TimeSpanUpDown, TimeSpan?>
    {
        protected override Core.Framework.Windows.Implementations.TimeSpanUpDown.Implementation.TimeSpanUpDown CreateEditor()
        {
            return new PropertyGridEditorTimeSpanUpDown();
        }
    }
}