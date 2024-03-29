﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Windows;
using Core.Framework.Windows.Implementations.Common;

namespace Core.Framework.Windows.Implementations.CollectionControl.Implementation
{
  public class ItemDeletingEventArgs : CancelRoutedEventArgs
  {
    #region Private Members

    private object _item;

    #endregion

    #region Constructor

    public ItemDeletingEventArgs( RoutedEvent itemDeletingEvent, object itemDeleting )
      : base( itemDeletingEvent )
    {
      _item = itemDeleting;
    }

    #region Property Item

    public object Item
    {
      get
      {
        return _item;
      }
    }

    #endregion

    #endregion
  }
}
