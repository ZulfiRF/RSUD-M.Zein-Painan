namespace Core.Framework.Windows.Controls
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Data;

    /// <summary>
    ///     Represents a grouping of tiles
    /// </summary>
    public class PanoramaGroup : INotifyPropertyChanged
    {
        #region Constructors and Destructors

        public PanoramaGroup(string header, ICollectionView tiles)
        {
            this.Header = header;
            this.Tiles = tiles;
        }

        public PanoramaGroup(string header, IEnumerable<object> tiles)
        {
            this.Header = header;
            this.Tiles = CollectionViewSource.GetDefaultView(tiles);
        }

        public PanoramaGroup(string header)
        {
            this.Header = header;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string Header { get; private set; }
        public ICollectionView Tiles { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void SetSource(IEnumerable<object> tiles)
        {
            this.Tiles = CollectionViewSource.GetDefaultView(tiles);
            this.OnPropertyChanged("Tiles");
        }

        #endregion

        #region Methods

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}