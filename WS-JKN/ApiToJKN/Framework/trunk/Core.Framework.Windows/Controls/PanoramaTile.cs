using System.ComponentModel;
using System.Windows.Input;

namespace Core.Framework.Windows.Controls
{
    public class PanoramaTile : INotifyPropertyChanged, IPanoramaTile
    {
        #region Fields

        private string text;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.RaisePropertyChanged("Text");
            }
        }

        public ICommand TileClickedCommand
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}