namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;

    public class SortGridViewColumnHeader : GridViewColumnHeader
    {
        #region Constructors and Destructors

        public SortGridViewColumnHeader()
        {
            this.Click += this.SortGridViewColumnHeaderClick;
            this.SortType = SortTypeDefinition.Asc;
        }

        #endregion

        #region Enums

        public enum SortTypeDefinition
        {
            Asc,

            Desc
        }

        #endregion

        #region Public Properties

        public SortTypeDefinition SortType { get; set; }

        #endregion

        #region Methods

        private void SortGridViewColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            var sortGridViewColumnHeader = sender as SortGridViewColumnHeader;
            if (sortGridViewColumnHeader != null)
            {
                DependencyObject parent = sortGridViewColumnHeader.Parent;
                while (parent != null)
                {
                    var frameworkElement = parent as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        parent = frameworkElement.Parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        #endregion
    }
}