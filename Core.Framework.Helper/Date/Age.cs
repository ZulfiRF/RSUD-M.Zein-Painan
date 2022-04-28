namespace Core.Framework.Helper.Date
{
    public class Age
    {
        #region Constructors and Destructors

        public Age(int year, byte month, byte day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public Age()
        {
            // TODO: Complete member initialization
        }

        #endregion

        #region Public Properties

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Age)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Year;
                hashCode = (hashCode * 397) ^ Month;
                hashCode = (hashCode * 397) ^ Day;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Year + " Thn " + Month + " Bln " + Day + " Hr";
        }

        #endregion

        #region Methods

        protected bool Equals(Age other)
        {
            return Year == other.Year && Month == other.Month && Day == other.Day;
        }

        #endregion
    }
}