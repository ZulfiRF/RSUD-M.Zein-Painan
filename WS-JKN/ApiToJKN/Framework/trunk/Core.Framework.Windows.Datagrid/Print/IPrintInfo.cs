﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Collections.Generic;

namespace Core.Framework.Windows.Datagrid.Print
{
  internal interface IPrintInfo
  {
    double GetPageRightOffset( double horizontalOffset, double viewportWidth );

    void UpdateElementVisibility( double horizontalOffset, double viewportWidth, object state );

    object CreateElementVisibilityState();
  }
}
