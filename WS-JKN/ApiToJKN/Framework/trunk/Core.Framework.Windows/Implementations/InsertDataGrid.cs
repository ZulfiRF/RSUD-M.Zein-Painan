using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Model;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Helper;


    public class InsertDataGrid : DataGrid, IGridViewControl
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            foreach (var dataGridColumn in Columns.OfType<IMultipleHeader>())
            {
                dataGridColumn.SetHeader();
            }
        }
        #region Fields

        private FrameworkElement currentControlFocus;

        #endregion

        #region Constructors and Destructors

        public InsertDataGrid()
        {
            this.IsReadOnly = true;
            this.FocusInColum = -1;
            this.KeyDown += this.CoreDataGridKeyDown;
            GridLinesVisibility = DataGridGridLinesVisibility.None;
            AlternatingRowBackground = new SolidColorBrush(Colors.LightGray);
            Foreground = new SolidColorBrush(Colors.Black);
            KeyUp += OnKeyDown;
            MouseDoubleClick += OnMouseDoubleClick;
            AutoGenerateColumns = false;
            IsDeleteOn = true;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            BindInsert();
        }

        public bool IsDeleteOn { get; set; }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Return)
            {
                keyEventArgs.Handled = false;
                BindInsert();
            }
            else if (keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Control && keyEventArgs.Key == Key.Delete)
            {
                if (IsDeleteOn)
                    if (Manager.Confirmation("Konfirmasi", "Apakah anda akan menghapus data ini?"))
                    {
                        //Items.Remove(SelectedItems as IList);
                        OnControlDelete(new ItemEventArgs<object>(this.SelectedItems));
                    }
            }
        }

        private void BindInsert()
        {
            if (SelectedItem == null) return;
            if (!(CurrentColumn is WriteDataGridColumn)) return;
            if (WriteDataGridColumn.IsBusy) return;
            var textBlock = CurrentColumn.GetCellContent(SelectedItem) as TextBlock;
            if (textBlock != null)
            {
                var dataGridCell = textBlock.Parent as DataGridCell;
                if (dataGridCell != null)
                {

                    //dataGridCell.Content = null;Binding = {System.Windows.Data.Binding}

                    var parent = dataGridCell.Column as WriteDataGridColumn;
                    if (parent == null) return;
                    var coreTextBox = parent.CreateElement(textBlock, dataGridCell);
                    dataGridCell.Content = coreTextBox;

                }
            }
        }


        #endregion

        #region Public Events

        public event EventHandler<SelectedControlArgs> SelectedControl;
        public event EventHandler<ItemEventArgs<object>> ControlDelete;

        /// <summary>
        /// Ketika di Ctrl + Dlete
        /// </summary>
        /// <param name="e"></param>
        public void OnControlDelete(ItemEventArgs<object> e)
        {
            var handler = ControlDelete;
            if (handler != null) handler(this, e);

            var item = this.SelectedItem;
            if (ItemsSource == null)
                this.Items.Remove(item);
            else if (ItemsSource is IList)
            {
                var list = (ItemsSource as IList);
                var index = list.IndexOf(item);
                list.Remove(item);
                ItemsSource = list;
                Items.Refresh();
                index--;
                SelectedItem = list[index];
            }
            var currentColumns = CurrentColumn as IFocusControl;
            if (currentColumns != null)
                currentColumns.FocusControl();            
        }

        #endregion

        #region Public Properties

        public IEnumerable DataSource
        {
            get
            {
                return this.Items;
            }
            set
            {
                //ItemsSource = value;
                this.Items.Clear();
                foreach (object item in value)
                {
                    this.Items.Add(item);
                }
            }
        }



        public bool AllowAddRows
        {
            get { return (bool)GetValue(AllowAddRowsProperty); }
            set { SetValue(AllowAddRowsProperty, value); }
        }

        public static readonly DependencyProperty AllowAddRowsProperty =
            DependencyProperty.Register("AllowAddRows", typeof(bool), typeof(InsertDataGrid), new UIPropertyMetadata(false));



        public int FocusInColum { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnSelectedControl(SelectedControlArgs e)
        {
            EventHandler<SelectedControlArgs> handler = this.SelectedControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                base.OnKeyDown(e);
            }


            //if (e.Key == Key.Return)
            //    e.Handled = true;
        }



        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            //if (CanUserAddRows == false) return;
            if (AllowAddRows == false)
            {
                base.OnSelectionChanged(e);
                return;
            }
            else
            {
                var index = 0;
                foreach (var item in Items)
                {
                    if (item.Equals(SelectedItem))
                    {
                        index++;
                        break;
                    }
                    index++;
                }
                if (this.SelectedIndex == this.Items.Count - 1 || SelectedIndex == -1)
                {
                    if (SelectedItem != null)
                    {
                        CreateNewItem();
                    }
                }
            }
            base.OnSelectionChanged(e);
        }

        public object CreateNewItem()
        {
            Type type = this.SelectedItem.GetType();
            var item = Activator.CreateInstance(type);
            if (ItemsSource == null)
            {
                if (Items.Count > 1)
                {
                    var items = Items.Cast<TableItem>();
                    if (items != null)
                    {
                        var count = items.Count(n => n.Tag == null);
                        if (count == 1 || count == 0)
                        {
                            this.Items.Add(item);
                        }
                    }
                    else
                        this.Items.Add(item);
                }
                else
                    this.Items.Add(item);
            }
            else if (ItemsSource is IList)
            {
                var list = (ItemsSource as IList);
                list.Add(item);
                var selectedItem = SelectedItem;
                ItemsSource = list;
                var cell = CurrentCell;
                Items.Refresh();
                SelectedItem = selectedItem;
                CurrentCell = cell;
            }
            return item;
        }

        private void CoreDataGridKeyDown(object sender, KeyEventArgs e)
        {
            if (FreezeKeyDown)
                return;
            if ((RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible || RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected) && RowDetailsTemplate != null)
                return;
            if (this.CurrentColumn == null)
            {
                return;
            }
            if (this.SelectedItem == null)
            {
                return;
            }
            ContentPresenter cp = null;
            if (this.FocusInColum == -1)
            {
                if (this.CurrentColumn.GetCellContent(this.SelectedItem) != null)
                {
                    FrameworkElement frameworkElement = this.CurrentColumn.GetCellContent(this.SelectedItem);
                    if (frameworkElement != null)
                    {
                        frameworkElement.Focus();
                        return;
                    }
                }
                cp = this.CurrentColumn.GetCellContent(this.SelectedItem) as ContentPresenter;
            }
            else
            {
                cp = this.Columns[this.FocusInColum].GetCellContent(this.SelectedItem) as ContentPresenter;
            }
            var dp = Manager.FindVisualChild<FrameworkElement>(cp);
            if (dp != null)
            {
                dp.Focus();
            }

            var cell = sender as DataGridCell;
            if (cell == null)
            {
                cell = Manager.FindVisualParent<DataGridCell>(dp);
            }
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                var dataGrid = Manager.FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                        {
                            cell.IsSelected = true;
                        }
                    }
                    else
                    {
                        var row = Manager.FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
            if (this.currentControlFocus != null)
            {
                if (!this.currentControlFocus.Equals(dp))
                {
                    this.OnSelectedControl(new SelectedControlArgs(dp));
                }
            }
            this.currentControlFocus = dp;
        }

        #endregion

        public bool FreezeKeyDown { get; set; }
    }
}