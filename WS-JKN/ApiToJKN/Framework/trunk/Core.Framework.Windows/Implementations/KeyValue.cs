namespace Core.Framework.Windows.Implementations
{
    using System.ComponentModel;

    public class KeyValue<TKey, TValue> : INotifyPropertyChanged
    {
        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        private TKey key;

        public TKey Key
        {
            get { return key; }
            set
            {
                key = value;
                this.OnPropertyChanged("Key");
            }
        }

        private TValue _value;

        public TValue Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.OnPropertyChanged("Value");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class KeyValue : INotifyPropertyChanged
    {
        #region Fields

        private bool isActive;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;
                this.OnPropertyChanged("IsActive");
            }
        }

        //public object Key { get; set; }
        private object key;

        public object Key
        {
            get { return key; }
            set
            {
                key = value;
                this.OnPropertyChanged("Key");
            }
        }

        private object _value;

        public KeyValue()
        {
            
        }
        public KeyValue(string value)
        {
            Key = value;
            Value = value;
        }

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.OnPropertyChanged("Value");
            }
        }

        //public object Value { get; set; }
        //public string Alias { get; set; }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}