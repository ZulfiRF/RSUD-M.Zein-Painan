namespace Core.Framework.Helper.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = true)]
    public class MenuAttribute : Attribute
    {
        #region Constructors and Destructors

        public MenuAttribute()
        {
        }

        public MenuAttribute(params object[] objects)
        {
            DomainModel = objects;
        }

        public MenuAttribute(string menuLocation, string icon, string hotKeys)
        {
            MenuLocation = menuLocation;
            Icon = icon;
            HotKeys = hotKeys;
        }

        #endregion

        #region Public Properties

        public object DomainModel { get; set; }
        public string HotKeys { get; set; }
        public string Icon { get; set; }
        public string MenuLocation { get; set; }
        public bool ShowInRightClickMenu { get; set; }
        public string Title { get; set; }

        public string Module { get; set; }

        #endregion
    }
}