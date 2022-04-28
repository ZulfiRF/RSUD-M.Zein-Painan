namespace Core.Framework.Helper
{
    public class ModuleItem
    {
        public string CodeModule { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}