﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/
using Core.Framework.Windows.Implementations.DateTimeUpDown.Implementation;
using System;
using System.Windows.Data;
using Core.Framework.Windows.Implementations.TimePicker.Implementation;

namespace Core.Framework.Windows.Implementations.Common.Converters
{
  public class TimeFormatToDateTimeFormatConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      TimeFormat timeFormat = ( TimeFormat )value;
      switch( timeFormat )
      {
        case TimeFormat.Custom:
          return DateTimeFormat.Custom;
        case TimeFormat.ShortTime:
          return DateTimeFormat.ShortTime;
        case TimeFormat.LongTime:
          return DateTimeFormat.LongTime;
        default:
          return DateTimeFormat.ShortTime;
      }
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
