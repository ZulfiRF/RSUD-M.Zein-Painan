using System;

namespace Core.Framework.Model.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FormInputAttribute : CoreInputAttribute
    {
        public FormInputAttribute()
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
        public FormInputAttribute(string title, int index, Type model, string displayPath, string valuePath)
            : base(title, index, model, displayPath, valuePath)
        {
        }

        public FormInputAttribute(string title, int index, string models, string displayPath, string valuePath)
            : base(title, index, null, displayPath, valuePath)
        {
            Models = models;
        }

        public string Models { get; set; }
    }
}