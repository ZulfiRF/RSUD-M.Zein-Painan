using System.ComponentModel;

namespace Core.Framework.Windows.Windows
{
    public class Notification : INotifyPropertyChanged
    {
        #region Fields

        private int id;

        private string imageUrl;

        private string message;

        private string title;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id == value)
                {
                    return;
                }
                this.id = value;
                this.OnPropertyChanged("Id");
            }
        }

        public string ImageUrl
        {
            get
            {
                return this.imageUrl;
            }

            set
            {
                if (this.imageUrl == value)
                {
                    return;
                }
                this.imageUrl = value;
                this.OnPropertyChanged("ImageUrl");
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message == value)
                {
                    return;
                }
                this.message = value;
                this.OnPropertyChanged("Message");
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (this.title == value)
                {
                    return;
                }
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

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
    }
}