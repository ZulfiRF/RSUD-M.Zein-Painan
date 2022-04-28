using System;

namespace Core.Framework.Model.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SearchAttribute : Attribute
    {
        //public Type TypeModel { get; set; }
        private Type typeModel;

        public SearchAttribute()
        {
        }

        public SearchAttribute(string title)
        {
            Title = title;
        }

        public SearchAttribute(string title, string propertyPath)
        {
            Title = title;
            PropertyPath = propertyPath;
        }

        public SearchAttribute(string title, string propertyPath, params string[] filterQuery)
        {
            Title = title;
            PropertyPath = propertyPath;
            FilterQuery = filterQuery;
        }

        public string Title { get; set; }
        //public string PropertyPath { get; set; }

        public string PropertyPath { get; set; }
        public string[] FilterQuery { get; set; }

        public Type TypeModel
        {
            get { return typeModel; }
            set
            {
                typeModel = value;
                if (!string.IsNullOrEmpty(PropertyPath) && !PropertyPath.Contains("."))
                {
                    PropertyPath = (Activator.CreateInstance(typeModel) as TableItem).TableName + "." + PropertyPath;
                }
            }
        }

        private void CallBack(object state)
        {
            var item = state as SearchAttribute;
            if (item != null)
            {
                if (!item.PropertyPath.Contains("."))
                {
                    item.PropertyPath = (Activator.CreateInstance(item.TypeModel) as TableItem).TableName + "." +
                                        item.PropertyPath;
                    //item.PropertyPath = 
                }
            }
        }
    }
}