namespace Core.Framework.Helper.Date
{
    public class Age
    {
        protected bool Equals(Age other)
        {
            return Year == other.Year && Month == other.Month && Day == other.Day;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Year;
                hashCode = (hashCode*397) ^ Month;
                hashCode = (hashCode*397) ^ Day;
                return hashCode;
            }
        }

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

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public override string ToString()
        {
            return Year + " Year " + Month + " Month " + Day + " Day";
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Age) obj);
        }
    }
}