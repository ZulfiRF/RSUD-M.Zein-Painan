using System;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class DateTimeUpDownEditor : UpDownEditor<Core.Framework.Windows.Implementations.DateTimeUpDown.Implementation.DateTimeUpDown, DateTime?>
    {
        protected override Core.Framework.Windows.Implementations.DateTimeUpDown.Implementation.DateTimeUpDown CreateEditor()
        {
            return new PropertyGridEditorDateTimeUpDown();
        }
    }
}