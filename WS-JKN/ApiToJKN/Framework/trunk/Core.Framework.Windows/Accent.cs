namespace Core.Framework.Windows
{
    using System;
    using System.Windows;

    public class Accent
    {
        #region Fields

        public ResourceDictionary Resources;

        #endregion

        #region Constructors and Destructors

        public Accent()
        {
        }

        public Accent(string name, Uri resourceAddress)
        {
            this.Name = name;
            this.Resources = new ResourceDictionary { Source = resourceAddress };
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        #endregion
    }
}