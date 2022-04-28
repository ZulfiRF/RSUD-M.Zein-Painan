namespace DbContext.Persistance
{
    public class PrimaryAttribute : System.Attribute
    {
        public string // Topic is a named parameter
        PrimaryKey 
        {
            get
            {
                return _PrimaryKey;
            }
            set
            {

                _PrimaryKey = value;
            }
        }
        public string // Topic is a named parameter
        AutoIncrement 
        {
            get
            {
                return _AutoIncrement;
            }
            set
            {

                _AutoIncrement = value;
            }
        }
        public PrimaryAttribute(string _PrimaryKey) // url is a positional parameter
        {
            this._PrimaryKey = _PrimaryKey;
        }
        public PrimaryAttribute(string _PrimaryKey, string _AutoIncrement)
        {
            this._PrimaryKey = _PrimaryKey;
            this._AutoIncrement = _AutoIncrement;
        }
        private string _PrimaryKey;
        private string _AutoIncrement;
    }

}
