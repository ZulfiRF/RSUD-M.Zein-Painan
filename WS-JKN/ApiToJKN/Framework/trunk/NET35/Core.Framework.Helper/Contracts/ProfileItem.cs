namespace Core.Framework.Helper.Contracts
{
    public class ProfileItem
    {
        public string CodeProfile { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}