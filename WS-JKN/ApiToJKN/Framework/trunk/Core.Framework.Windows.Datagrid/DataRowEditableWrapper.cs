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
using System.Text;
using System.ComponentModel;

namespace Core.Framework.Windows.Datagrid
{
  internal class DataRowEditableWrapper : IEditableObject
  {
    public DataRowEditableWrapper( System.Data.DataRow dataRow )
    {
      m_dataRow = dataRow;
    }

    #region IEditableObject Members

    public void BeginEdit()
    {
      m_dataRow.BeginEdit();
    }

    public void CancelEdit()
    {
      m_dataRow.CancelEdit();
    }

    public void EndEdit()
    {
      m_dataRow.EndEdit();
    }

    #endregion

    private System.Data.DataRow m_dataRow;
  }
}
