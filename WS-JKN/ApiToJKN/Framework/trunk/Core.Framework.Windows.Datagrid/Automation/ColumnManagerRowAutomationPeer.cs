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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation.Peers;

namespace Core.Framework.Windows.Datagrid.Automation
{
  public class ColumnManagerRowAutomationPeer : RowAutomationPeer
  {
    public ColumnManagerRowAutomationPeer( ColumnManagerRow owner )
      : base( owner )
    {
    }

    public new ColumnManagerRow Owner
    {
      get
      {
        return base.Owner as ColumnManagerRow;
      }
    }

    protected override bool IsContentElementCore()
    {
      return false;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Header;
    }
  }
}
