﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.ComponentModel;

namespace Core.Framework.Windows.Datagrid
{
  [Flags]
  public enum CellEditorDisplayConditions
  {
    None = 0x00,
    RowIsBeingEdited = 0x01,
    MouseOverCell = 0x02,
    MouseOverRow = 0x04,
    RowIsCurrent = 0x08,
    CellIsCurrent = 0x10,
    Always = 0x20
  }
}
