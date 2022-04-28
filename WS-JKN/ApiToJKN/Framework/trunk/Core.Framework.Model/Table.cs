using System.ComponentModel;

namespace Core.Framework.Model
{
    /// <summary>
    ///     Class Table
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public class Table<TEntity> : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Implementation of INotifyPropertyChanged

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}