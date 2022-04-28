/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Editors
{
    public class ColorEditor : TypeEditor<Core.Framework.Windows.Implementations.ColorPicker.Implementation.ColorPicker>
    {
        protected override Core.Framework.Windows.Implementations.ColorPicker.Implementation.ColorPicker CreateEditor()
        {
            return new PropertyGridEditorColorPicker();
        }

        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
            Editor.DisplayColorAndName = true;
        }
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = Core.Framework.Windows.Implementations.ColorPicker.Implementation.ColorPicker.SelectedColorProperty;
        }
    }
}
