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
using System.Globalization;

namespace Core.Framework.Windows.Datagrid.Export
{
  public abstract class FormatSettingsBase
  {
    public string DateTimeFormat
    {
      get;
      set;
    }

    public string NumericFormat
    {
      get;
      set;
    }

    public string FloatingPointFormat
    {
      get;
      set;
    }

    public CultureInfo Culture
    {
      get;
      set;
    }
  }
}
