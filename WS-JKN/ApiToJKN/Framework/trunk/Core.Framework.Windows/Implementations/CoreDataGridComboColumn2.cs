using System.Windows;
using System.Windows.Controls;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridComboColumn2 : DataGridColumn
    {
        // Using a DependencyProperty as the backing store for DomainNameSpaces.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(CoreDataGridComboColumn),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty DomainNameSpacesProperty =
            DependencyProperty.Register(
                "DomainNameSpaces",
                typeof(string),
                typeof(CoreDataGridComboColumn),
                new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for UsingSearchByFramework.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsingSearchByFrameworkProperty =
            DependencyProperty.Register(
                "UsingSearchByFramework",
                typeof(bool),
                typeof(CoreDataGridComboColumn),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath",
            typeof(string),
            typeof(CoreDataGridComboColumn),
            new UIPropertyMetadata(null));

        #endregion

        #region Public Properties

        public string DisplayMemberPath
        {
            get
            {
                return (string)this.GetValue(DisplayMemberPathProperty);
            }
            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        public string DomainNameSpaces
        {
            get
            {
                return (string)this.GetValue(DomainNameSpacesProperty);
            }
            set
            {
                this.SetValue(DomainNameSpacesProperty, value);
            }
        }

        public bool UsingSearchByFramework
        {
            get
            {
                return (bool)this.GetValue(UsingSearchByFrameworkProperty);
            }
            set
            {
                this.SetValue(UsingSearchByFrameworkProperty, value);
            }
        }

        public string ValuePath
        {
            get
            {
                return (string)this.GetValue(ValuePathProperty);
            }
            set
            {
                this.SetValue(ValuePathProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     When overridden in a derived class, gets an editing element that is bound to the
        ///     <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value of the column.
        /// </summary>
        /// <returns>
        ///     A new editing element that is bound to the <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" />
        ///     property value of the column.
        /// </returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item that is represented by the row that contains the intended cell.</param>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return null;
        }

        /// <summary>
        ///     When overridden in a derived class, gets a read-only element that is bound to the
        ///     <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value of the column.
        /// </summary>
        /// <returns>
        ///     A new read-only element that is bound to the <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" />
        ///     property value of the column.
        /// </returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item that is represented by the row that contains the intended cell.</param>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var combo = new CoreComboBox();

            combo.Style = Application.Current.Resources["MetroComboBox"] as Style;
            combo.Loaded += this.ComboOnLoaded;
            combo.Height = cell.Height;
            combo.DisplayMemberPath = this.DisplayMemberPath;
            combo.UsingSearchByFramework = this.UsingSearchByFramework;
            combo.DomainNameSpaces = this.DomainNameSpaces;
            combo.ValuePath = this.ValuePath;

            return combo;
        }

        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var coreComboBox = sender as CoreComboBox;
            if (coreComboBox != null)
            {
                Manager.Timeout(coreComboBox.Dispatcher, () => { coreComboBox.OnApplyTemplate(); });
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
    }
}