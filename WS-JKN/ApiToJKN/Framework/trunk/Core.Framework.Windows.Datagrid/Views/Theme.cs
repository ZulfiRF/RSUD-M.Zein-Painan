/*************************************************************************************

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
using System.ComponentModel;
using System.Text;
using System.Windows;
using Core.Framework.Windows.Datagrid.Markup;

namespace Core.Framework.Windows.Datagrid.Views
{
  [TypeConverter( typeof( ThemeConverter ) )]
  public abstract class Theme : DependencyObject
  {
    public bool IsViewSupported( Type viewType )
    {
      return Theme.IsViewSupported( viewType, this.GetType() );
    }

    public static bool IsViewSupported( Type viewType, Type themeType )
    {
      object[] attributes = themeType.GetCustomAttributes( typeof( TargetViewAttribute ), true );

      foreach( TargetViewAttribute attribute in attributes )
      {
        if( attribute.ViewType == viewType )
          return true;
      }

      return false;
    }
  }
}
