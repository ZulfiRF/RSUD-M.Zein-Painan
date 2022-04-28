using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Helper;

    [TemplatePart(Name = PART_TextLoading, Type = typeof(UIElement))]
    public class CoreDataGrid : DataGrid, IGridViewControl
    {
        // Using a DependencyProperty as the backing store for UseFindButton.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_ProgressBar = "PART_ProgressBar";

        private const string PART_ScrollViewer = "DG_ScrollViewer";

        private const string PART_TextLoading = "PART_TextLoading";

        private const string Part_BtnFind = "btnFind";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty UseContextMenuInHeaderProperty =
            DependencyProperty.Register(
                "UseContextMenuInHeader",
                typeof(bool),
                typeof(CoreDataGrid),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty UseFindButtonProperty = DependencyProperty.Register(
            "UseFindButton",
            typeof(bool),
            typeof(CoreDataGrid),
            new UIPropertyMetadata(false, ChangeUseFindButton));

        #endregion

        #region Fields

        private readonly List<string> hasGroupBy = new List<string>();

        private int countRowInGrid;

        private int countRowLoad;

        private FrameworkElement currentControlFocus;

        private ContextMenu cxMenu;

        private IEnumerable dataSource;

        private List<object> defaulItemSource;

        private MenuItem menuItem;

        #endregion

        #region Constructors and Destructors

        public CoreDataGrid()
        {
            this.BorderThickness = new Thickness(0, 0, 0, 0);
            this.GridLinesVisibility = DataGridGridLinesVisibility.None;
            //this.LoadingRow += this.CoreDataGridLoadingRow;
            this.KeyUp += this.CoreDataGridKeyUp;
            this.KeyDown += this.CoreDataGridKeyDown;
            this.KeyDown += this.CoreDataGridSpecialKeyDown;
            this.DataContextChanged += this.OnDataContextChanged;
            this.FocusInColum = -1;
            this.PreviewMouseLeftButtonDown += this.DataGridCell_PreviewMouseLeftButtonDown;
            this.currentControlFocus = new FrameworkElement();
            this.Loaded += this.CoreDataGridLoaded;
            this.AutoGeneratingColumn += this.CoreDataGrid_AutoGeneratingColumn;
            this.MouseRightButtonUp += this.CoreMouseRightButtonUp;
            this.IsReadOnly = true;
            this.MouseDoubleClick += OnMouseDoubleClick;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Items.Refresh();
        }

        #endregion

        #region Public Events

        public event EventHandler AddRowAutomatic;

        public event EventHandler DeletePress;

        public event EventHandler<DataGridRowEventArgs> LoadingRowComplete;

        public event EventHandler<RowCountEventArgs> RefreshData;
        public event EventHandler<SelectedControlArgs> SelectedControl;

        public event EventHandler ShowFindControl;

        #endregion

        #region Public Properties

        public static CoreDataGrid Current { get; set; }

        public IEnumerable DataSource
        {
            get
            {
                return this.Items;
            }
            set
            {
                if (value == null)
                {
                    this.Items.Clear();
                    this.dataSource = null;
                    this.defaulItemSource = null;
                    return;
                }
                this.countRowLoad = 0;
                this.countRowInGrid = 0;
                this.Items.Clear();
                if (this.PartProgressRing != null)
                {
                    this.PartProgressRing.IsActive = true;
                }
                this.defaulItemSource = null;
                // ItemsSource = value;
                if (this.LazyLoadItem == 0)
                {
                    foreach (object model in value)
                    {
                        this.Items.Add(model);
                    }
                }
                else
                {
                    int count = 0;
                    int countLoad = 0;
                    ObservableCollection<object> listModel = null;
                    foreach (object model in value)
                    {
                        if (count == 0)
                        {
                            listModel = new ObservableCollection<object>();
                        }
                        count++;
                        this.countRowInGrid++;
                        countLoad++;
                        listModel.Add(model);
                        ThreadPool.QueueUserWorkItem(
                            state => Manager.Timeout(
                                this.Dispatcher,
                                () =>
                                {
                                    if (this.PartTxtLoading != null)
                                    {
                                        this.PartTxtLoading.Text = state.ToString();
                                    }
                                }),
                            countLoad);

                        if (count == this.LazyLoadItem)
                        {
                            count = 0;
                            ThreadPool.QueueUserWorkItem(
                                state => Manager.Timeout(
                                    this.Dispatcher,
                                    () =>
                                    {
                                        Thread.Sleep(2);
                                        var stateItems = state as IEnumerable;
                                        if (stateItems != null)
                                        {
                                            foreach (object stateItem in stateItems)
                                            {
                                                this.Items.Add(stateItem);
                                            }
                                        }
                                    }),
                                listModel);

                            ;
                        }
                    }
                    if (count != 0)
                    {
                        ThreadPool.QueueUserWorkItem(
                            state => Manager.Timeout(
                                this.Dispatcher,
                                () =>
                                {
                                    var stateItems = state as IEnumerable;
                                    if (stateItems != null)
                                    {
                                        foreach (object stateItem in stateItems)
                                        {
                                            this.Items.Add(stateItem);
                                        }
                                    }
                                }),
                            listModel);
                    }
                }
                this.dataSource = value;
            }
        }

        public bool EditMode { get; set; }

        public int FocusInColum { get; set; }

        public bool FreezeRetrun { get; set; }

        public bool IsBusy
        {
            get
            {
                return this.PartProgressRing.IsActive;
            }
            set
            {
                if (this.PartProgressRing != null)
                {
                    this.PartProgressRing.IsActive = value;
                }
            }
        }

        public int LazyLoadItem { get; set; }

        public ScrollViewer PartScrollViewer { get; set; }

        //    }
        public TextBlock PartTxtLoading { get; set; }

        public bool UseContextMenuInHeader
        {
            get
            {
                return (bool)this.GetValue(UseContextMenuInHeaderProperty);
            }
            set
            {
                this.SetValue(UseContextMenuInHeaderProperty, value);
            }
        }

        public bool UseFindButton
        {
            get
            {
                return (bool)this.GetValue(UseFindButtonProperty);
            }
            set
            {
                this.SetValue(UseFindButtonProperty, value);
            }
        }

        #endregion

        #region Properties

        protected Button PartBtnFind { get; set; }

        protected ProgressRing PartProgressRing { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Add(object model)
        {
            this.AddChild(model);
        }

        //                }
        //                else base.OnKeyDown(e);
        //            }
        //            else base.OnKeyDown(e);
        //        }
        //        catch (Exception)
        //        {
        //        }
        public void AddItemSource(IEnumerable value)
        {
            int count = 0;
            int countLoad = 0;
            List<object> listModel = null;
            foreach (object model in value)
            {
                if (count == 0)
                {
                    listModel = new List<object>();
                }
                count++;
                countLoad++;
                this.countRowInGrid++;
                listModel.Add(model);
                ThreadPool.QueueUserWorkItem(
                    state => Manager.Timeout(this.Dispatcher, () => { this.PartTxtLoading.Text = state.ToString(); }),
                    countLoad);

                if (count == this.LazyLoadItem)
                {
                    count = 0;
                    ThreadPool.QueueUserWorkItem(
                        state => Manager.Timeout(
                            this.Dispatcher,
                            () =>
                            {
                                Thread.Sleep(2);
                                var stateItems = state as IEnumerable;
                                if (stateItems != null)
                                {
                                    foreach (object stateItem in stateItems)
                                    {
                                        this.Items.Add(stateItem);
                                    }
                                }
                            }),
                        listModel);

                    ;
                }
            }
            if (count != 0)
            {
                ThreadPool.QueueUserWorkItem(
                    state => Manager.Timeout(
                        this.Dispatcher,
                        () =>
                        {
                            var stateItems = state as IEnumerable;
                            if (stateItems != null)
                            {
                                foreach (object stateItem in stateItems)
                                {
                                    this.Items.Add(stateItem);
                                }
                            }
                        }),
                    listModel);
            }
        }

        public List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            var visualCollection = new List<T>();
            this.GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        public void OnAddRowAutomatic(EventArgs e)
        {
            EventHandler handler = this.AddRowAutomatic;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override void OnApplyTemplate()
        {
            this.PartProgressRing = this.GetTemplateChild(PART_ProgressBar) as ProgressRing;
            this.PartBtnFind = this.GetTemplateChild(Part_BtnFind) as Button;
            if (this.PartProgressRing != null)
            {
                this.PartProgressRing.IsActive = true;
            }
            this.PartTxtLoading = this.GetTemplateChild(PART_TextLoading) as TextBlock;
            this.PartScrollViewer = this.GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
            base.OnApplyTemplate();
            if (this.PartScrollViewer != null)
            {
                this.PartScrollViewer.ScrollChanged += this.PartScrollViewerOnScrollChanged;
            }
            if (this.PartBtnFind != null)
            {
                this.PartBtnFind.Click += this.PartBtnFindClick;
                //if (UseFindButton)
                //    PartBtnFind.Visibility = System.Windows.Visibility.Visible;
                //else
                //    PartBtnFind.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void OnDeletePress(EventArgs e)
        {
            EventHandler handler = this.DeletePress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnLoadingRowComplete(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.LoadingRowComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnRefreshData(RowCountEventArgs e)
        {
            EventHandler<RowCountEventArgs> handler = this.RefreshData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnSelectedControl(SelectedControlArgs e)
        {
            EventHandler<SelectedControlArgs> handler = this.SelectedControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnShowFindControl(EventArgs e)
        {
            EventHandler handler = this.ShowFindControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        internal void Refresh()
        {
            //var data = SelectedItem;
            //List<object> lastItems = new List<object>();
            //object lastItem = null;
            //foreach (var item in ItemsSource)
            //{
            //    lastItem = item;
            //    lastItems.Add(item);
            //}

            //if (data == lastItem || data.ToString().Equals("{NewItemPlaceholder}"))
            //{
            //    lastItems.Add(Activator.CreateInstance(data.GetType()));
            //  //  ItemsSource = lastItems;
            //    Manager.Timeout(Dispatcher, () =>
            //    {
            //        SelectedItem = data;
            //        Focus();
            //        OnAddRowAutomatic(EventArgs.Empty);
            //    });
            //}
        }

        protected override void OnExecutedBeginEdit(ExecutedRoutedEventArgs e)
        {
            //var a = e.OriginalSource as DataGridCell;
            //var cp = a.Column.GetCellContent(SelectedItem) as ContentPresenter;
            //var dp = Manager.FindVisualChild<FrameworkElement>(cp);
            //if (dp != null)
            //{
            //    dp.Focus();
            //}
            base.OnExecutedBeginEdit(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            Current = this;
            base.OnGotFocus(e);
        }

        private static void ChangeUseFindButton(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreDataGrid;
            if (form != null)
            {
                form.UseFindButton = (bool)e.NewValue;
            }
        }

        private void CheckCompleteLoadData()
        {
            this.countRowLoad++;
            //PartProgressRing.IsActive = false;
        }

        private void CoreDataGridKeyUp(object sender, KeyEventArgs e)
        {
            if (this.SelectedItem == null)
            {
                return;
            }
            if (this.CurrentColumn == null)
            {
                return;
            }
            this.BeginEdit(e);
            ContentPresenter cp = null;
            if (this.FocusInColum == -1)
            {
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

        private void CoreDataGridKeyDown(object sender, KeyEventArgs e)
        {
            if (this.SelectedItem == null)
            {
                return;
            }
            if (this.CurrentColumn == null)
            {
                return;
            }
            this.BeginEdit(e);
            ContentPresenter cp = null;
            if (this.FocusInColum == -1)
            {
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

        private void CoreDataGridLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.cxMenu = new ContextMenu();
                var parenMenuItem = new MenuItem();
                parenMenuItem.Header = "Show/Hide Column";
                this.cxMenu.Items.Add(parenMenuItem);
                foreach (DataGridColumn item in this.Columns)
                {
                    DataGridColumnHeader dgColHeader = this.GetColumnHeaderFromColumn(item);
                    if (dgColHeader == null)
                    {
                        continue;
                    }

                    if (this.UseContextMenuInHeader)
                    {
                        dgColHeader.ContextMenu = this.cxMenu;
                    }
                    var gridInTemplate = (Popup)dgColHeader.Template.FindName("popUp", dgColHeader);
                    dgColHeader.MouseRightButtonDown += (a, b) =>
                    {
                        (a as DataGridColumnHeader).ContextMenu = this.cxMenu;
                        //gridInTemplate.IsOpen = !gridInTemplate.IsOpen;
                        //gridInTemplate.Width = Manager.GetSize((a as DataGridColumnHeader)).Width;
                    };
                    // dgColHeader.SizeChanged += (a, b) =>
                    this.menuItem = new MenuItem();
                    this.menuItem.Header = item.Header;
                    this.menuItem.IsChecked = true;
                    parenMenuItem.Items.Add(this.menuItem);
                    this.menuItem.Click += this.menuItem_Click;
                    this.menuItem.Checked += this.menuItem_Checked;
                    this.menuItem.Unchecked += this.menuItem_Unchecked;
                }

                parenMenuItem = new MenuItem();
                parenMenuItem.Header = "Group By";
                this.cxMenu.Items.Add(parenMenuItem);
                foreach (DataGridColumn item in this.Columns)
                {
                    var aa = new DataGridTextColumn();
                    //aa.Binding

                    DataGridColumnHeader dgColHeader = this.GetColumnHeaderFromColumn(item);
                    if (dgColHeader == null)
                    {
                        continue;
                    }

                    if (this.UseContextMenuInHeader)
                    {
                        dgColHeader.ContextMenu = this.cxMenu;
                    }
                    var gridInTemplate = (Popup)dgColHeader.Template.FindName("popUp", dgColHeader);
                    dgColHeader.MouseRightButtonDown += (a, b) =>
                    {
                        (a as DataGridColumnHeader).ContextMenu = this.cxMenu;
                        //gridInTemplate.IsOpen = !gridInTemplate.IsOpen;
                        //gridInTemplate.Width = Manager.GetSize((a as DataGridColumnHeader)).Width;
                    };
                    // dgColHeader.SizeChanged += (a, b) =>
                    this.menuItem = new MenuItem();
                    var dataGridBoundColumn = item as DataGridBoundColumn;
                    if (dataGridBoundColumn != null)
                    {
                        var binding = dataGridBoundColumn.Binding as Binding;
                        if (binding != null)
                        {
                            this.menuItem.Tag = binding.Path.Path;
                        }
                    }
                    this.menuItem.Header = item.Header;
                    this.menuItem.IsChecked = false;
                    parenMenuItem.Items.Add(this.menuItem);
                    this.menuItem.Click += this.MenuItemGroupByOnClick;
                }
                this.menuItem = new MenuItem();
                this.menuItem.Header = "Search Data";
                this.cxMenu.Items.Add(this.menuItem);
                this.menuItem.Click += this.PartBtnFindClick;
            }
            catch (Exception)
            {
            }
        }

        private void CoreDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {

            //e.Row.
            var contextMenu = new ContextMenu();

            //this.menuItem = new MenuItem();
            //this.menuItem.Header = "Search Data";
            //this.menuItem.Click += this.PartBtnFindClick;
            //contextMenu.Items.Add(this.menuItem);
            //e.Row.ContextMenu = contextMenu;
            //Manager.Timeout(this.Dispatcher, () => this.OnLoadingRowComplete(e));
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(CultureInfo.InvariantCulture);
            //if (this.countRowLoad == 0)
            //{
            //    Manager.Timeout(this.Dispatcher, () => this.CheckCompleteLoadData());
            //}
        }

        private void CoreDataGridSpecialKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.OnDeletePress(EventArgs.Empty);
            }
        }

        private void CoreDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
        }

        private void CoreMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;

            while ((depObj != null) && !(depObj is DataGridColumnHeader))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }

            if (depObj == null)
            {
                return;
            }

            if (depObj is DataGridColumnHeader)
            {
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            if (cell == null)
            {
                cell = Manager.FindVisualParent<DataGridCell>(e.OriginalSource as UIElement);
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
        }

        private DataGridColumnHeader GetColumnHeaderFromColumn(DataGridColumn column)
        {
            // dataGrid is the name of your DataGrid. In this case Name="dataGrid"
            List<DataGridColumnHeader> columnHeaders = this.GetVisualChildCollection<DataGridColumnHeader>(this);
            foreach (DataGridColumnHeader columnHeader in columnHeaders)
            {
                if (columnHeader.Column == column)
                {
                    return columnHeader;
                }
            }
            return null;
        }

        private void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    this.GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        private void MenuItemGroupByOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = sender as MenuItem;
            if (item != null)
            {
                if (item.Header != null)
                {
                    if (item.Tag == null)
                    {
                        return;
                    }
                    if (!item.IsChecked)
                    {
                        this.hasGroupBy.Add(item.Tag.ToString());
                    }
                    else
                    {
                        this.hasGroupBy.Remove(item.Tag.ToString());
                    }
                    this.RefreshGroupData();
                    item.IsChecked = !item.IsChecked;
                }
            }
        }

        private void OnDataContextChanged(
            object sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        private void PartBtnFindClick(object sender, RoutedEventArgs e)
        {
            this.OnShowFindControl(EventArgs.Empty);
        }

        private void PartScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (this.countRowLoad == 0)
            {
                return;
            }

            if (e.VerticalOffset / this.PartScrollViewer.ScrollableHeight >= 0.89)
            {
                this.OnRefreshData(new RowCountEventArgs(this.countRowInGrid));
            }
            Items.Refresh();
        }

        private void RefreshGroupData()
        {
            ListCollectionView listCollectionView;
            if (this.ItemsSource == null)
            {
                List<object> data = this.Items.Cast<object>().ToList();
                this.defaulItemSource = data;
                listCollectionView = new ListCollectionView(data);
                this.Items.Clear();
            }
            else
            {
                if (this.defaulItemSource == null)
                {
                    this.defaulItemSource = this.ItemsSource.Cast<object>().ToList();
                    listCollectionView = new ListCollectionView(this.ItemsSource.Cast<object>().ToList());
                }
                else
                {
                    listCollectionView = new ListCollectionView(this.defaulItemSource);
                }
            }
            foreach (string groupBy in this.hasGroupBy)
            {
                if (listCollectionView.GroupDescriptions != null)
                {
                    listCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(groupBy));
                }
            }
            this.ItemsSource = listCollectionView;
        }

        private void menuItem_Checked(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            foreach (DataGridColumn column in this.Columns)
            {
                if (column.Header == null)
                {
                    continue;
                }
                if (column.Header.ToString().Contains(item.Header.ToString()))
                {
                    column.Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item.IsChecked)
            {
                item.IsChecked = false;
            }
            else
            {
                item.IsChecked = true;
            }
        }

        private void menuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            foreach (DataGridColumn column in this.Columns)
            {
                if (column.Header == null)
                {
                    continue;
                }
                if (column.Header.ToString().Contains(item.Header.ToString()))
                {
                    column.Visibility = Visibility.Collapsed;
                    break;
                }
            }
        }

        #endregion

        //public new IEnumerable ItemsSource
        //{
        //    get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        //public new static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CoreDataGrid), new UIPropertyMetadata(ItemsSourceChange));

        //private static void ItemsSourceChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    var form = dependencyObject as CoreDataGrid;
        //    if (form != null)
        //    {
        //        var value = dependencyPropertyChangedEventArgs.NewValue as IEnumerable;
        //        if (value == null) return;
        //        var itemsSource = value as object[] ?? value.Cast<object>().ToArray();
        //        form.ItemsSource = itemsSource;
        //        form.Items.Clear();
        //        if (form.LazyLoadItem == 0)
        //        {
        //            foreach (var model in itemsSource)
        //            {
        //                form.Items.Add(model);
        //            }
        //        }
        //        else
        //        {
        //            var count = 0;
        //            var countLoad = 0;
        //            List<object> listModel = null;
        //            foreach (var model in value)
        //            {
        //                if (count == 0)
        //                    listModel = new List<object>();
        //                count++;
        //                countLoad++;
        //                listModel.Add(model);
        //                ThreadPool.QueueUserWorkItem(state => Manager.Timeout(form.Dispatcher, () =>
        //                {
        //                    form.PartTxtLoading.Text = state.ToString();
        //                }), countLoad);

        //                if (count == form.LazyLoadItem)
        //                {
        //                    count = 0;
        //                    ThreadPool.QueueUserWorkItem(state => Manager.Timeout(form.Dispatcher, () =>
        //                    {
        //                        Thread.Sleep(2);
        //                        var stateItems = state as IEnumerable;
        //                        if (stateItems != null)
        //                            foreach (var stateItem in stateItems)
        //                            {
        //                                form.Items.Add(stateItem);
        //                            }
        //                    }), listModel);
        //                }
        //            }
        //            if (count != 0)
        //                ThreadPool.QueueUserWorkItem(state => Manager.Timeout(form.Dispatcher, () =>
        //                {
        //                    var stateItems = state as IEnumerable;
        //                    if (stateItems != null)
        //                        foreach (var stateItem in stateItems)
        //                        {
        //                            form.Items.Add(stateItem);
        //                        }
        //                }), listModel);
        //        }
        //    }
        //}

        //    protected override void OnKeyDown(KeyEventArgs e)
        //    {
        //        try
        //        {

        //            //if (EditMode)
        //            //    BeginEdit(e);
        //            if (FreezeRetrun && EditMode)
        //            {
        //                if (e.Key == Key.Return)
        //                {
        //                    e.Handled = true;
        //                    int index = 0;
        //                    foreach (var dataGridColumn in Columns)
        //                    {
        //                        if (CurrentColumn == dataGridColumn)
        //                        {
        //                            break;
        //                        }
        //                        index++;
        //                    }
        //                    object nextItem = null;
        //                    var cek = false;
        //                    foreach (var model in ItemsSource)
        //                    {
        //                        if (cek)
        //                        {
        //                            nextItem = model;
        //                            break;
        //                        }
        //                        if (model == SelectedItem)
        //                            cek = true;

        //                    }
        //                    index = index + 1;
        //                    if (Columns.Count + 1 == index + 1)
        //                    {
        //                        index = 0;
        //                        if (nextItem != null)
        //                        {
        //                            e.Handled = false;
        //                            base.OnKeyDown(e);
        //                            var cp = Columns[index].GetCellContent(nextItem) as ContentPresenter;
        //                            var dp = Manager.FindVisualChild<FrameworkElement>(cp);
        //                            if (dp != null)
        //                            {
        //                                dp.Focus();
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var cp = Columns[index].GetCellContent(SelectedItem) as ContentPresenter;
        //                        var dp = Manager.FindVisualChild<FrameworkElement>(cp);
        //                        if (dp != null)
        //                        {
        //                            dp.Focus();
        //                        }
        //                    }
    }
}