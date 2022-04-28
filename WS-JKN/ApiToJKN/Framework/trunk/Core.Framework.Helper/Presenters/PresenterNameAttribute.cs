namespace Core.Framework.Helper.Presenters
{
    using System;

    public class PresenterNameAttribute : Attribute
    {
        public PresenterNameAttribute(string presenterName)
        {
            Name = presenterName;
        }

        public string Name { get; set; }
    }
}
