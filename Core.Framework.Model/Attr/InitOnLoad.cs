using System;
using System.Linq;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class InitOnLoad
    /// </summary>
    public class InitOnLoad : Attribute
    {
        /// <summary>
        ///     Initialises the specified table item.
        /// </summary>
        /// <param name="tableItem">The table item.</param>
        public static void Initialise(TableItem tableItem)
        {
            foreach (
                var property in
                    tableItem.GetType()
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes(typeof (InitOnLoad), false).Any()))
            {
                if (property.GetValue(tableItem, null) == null)
                    property.SetValue(tableItem, Activator.CreateInstance(property.PropertyType), null);
            }
        }
    }
}