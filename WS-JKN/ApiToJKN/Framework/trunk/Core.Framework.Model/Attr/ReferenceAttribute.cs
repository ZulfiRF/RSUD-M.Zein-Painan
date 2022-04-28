using System;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class ReferenceAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceAttribute : Attribute
    {
        private string property;
        private Type typeModel;

        /// <summary>
        ///     Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        ///     Gets or sets the referenec key.
        /// </summary>
        /// <value>The referenec key.</value>
        public string[] ReferenecKey { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ReferenceItem[] Items { get; set; }

        public Type TypeModel
        {
            get { return typeModel; }
            set
            {
                typeModel = value;
                var obj = Activator.CreateInstance(typeModel);
                if (obj is TableItem)
                {
                    TableName = (obj as TableItem).TableName;
                }
            }
        }

        public string Property
        {
            get { return property; }
            set
            {
                try
                {
                    property = value;
                    var propertyItem = GetType().GetProperty(property);
                    if (propertyItem != null)
                    {
                        TypeModel = propertyItem.PropertyType;
                        var obj = Activator.CreateInstance(TypeModel);
                        if (obj is TableItem)
                        {
                            TableName = (obj as TableItem).TableName;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}