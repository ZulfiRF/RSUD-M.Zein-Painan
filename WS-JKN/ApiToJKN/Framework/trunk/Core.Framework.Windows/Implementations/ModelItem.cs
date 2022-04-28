namespace Core.Framework.Windows.Implementations
{
    public class ModelItem
    {
        #region Constructors and Destructors

        public ModelItem(object key, object description)
        {
            this.Key = key;
            this.Description = description;
        }

        #endregion

        #region Public Properties

        public object Description { get; set; }
        public object Key { get; set; }

        #endregion
    }
}