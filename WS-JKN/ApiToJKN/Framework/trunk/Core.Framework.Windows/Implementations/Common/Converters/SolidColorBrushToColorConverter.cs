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
using System.Windows.Data;
using System.Windows.Media;

namespace Core.Framework.Windows.Implementations.Common.Converters
{
  public class SolidColorBrushToColorConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      SolidColorBrush brush = value as SolidColorBrush;
      if( brush != null )
        return brush.Color;

      return default( Color );
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      if( value != null )
      {
        Color color = ( Color )value;
        return new SolidColorBrush( color );
      }

      return default( SolidColorBrush );
    }

    #endregion
  }
}
