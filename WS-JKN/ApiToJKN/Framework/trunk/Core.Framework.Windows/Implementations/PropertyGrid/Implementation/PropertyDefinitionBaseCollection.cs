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
using Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Definitions;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation
{
    public abstract class PropertyDefinitionBaseCollection<T> : DefinitionCollectionBase<T> where T : PropertyDefinitionBase
    {
        internal PropertyDefinitionBaseCollection() { }

        public T this[object propertyId]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.TargetProperties.Contains(propertyId))
                        return item;
                }

                return null;
            }
        }

        internal T GetRecursiveBaseTypes(Type type)
        {
            // If no definition for the current type, fall back on base type editor recursively.
            T ret = null;
            while (ret == null && type != null)
            {
                ret = this[type];
                type = type.BaseType;
            }
            return ret;
        }
    }
}
