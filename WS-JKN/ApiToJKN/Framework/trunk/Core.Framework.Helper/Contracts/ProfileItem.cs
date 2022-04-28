namespace Core.Framework.Helper.Contracts
{
    public class ProfileItem
    {
        #region Public Properties

        public string CodeProfile { get; set; }
        public string Name { get; set; }
        public string ALamat { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}