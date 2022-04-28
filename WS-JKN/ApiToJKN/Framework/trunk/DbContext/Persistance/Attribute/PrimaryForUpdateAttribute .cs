namespace DbContext.Persistance
{
    public class PrimaryUpdateAttribute : System.Attribute
    {
        public string FieldValue { get; set; }
        public PrimaryUpdateAttribute(string FieldValue)
        {
            this.FieldValue = FieldValue;
        }
    }
}
