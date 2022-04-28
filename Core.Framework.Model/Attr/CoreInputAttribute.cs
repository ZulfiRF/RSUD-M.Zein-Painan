using System;

namespace Core.Framework.Model.Attr
{
    public class CoreInputAttribute : Attribute
    {
        public CoreInputAttribute()
        {
        }

        /// <summary>
        ///     For Combobox
        /// </summary>
        /// <param name="title"></param>
        /// <param name="index"></param>
        /// <param name="model"></param>
        /// <param name="displayPath"></param>
        /// <param name="valuePath"></param>
        public CoreInputAttribute(string title, int index, Type model, string displayPath, string valuePath)
        {
            Type = FormType.ListItem;
            Title = title;
            Index = index;

            DisplayPath = displayPath;
            ValuePath = valuePath;
        }

        public int Index { get; set; }
        public FormType Type { get; set; }
        public Type Converter { get; set; }
        public string Title { get; set; }
        public string ValuePath { get; set; }
        public string DisplayPath { get; set; }
        public Type TypeModel { get; set; }

        public TableItem ModelItem
        {
            get { return Activator.CreateInstance(TypeModel) as TableItem; }
        }
    }
}