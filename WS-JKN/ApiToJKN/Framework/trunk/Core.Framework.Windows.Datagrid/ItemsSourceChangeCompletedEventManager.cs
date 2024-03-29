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

namespace Core.Framework.Windows.Datagrid
{
  internal class ItemsSourceChangeCompletedEventManager : WeakEventManager
  {
    private ItemsSourceChangeCompletedEventManager()
    {
    }

    public static void AddListener( DataGridControl source, IWeakEventListener listener )
    {
      CurrentManager.ProtectedAddListener( source, listener );
    }

    public static void RemoveListener( DataGridControl source, IWeakEventListener listener )
    {
      CurrentManager.ProtectedRemoveListener( source, listener );
    }

    protected override void StartListening( object source )
    {
      DataGridControl dataGridControl = ( DataGridControl )source;
      dataGridControl.ItemsSourceChangeCompleted += new EventHandler( this.OnItemsSourceChanged );
    }

    protected override void StopListening( object source )
    {
      DataGridControl dataGridControl = ( DataGridControl )source;
      dataGridControl.ItemsSourceChangeCompleted -= new EventHandler( this.OnItemsSourceChanged );
    }

    private static ItemsSourceChangeCompletedEventManager CurrentManager
    {
      get
      {
        Type managerType = typeof( ItemsSourceChangeCompletedEventManager );
        ItemsSourceChangeCompletedEventManager currentManager = ( ItemsSourceChangeCompletedEventManager )WeakEventManager.GetCurrentManager( managerType );

        if( currentManager == null )
        {
          currentManager = new ItemsSourceChangeCompletedEventManager();
          WeakEventManager.SetCurrentManager( managerType, currentManager );
        }

        return currentManager;
      }
    }

    private void OnItemsSourceChanged( object sender, EventArgs args )
    {
      this.DeliverEvent( sender, args );
    }
  }
}
