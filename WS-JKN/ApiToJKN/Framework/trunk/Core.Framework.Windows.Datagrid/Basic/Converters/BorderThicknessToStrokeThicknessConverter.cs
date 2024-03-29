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
using System.Windows;
using System.Windows.Data;

namespace Core.Framework.Windows.Datagrid.Basic.Converters
{
  public class BorderThicknessToStrokeThicknessConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      Thickness thickness = ( Thickness )value;
      return ( thickness.Bottom + thickness.Left + thickness.Right + thickness.Top ) / 4;
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      int? thick = ( int? )value;
      int thickValue = thick.HasValue ? thick.Value : 0;

      return new Thickness( thickValue, thickValue, thickValue, thickValue );
    }

    #endregion
  }
}
