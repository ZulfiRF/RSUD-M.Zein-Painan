using System;

namespace Core.Framework.Helper.Contracts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MenuAttribute : Attribute
    {
        public string MenuLocation { get; set; }
        public string Icon { get; set; }
        public string HotKeys { get; set; }
        public object DomainModel { get; set; }
        public MenuAttribute()
        {
            
        }
        public MenuAttribute(params object[] objects)
        {
            DomainModel = objects;
        }
        public MenuAttribute(string menuLocation, string icon,string hotKeys)
        {
            MenuLocation = menuLocation;
            Icon = icon;
            HotKeys = hotKeys;
        }
    }
}