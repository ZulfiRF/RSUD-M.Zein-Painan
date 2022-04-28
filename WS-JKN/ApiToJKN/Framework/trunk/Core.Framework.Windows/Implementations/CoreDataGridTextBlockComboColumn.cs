using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Model.Attr;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Implementations.InputGrid;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridTextBlockComboColumn : DataGridComboBoxColumn, IMultipleHeader
    {
        //public event KeyEventHandler TextBoxKeyDown;


        public event SelectionChangedEventHandler SelectionChanged;

        public bool DisableAutoSearchWhenSelectedItem { get; set; }


        private List<DataGridCell> listCellLoad;
        private bool ChangeFromLocal { get; set; }

        private bool HasChangeCombo { get; set; }

        protected virtual void OnSelectionChanged(CoreComboBoxGrid ctrl, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(ctrl, e);
            }
        }

        #region Static Fields

        //public static readonly DependencyProperty DisplayMemberPathProperty =
        //    DependencyProperty.Register(
        //        "DisplayMemberPath",
        //        typeof(string),
        //        typeof(CoreDataGridTextBlockComboColumn),
        //        new UIPropertyMetadata(null));

        public static readonly DependencyProperty DomainNameSpacesProperty =
            DependencyProperty.Register(
                "DomainNameSpaces",
                typeof(string),
                typeof(CoreDataGridTextBlockComboColumn),
                new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for UsingSearchByFramework.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsingSearchByFrameworkProperty =
            DependencyProperty.Register(
                "UsingSearchByFramework",
                typeof(bool),
                typeof(CoreDataGridTextBlockComboColumn),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath",
            typeof(string),
            typeof(CoreDataGridTextBlockComboColumn),
            new UIPropertyMetadata(null));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set
            { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CoreDataGridTextBlockComboColumn), new UIPropertyMetadata(true));



        #endregion Static Fields

        #region Public Properties

        //public string DisplayMemberPath
        //{
        //    get { return (string)GetValue(DisplayMemberPathProperty); }
        //    set { SetValue(DisplayMemberPathProperty, value); }
        //}

        public string DomainNameSpaces
        {
            get { return (string)GetValue(DomainNameSpacesProperty); }
            set { SetValue(DomainNameSpacesProperty, value); }
        }

        public bool UsingSearchByFramework
        {
            get { return (bool)GetValue(UsingSearchByFrameworkProperty); }
            set { SetValue(UsingSearchByFrameworkProperty, value); }
        }

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        private Type domainType;

        public Type DomainType
        {
            get { return domainType; }
            set
            {
                domainType = value;
                if (domainType == null) return;
                DomainNameSpaces = value.FullName;
                UsingSearchByFramework = true;
            }
        }

        private List<object> itemsSource = new List<object>();
        public new IEnumerable ItemsSource
        {
            get
            {
                return itemsSource;
            }
            set
            {
                itemsSource.Clear();
                if (value != null)
                    foreach (var source in value)
                    {
                        //if (itemsSource.All(n => n.Key != source.ToString()))
                        itemsSource.Add(source);
                    }
            }
        }

        public void Focus(object dataItem = null)
        {

            Manager.Timeout(Dispatcher, () =>
            {
                if (dataItem == null)
                {
                    var firstOrDefault = listCellLoad.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        while (!firstOrDefault.IsFocused)
                        {
                            Thread.Sleep(2);
                            firstOrDefault.Focus();
                        }
                    }
                }
                else
                    foreach (var dataGridCell in listCellLoad.Where(n => n.IsLoaded))
                    {
                        if (dataGridCell.DataContext != null)
                        {
                            if (dataGridCell.DataContext.Equals(dataItem))
                            {
                                while (!dataGridCell.IsFocused)
                                {
                                    Thread.Sleep(2);
                                    dataGridCell.Focus();
                                }
                                this.DataGridOwner.SelectedItem = dataItem;
                                break;
                            }
                        }
                    }
            });
            listCellLoad = (listCellLoad.Where(n => n.IsLoaded)).ToList();
        }
        #endregion Public Properties

        #region Methods

        private readonly CoreDictionary<LabelInfo, CoreComboBoxGrid> dictionary = new CoreDictionary<LabelInfo, CoreComboBoxGrid>();

        private readonly CoreDictionary<object, DataGridCell> dictionaryControlCells =
            new CoreDictionary<object, DataGridCell>();

        private DataGridCell currentCell;

        public bool DisableAutoSearch { get; set; }



        public IEnumerable ItemsSourceLocal { get; set; }



        public void FocusControl()
        {
            foreach (var dataGridCell in dictionaryControlCells)
            {
                dataGridCell.Value.Focus();
                break;
            }
        }

        CoreComboBoxGrid combo = null;
        public CoreComboBoxGrid GetContent(object dataItem)
        {
            if (dataItem == null)
                return null;
            var controlInGrid = GetCellContent(dataItem);
            if (controlInGrid is LabelInfo)
                combo = (controlInGrid as LabelInfo).Tag as CoreComboBoxGrid;
            else
                combo = GetCellContent(dataItem) as CoreComboBoxGrid;

            if (combo == null)
                combo = dictionary.FirstOrDefault(n => n.Key.Equals((controlInGrid as LabelInfo).Text)).Value;
            return combo;
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            bool value = base.CommitCellEdit(editingElement);
            return value;
        }
        private void ApplyBinding(BindingBase binding, CoreComboBoxGrid target, DependencyProperty property)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
            }
        }
        private void DataGridOwnerOnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is INotifyPropertyChanged)
                (e.Row.Item as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
        }
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            this.DataGridOwner.PreviewKeyDown -= HandleReturn;
            if (listCellLoad == null)
                listCellLoad = new List<DataGridCell>();
            if (!listCellLoad.Any(n => n.Equals(cell)))
                listCellLoad.Add(cell);
            cell.GotFocus += (sender, args) =>
            {
                cell.Background = new SolidColorBrush(Colors.RoyalBlue);
            };
            cell.LostFocus += (sender, args) =>
            {
                cell.Background = new SolidColorBrush(Colors.Transparent);
            };
            cell.KeyDown += CellOnKeyDown;
            var label = new LabelInfo();
            if (dataItem != null)
            {
                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                    dataItem,
                    this.SelectedValuePath);
                if (propertyValuePath != null)
                {
                    //combo.Value = propertyValuePath.ToString();
                    var propertyReference = dataItem.GetType().GetProperty(SelectedValuePath).GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute) as ReferenceAttribute;
                    if (propertyReference != null)
                    {
                        var result = HelperManager.BindPengambilanObjekDariSource(
                            dataItem.GetType().GetProperty(propertyReference.Property).GetValue(dataItem, null),
                            DisplayMemberPath);
                        label.Text = result != null ? result.ToString() : null;
                    }
                    else
                    {
                        if (dataItem != null)
                        {
                            if (dataItem.GetType().GetProperty(DisplayMemberPath) != null)
                            {
                                var result = dataItem.GetType().GetProperty(DisplayMemberPath).GetValue(dataItem, null);
                                label.Text = result != null ? result.ToString() : null;
                            }
                            else
                            {
                                var result = dataItem.GetType().GetProperty(SelectedValuePath).GetValue(dataItem, null);
                                label.Text = result != null ? result.ToString() : null;
                            }
                        }
                    }
                }

                #region Generate ComboBox

                //if (!dictionary.Keys.Any(n => n.Guid.Equals(label.Guid)))
                if (!dictionary.ContainsKey(label))
                {
                    var combo = new CoreComboBoxGrid();
                    if (DomainType != null)
                        combo.DomainType = DomainType;
                    combo.IsEnabled = IsEnabled;
                    combo.Loaded += ComboOnLoaded;
                    combo.LostFocus += ComboOnGotFocus;
                    combo.Temp = dataItem;
                    ApplyBinding(this.SelectedItemBinding, combo, Selector.SelectedItemProperty);
                    ApplyBinding(this.SelectedValueBinding, combo, CoreComboBoxGrid.SelectedValueProperty);
                    ApplyBinding(this.TextBinding, combo, CoreComboBoxGrid.TextProperty);
                    if (this.TextBinding != null)
                    {
                        BindingOperations.SetBinding(label, LabelInfo.TextProperty, this.TextBinding);
                    }
                    else
                    {
                        BindingOperations.ClearBinding(label, LabelInfo.TextProperty);
                    }
                    combo.GotFocus += (sender, args) =>
                                          {
                                              if (DataGridOwner is InsertDataGrid)
                                                  (DataGridOwner as InsertDataGrid).FreezeKeyDown = true;
                                              DataGridOwner.CurrentColumn = this;
                                              DataGridOwner.SelectedItem = combo.Temp;
                                              //OnInitializeComboBox(
                                              //    new ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>(
                                              //        new KeyValuePair<object, CoreComboBoxGrid>(combo.Temp, combo)));
                                          };

                    object content = cell.Content;
                    var children = Manager.FindVisualChild<LabelInfo>(content as FrameworkElement);
                    combo.Height = cell.Height;
                    combo.Style = Application.Current.Resources["MetroComboBoxGrid"] as Style;
                    combo.Height = cell.Height;
                    combo.DisableAutoSearchWhenSelectedItem = DisableAutoSearchWhenSelectedItem;
                    combo.DisplayMemberPath = DisplayMemberPath;
                    combo.UsingSearchByFramework = UsingSearchByFramework;
                    combo.DomainNameSpaces = DomainNameSpaces;
                    combo.ValuePath = string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath;
                    //combo.DisableSearch = DisableAutoSearch;
                    combo.Tag = children;

                    var parent = Manager.FindVisualChild<LabelInfo>(cell as FrameworkElement);
                    if (parent != null) parent.Tag = combo;

                    if (ItemsSource != null && !UsingSearchByFramework)
                        combo.ItemsSource = ItemsSource;
                    //if (!UsingSearchByFramework)
                    //    combo.ItemsSource = ItemsSource;
                    combo.KeyDown += (sender, args) =>
                                         {
                                             if (args.Key == Key.Return)
                                             {
                                                 cell.IsEditing = false;
                                                 Manager.Timeout(Dispatcher, () =>
                                                                             OnChangeState(
                                                                                 new ItemEventArgs<StateInfo>(
                                                                                     StateInfo.Commit))
                                                     );

                                             }
                                         };
                    combo.SelectionChanged += ComboOnSelectionChanged;
                    this.DataGridOwner.KeyDown += DataGridOwnerOnKeyDown;

                    combo.Loaded += (sender, args) =>
                                        {
                                            Manager.Timeout(Dispatcher, () =>
                                                                            {
                                                                                combo.FocusControl();
                                                                            });
                                        };

                    #endregion

                    label.Tag = combo;
                    OnInitializeComboBox(new ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>(
                        new KeyValuePair<object, CoreComboBoxGrid>(combo.Temp, combo)));
                    dictionary.Add(label, combo);
                }
            }

            if (dataItem is INotifyPropertyChanged)
            {
                (dataItem as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            }
            EventHandler<ItemEventArgs<object>> handler = Initialize;

            if (handler != null) handler(cell, new ItemEventArgs<object>(dataItem));
            Manager.Timeout(Dispatcher, () => OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Info)));
            return label;
        }

        //public LabelInfo CurrentLabel { get; set; }

        private void CellOnKeyDown(object sender, KeyEventArgs args)
        {
            var cell = sender as DataGridCell;
            if (args.Key == Key.Return)
            {
                if (cell != null) cell.IsEditing = true;
                OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Edit));
                args.Handled = true;
                this.DataGridOwner.PreviewKeyDown += HandleReturn;
                currentCell = cell;
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            IEnumerable source = null;
            var data = dictionary.FirstOrDefault(n => n.Key.Guid.Equals((cell.Content as LabelInfo).Guid) && n.Value.ItemsSource != null).Value;
            if (data != null)
                source = data.ItemsSource;

            if (this.combo != null)
            {
                if (this.combo.ItemsSource.Cast<object>().Any())
                    source = this.combo.ItemsSource;
            }

            cell.KeyDown -= CellOnKeyDown;
            CoreComboBoxGrid combo;
            if (listCellLoad == null)
                listCellLoad = new List<DataGridCell>();
            listCellLoad.Add(cell);
            //if (!dictionary.TryGetValue(dataItem, out combo))
            {
                combo = new CoreComboBoxGrid();
                combo.IsEnabled = IsEnabled;
                combo.Loaded += ComboOnLoaded;
                combo.LostFocus += ComboOnGotFocus;
                combo.KeyDown += delegate (object sender, KeyEventArgs args) { OnKeyDown(sender, args); };

                combo.Temp = dataItem;
                ApplyBinding(this.SelectedItemBinding, combo, Selector.SelectedItemProperty);
                ApplyBinding(this.SelectedValueBinding, combo, CoreComboBoxGrid.SelectedValueProperty);
                ApplyBinding(this.TextBinding, combo, CoreComboBoxGrid.TextProperty);
                combo.GotFocus += (sender, args) =>
                {
                    if (DataGridOwner is InsertDataGrid)
                        (DataGridOwner as InsertDataGrid).FreezeKeyDown = true;
                    DataGridOwner.CurrentColumn = this;
                    DataGridOwner.SelectedItem = combo.Temp;
                    //OnInitializeComboBox(new ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>(new KeyValuePair<object, CoreComboBoxGrid>(combo.Temp, combo)));
                };
                //OnBindingItemSource(new ItemEventArgs<CoreComboBoxGrid>(combo));
                //combo.SelectionChanged += (sender, args) => OnSelectionChanged(combo, args);
                object content = cell.Content;
                var children = Manager.FindVisualChild<LabelInfo>(content as FrameworkElement);
                combo.Height = cell.Height;
                combo.Style = Application.Current.Resources["MetroComboBoxGrid"] as Style;
                combo.Height = cell.Height;
                combo.DisableAutoSearchWhenSelectedItem = DisableAutoSearchWhenSelectedItem;
                combo.DisplayMemberPath = DisplayMemberPath;
                combo.UsingSearchByFramework = UsingSearchByFramework;
                combo.DomainNameSpaces = DomainNameSpaces;
                combo.ValuePath = string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath;
                //combo.DisableSearch = DisableAutoSearch;
                combo.Tag = children;

                var parent = Manager.FindVisualChild<LabelInfo>(cell as FrameworkElement);
                if (parent != null) parent.Tag = combo;

                if (source != null) combo.ItemsSource = source;
                else if (ItemsSource != null && !UsingSearchByFramework)
                    combo.ItemsSource = ItemsSource;
                //if (!UsingSearchByFramework)
                //    combo.ItemsSource = ItemsSource;                
                combo.KeyDown += (sender, args) =>
                {
                    if (args.Key == Key.Return)
                    {
                        cell.IsEditing = false;
                        Manager.Timeout(Dispatcher, () =>
                                                            OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit))
                            );

                    }
                };

                //combo.LostFocus += (sender, args) => canHide = true;
                combo.SelectionChanged += ComboOnSelectionChanged;
                this.DataGridOwner.KeyDown += DataGridOwnerOnKeyDown;

                combo.Loaded += (sender, args) =>
                {
                    Manager.Timeout(Dispatcher, () =>
                    {
                        combo.FocusControl();
                    });
                };
                OnInitializeComboBox(new ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>(new KeyValuePair<object, CoreComboBoxGrid>(combo.Temp, combo)));

                //combo = data;
                //       dictionary.Add(dataItem, combo);
            }
            return combo;//base.GenerateEditingElement(cell, dataItem);
        }

        private void HandleReturn(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                if (currentCell != null)
                {
                    var combo = currentCell.Content as CoreComboBoxGrid;
                    DataGrid parent = DataGridOwner;
                    if (parent != null)
                    {
                        parent.CommitEdit();
                    }
                    var comboLocal = combo;
                    var gridCell = currentCell;
                    if (gridCell != null)
                    {
                        HasChangeCombo = true;
                        PropertyInfo propertyValuePath = gridCell.DataContext.GetType().GetProperty(SelectedValuePath);
                        if (propertyValuePath != null)
                        {
                            if (comboLocal != null)
                            {
                                if (comboLocal.SelectedItem != null)
                                {
                                    if (comboLocal.ValuePath != null)
                                    {
                                        PropertyInfo prop =
                                            comboLocal.SelectedItem.GetType().GetProperty(comboLocal.ValuePath);
                                        if (prop != null)
                                        {
                                            if (!ChangeFromLocal)
                                            {
                                                object propertyValueSelected = prop.GetValue(comboLocal.SelectedItem,
                                                    null);
                                                comboLocal.ValueTemp = propertyValueSelected;
                                                propertyValuePath.SetValue(gridCell.DataContext, propertyValueSelected,
                                                    null);
                                            }
                                            ThreadPool.QueueUserWorkItem(delegate
                                            {
                                                Thread.Sleep(100);
                                                ChangeFromLocal = false;
                                            });
                                        }
                                        var labelInfo = gridCell.Content as LabelInfo;
                                        if (labelInfo != null)
                                        {
                                            var dataItem = gridCell.DataContext;
                                            if (dataItem != null)
                                            {
                                                object propertyValuePathLabel = HelperManager.BindPengambilanObjekDariSource(
                                                    dataItem,
                                                    this.SelectedValuePath);
                                                if (propertyValuePathLabel != null)
                                                {
                                                    //combo.Value = propertyValuePath.ToString();
                                                    var propertyReference = dataItem.GetType().GetProperty(SelectedValuePath).GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute) as ReferenceAttribute;
                                                    if (propertyReference != null)
                                                    {
                                                        var result = HelperManager.BindPengambilanObjekDariSource(
                                                            dataItem.GetType().GetProperty(propertyReference.Property).GetValue(dataItem, null),
                                                            DisplayMemberPath);
                                                        labelInfo.Text = result != null ? result.ToString() : null;
                                                    }
                                                    //var propertyReference = comboLocal.GetType().GetProperty(SelectedValuePath).GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute) as ReferenceAttribute;
                                                    //if (propertyReference != null)
                                                    //{
                                                    //    var result = HelperManager.BindPengambilanObjekDariSource(
                                                    //        dataItem.GetType().GetProperty(propertyReference.Property).GetValue(dataItem, null),
                                                    //        DisplayMemberPath);
                                                    //    labelInfo.Text = result != null ? result.ToString() : null;
                                                    //}

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //OnSelectionChanged(sender as CoreComboBoxGrid, null);
                    OnSelectionChanged(combo as CoreComboBoxGrid, null);
                    currentCell.IsEditing = false;
                }
                Manager.Timeout(Dispatcher, () => OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit)));
            }
        }

        //protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        //{
        //    if (!firstLoad)
        //    {
        //        DataGridOwner.LoadingRow += DataGridOwnerOnLoadingRow;
        //        DataGridOwner.SelectedCellsChanged += DataGridOwnerOnSelectedCellsChanged;
        //        firstLoad = true;
        //    }

        //    cell.GotFocus += (sender, args) => cell.Background = new SolidColorBrush(Colors.RoyalBlue);
        //    cell.LostFocus += (sender, args) => cell.Background = new SolidColorBrush(Colors.Transparent);
        //    //DataGridCell obj;
        //    //if (!dictionaryControlCells.TryGetValue(dataItem, out obj))
        //    //    dictionaryControlCells.Add(dataItem, cell);
        //    CoreComboBoxGrid combo;
        //    if (listCellLoad == null)
        //        listCellLoad = new List<DataGridCell>();
        //    listCellLoad.Add(cell);
        //    //if (!dictionary.TryGetValue(dataItem, out combo))
        //    {
        //        combo = new CoreComboBoxGrid();
        //        combo.IsEnabled = IsEnabled;
        //        combo.Loaded += ComboOnLoaded;
        //        combo.LostFocus += ComboOnGotFocus;
        //        combo.Temp = dataItem;
        //        ApplyBinding(this.SelectedItemBinding, combo, Selector.SelectedItemProperty);
        //        ApplyBinding(this.SelectedValueBinding, combo, CoreComboBoxGrid.SelectedValueProperty);
        //        ApplyBinding(this.TextBinding, combo, CoreComboBoxGrid.TextProperty);
        //        combo.GotFocus += (sender, args) =>
        //        {
        //            if (DataGridOwner is InsertDataGrid)
        //                (DataGridOwner as InsertDataGrid).FreezeKeyDown = true;
        //            DataGridOwner.CurrentColumn = this;
        //            DataGridOwner.SelectedItem = combo.Temp;
        //        };
        //        //OnBindingItemSource(new ItemEventArgs<CoreComboBoxGrid>(combo));
        //        //combo.SelectionChanged += (sender, args) => OnSelectionChanged(combo, args);
        //        object content = cell.Content;
        //        var children = Manager.FindVisualChild<LabelInfo>(content as FrameworkElement);
        //        combo.Height = cell.Height;
        //        combo.Style = Application.Current.Resources["MetroComboBoxGrid"] as Style;
        //        combo.Height = cell.Height;
        //        combo.DisableAutoSearchWhenSelectedItem = DisableAutoSearchWhenSelectedItem;
        //        combo.DisplayMemberPath = DisplayMemberPath;
        //        combo.UsingSearchByFramework = UsingSearchByFramework;
        //        combo.DomainNameSpaces = DomainNameSpaces;
        //        combo.ValuePath = string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath;
        //        //combo.DisableSearch = DisableAutoSearch;
        //        combo.Tag = children;

        //        if (!UsingSearchByFramework)
        //            combo.ItemsSource = ItemsSource;
        //        combo.KeyUp += (sender, args) =>
        //        {
        //            if (args.Key == Key.Return)
        //            {
        //                LoadDefaultStateContent(cell);
        //                OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit));
        //                ThreadPool.QueueUserWorkItem(delegate
        //                {
        //                    Manager.Timeout(Dispatcher, () =>
        //                    {
        //                        while (!((DataGridCell)cell).IsFocused)
        //                        {
        //                            Thread.Sleep(2);
        //                            ((DataGridCell)cell).Focus();
        //                        }
        //                    });

        //                });
        //            }
        //        };
        //        //combo.LostFocus += (sender, args) => canHide = true;
        //        combo.SelectionChanged += ComboOnSelectionChanged;
        //        //       dictionary.Add(dataItem, combo);
        //    }

        //    if (combo.Parent != null)
        //    {
        //        var dataGridCell = combo.Parent as DataGridCell;
        //        if (dataGridCell != null) dataGridCell.Content = null;
        //    }
        //    var LabelInfo2 = new LabelInfo();
        //    if (dataItem != null)
        //    {
        //        object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
        //            dataItem,
        //            this.SelectedValuePath);
        //        if (propertyValuePath != null)
        //        {
        //            //combo.Value = propertyValuePath.ToString();
        //            var propertyReference = dataItem.GetType().GetProperty(SelectedValuePath).GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute) as ReferenceAttribute;
        //            if (propertyReference != null)
        //            {
        //                var result = HelperManager.BindPengambilanObjekDariSource(
        //                    dataItem.GetType().GetProperty(propertyReference.Property).GetValue(dataItem, null),
        //                    DisplayMemberPath);
        //                LabelInfo2.Text = result != null ? result.ToString() : null;
        //            }

        //        }
        //    }


        //    if (dataItem is INotifyPropertyChanged)
        //    {
        //        (dataItem as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
        //    }

        //    PropertyInfo property = cell.GetType()
        //        .GetProperty(
        //            "EditingElement",
        //            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public
        //            | BindingFlags.Instance);
        //    if (property != null)
        //    {
        //        var labelInfo = (property.GetValue(cell, null) as LabelInfo);
        //        if (labelInfo != null)
        //        {
        //            PropertyInfo comboProperty = combo.GetType()
        //                .GetProperty(
        //                    "CurrentText",
        //                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic
        //                    | BindingFlags.Public | BindingFlags.Instance);
        //            if (comboProperty != null)
        //            {
        //                comboProperty.SetValue(combo, labelInfo.Text, null);
        //            }
        //            comboProperty = combo.GetType()
        //                .GetProperty(
        //                    "CurrentItem",
        //                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic
        //                    | BindingFlags.Public | BindingFlags.Instance);
        //            if (comboProperty != null)
        //            {
        //                comboProperty.SetValue(combo, labelInfo.Tag, null);
        //            }
        //        }
        //    }
        //    if (dataItem != null)
        //    {
        //        BindValueToLabelInfo(dataItem, combo, LabelInfo2);
        //    }
        //    LabelInfo2.Tag = combo;
        //    cell.PreviewKeyUp += LabelInfo2OnKeyUp;
        //    cell.PreviewKeyUp += (sender, args) => OnPreviewKeyUp(sender, args);
        //    combo.Tag = LabelInfo2;
        //    LabelInfo2.DataContext = dataItem;
        //    if (dataItem != null)
        //    {
        //        if (dataItem.GetType().GetProperty(this.DisplayMemberPath) != null)
        //            if (dataItem.GetType().GetProperty(this.DisplayMemberPath).CanWrite)
        //                BindingOperations.SetBinding(LabelInfo2, LabelInfo.TextProperty, new Binding(this.DisplayMemberPath)
        //                {
        //                    Mode = BindingMode.TwoWay,
        //                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        //                });
        //            else
        //                BindingOperations.SetBinding(LabelInfo2, LabelInfo.TextProperty, new Binding(this.DisplayMemberPath)
        //                {
        //                    Mode = BindingMode.OneWay,
        //                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        //                });
        //    }
        //    EventHandler<ItemEventArgs<object>> handler = Initialize;
        //    if (handler != null) handler(combo, new ItemEventArgs<object>(dataItem));
        //    this.DataGridOwner.PreviewKeyDown += DataGridOwnerOnPreviewKeyDown;
        //    return LabelInfo2;
        //}

        private void DataGridOwnerOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //if (!canPressReturn)
                //    e.Handled = true;
                //else 

                if (e.OriginalSource is DataGridCell)
                {
                    var textBox = e.OriginalSource as DataGridCell;
                    currentCell = textBox;
                    //CanPressReturn = false;
                    //BindingValueFromTextBox(textBox.Tag, textBox, textBox);

                    //BindValueToLabelInfo(coreTextBox.DataContext, coreTextBox, LabelInfo2);
                    LoadDefaultStateContent(currentCell);
                    e.Handled = true;
                }
                else
                {

                }

            }
        }
        private void ComboOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            DataGrid parent = DataGridOwner;
            if (parent != null)
            {
                parent.CommitEdit();
            }
            var comboLocal = sender as CoreComboBoxGrid;
            var gridCell = comboLocal.Parent as DataGridCell;
            if (gridCell != null)
            {
                HasChangeCombo = true;
                PropertyInfo propertyValuePath = gridCell.DataContext.GetType().GetProperty(SelectedValuePath);
                if (propertyValuePath != null)
                {
                    if (comboLocal != null)
                    {
                        if (comboLocal.SelectedItem != null)
                        {
                            if (comboLocal.ValuePath != null)
                            {
                                PropertyInfo prop =
                                    comboLocal.SelectedItem.GetType().GetProperty(comboLocal.ValuePath);
                                if (prop != null)
                                {
                                    if (!ChangeFromLocal)
                                    {
                                        object propertyValueSelected = prop.GetValue(comboLocal.SelectedItem, null);
                                        comboLocal.ValueTemp = propertyValueSelected;
                                        propertyValuePath.SetValue(gridCell.DataContext, propertyValueSelected, null);
                                    }
                                    ThreadPool.QueueUserWorkItem(delegate
                                    {
                                        Thread.Sleep(100);
                                        ChangeFromLocal = false;
                                    });
                                }
                                if (comboLocal.Tag != null)
                                {
                                    var labelInfo = comboLocal.Tag as LabelInfo;
                                    if (labelInfo != null)
                                    {
                                        //if (changeFromLocal)
                                        {
                                            var dataGridCell = comboLocal.Parent as DataGridCell;
                                            if (dataGridCell != null)
                                            {
                                                //dataGridCell.Content = (comboLocal.Tag as LabelInfo);
                                                gridCell.IsEditing = false;
                                                OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit));
                                                //ThreadPool.QueueUserWorkItem(delegate(object state)
                                                //{

                                                //    Manager.Timeout(Dispatcher, () =>
                                                //    {
                                                //        LoadDefaultStateContent(state as DataGridCell);
                                                //        while (!((DataGridCell)state).IsFocused)
                                                //        {
                                                //            Thread.Sleep(2);
                                                //            ((DataGridCell)state).Focus();
                                                //        }
                                                //    });
                                                //}, dataGridCell);
                                            }
                                        }
                                        labelInfo.Text = comboLocal.Text;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            OnSelectionChanged(sender as CoreComboBoxGrid, args);
        }
        public event EventHandler<ItemEventArgs<object>> Initialize;
        public event KeyEventHandler PreviewKeyUp;
        private void DataGridOwnerOnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs selectedCellsChangedEventArgs)
        {
            //if (currentCell != null && canHide)
            //{
            //    LoadDefaultStateContent(currentCell);
            //    currentCell = null;
            //}
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Manager.Timeout(Dispatcher, () =>
                                            {
                                                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(sender, this.SelectedValuePath);
                                                if (propertyValuePath != null)
                                                {
                                                    var item = this.GetCellContent(sender) as LabelInfo;
                                                    if (item != null)
                                                    {
                                                        var dataItem = item.DataContext;
                                                        var labelInfo2 = item;
                                                        PropertyInfo property;
                                                        property = dataItem.GetType().GetProperty(string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath);
                                                        if (property != null)
                                                        {
                                                            object reference = property.GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute);
                                                            if (reference != null)
                                                            {
                                                                var referenceAttribute = reference as ReferenceAttribute;
                                                                if (referenceAttribute != null)
                                                                {
                                                                    property = dataItem.GetType().GetProperty(referenceAttribute.Property);
                                                                    object result =
                                                                        HelperManager.BindPengambilanObjekDariSource(property.GetValue(dataItem, null),
                                                                            DisplayMemberPath);

                                                                    Dispatcher.Invoke((ThreadStart)delegate ()
                                                                    {
                                                                        {

                                                                            if (labelInfo2 != null)
                                                                                labelInfo2.Text = (result == null ? "" : result.ToString());
                                                                        }
                                                                    }, DispatcherPriority.Render);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                object result =
                                                                    HelperManager.BindPengambilanObjekDariSource(dataItem,
                                                                        DisplayMemberPath);

                                                                Dispatcher.Invoke((ThreadStart)delegate ()
                                                                {
                                                                    if (labelInfo2 != null)
                                                                        labelInfo2.Text = (result == null ? "" : result.ToString());
                                                                }, DispatcherPriority.Render);

                                                            }
                                                        }
                                                    }
                                                }

                                            });
        }

        private void BindValueToLabelInfo(object dataItem, CoreComboBoxGrid combo, LabelInfo LabelInfo2)
        {
            PropertyInfo property;
            property = dataItem.GetType().GetProperty(string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath);
            if (property != null)
            {
                combo.ValueTemp = property.GetValue(dataItem, null);
                object reference = property.GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute);
                if (reference != null)
                {
                    var referenceAttribute = reference as ReferenceAttribute;
                    if (referenceAttribute != null)
                    {
                        property = dataItem.GetType().GetProperty(referenceAttribute.Property);
                        object result =
                            HelperManager.BindPengambilanObjekDariSource(property.GetValue(dataItem, null),
                                DisplayMemberPath);
                        ThreadPool.QueueUserWorkItem((s) =>
                        {
                            Dispatcher.Invoke((ThreadStart)delegate ()
                            {
                                var arr = s as object[];
                                if (arr != null)
                                {
                                    var LabelInfo = arr[0] as LabelInfo;
                                    if (LabelInfo != null)
                                        LabelInfo.Text = (arr[1] == null ? "" : arr[1].ToString());
                                }
                            }, DispatcherPriority.Render);
                        }, new object[] { LabelInfo2, result });
                        combo.Text = (result == null ? "" : result.ToString());
                    }
                }
                else
                {
                    object result =
                        HelperManager.BindPengambilanObjekDariSource(dataItem,
                            DisplayMemberPath);
                    ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Dispatcher.Invoke((ThreadStart)delegate ()
                        {
                            var arr = s as object[];
                            if (arr != null)
                            {
                                var LabelInfo = arr[0] as LabelInfo;
                                if (LabelInfo != null)
                                    LabelInfo.Text = (arr[1] == null ? "" : arr[1].ToString());
                            }
                        }, DispatcherPriority.Render);
                    }, new object[] { LabelInfo2, result });
                    combo.Text = (result == null ? "" : result.ToString());
                }
            }
        }

        private void ComboOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (DataGridOwner is InsertDataGrid)
                (DataGridOwner as InsertDataGrid).FreezeKeyDown = false;
        }

        //public IEnumerable ItemsSourceLocal { get{} set; }
        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var combo = sender as CoreComboBoxGrid;
            if (combo != null)
            {
                combo.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void CoreComboBoxGridOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        public event KeyEventHandler KeyDown;
        private void DataGridOwnerOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                e.Handled = true;
        }

        private void DataGridOwnerOnKeyDown2(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                DataGridOwner.KeyUp -= DataGridOwnerOnKeyDown2;
                LoadDefaultStateContent(currentCell);
            }
        }
        public enum StateInfo
        {
            Info, Edit, Commit
        }

        public event EventHandler<ItemEventArgs<StateInfo>> ChangeState;
        private void LoadDefaultStateContent(DataGridCell cell)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(delegate (object state)
                {
                    Thread.Sleep(20);
                    Manager.Timeout(Dispatcher, () =>
                                                    {
                                                        this.Tag = state;
                                                        OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Info));
                                                    });
                    HasChangeCombo = false;
                    //Manager.Timeout(Dispatcher, () => cell.KeyUp += LabelInfo2OnKeyUp);
                }, cell);

                //cell.KeyUp -= LabelInfo2OnKeyUp;
                var coreComboBoxGrid = cell.Content as CoreComboBoxGrid;
                if (coreComboBoxGrid != null)
                {
                    var LabelInfo = coreComboBoxGrid.Tag as LabelInfo;
                    if (LabelInfo != null) LabelInfo.Text = coreComboBoxGrid.Text;
                    cell.Content = coreComboBoxGrid.Tag as LabelInfo;
                    var property = cell.DataContext.GetType().GetProperty(this.ValuePath);
                    if (property != null)
                        cell.DataContext.GetType().GetProperty(this.ValuePath).SetValue(cell.DataContext, coreComboBoxGrid.Value, null);
                    else
                        cell.DataContext.GetType().GetProperty(this.SelectedValuePath).SetValue(cell.DataContext, coreComboBoxGrid.Value, null);
                }


                DataGridOwner.KeyUp -= DataGridOwnerOnKeyDown;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                Manager.Timeout(Dispatcher, () =>
                {
                    while (!((DataGridCell)state).IsFocused)
                    {
                        Thread.Sleep(2);
                        ((DataGridCell)state).Focus();
                    }
                    Console.WriteLine();
                });
            }, cell);
        }

        //private object currentContext;
        private void LabelInfo2OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !HasChangeCombo)
            {
                var cell = sender as DataGridCell;
                if (cell != null)
                {
                    currentCell = cell;
                    if (cell.Content != null)
                    {
                        if (cell.Content is LabelInfo)
                        {
                            var coreComboBoxGrid = (cell.Content as LabelInfo).Tag as CoreComboBoxGrid;
                            if (coreComboBoxGrid != null)
                            {
                                //cell.KeyUp -= LabelInfo2OnKeyUp;

                                //if (CoreComboBoxGrid.Value == null &&
                                //    !string.IsNullOrEmpty((cell.Content as LabelInfo).Text))
                                //{
                                //ChangeFromLocal = true;
                                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                                                                                    cell.DataContext,
                                                                                    string.IsNullOrEmpty(ValuePath) ? SelectedValuePath : ValuePath);
                                coreComboBoxGrid.Value = propertyValuePath;
                                //}
                                ThreadPool.QueueUserWorkItem(delegate (object state)
                                {
                                    var arr = state as object[];
                                    Thread.Sleep(50);
                                    Manager.Timeout(Dispatcher, () =>
                                    {
                                        if (arr != null)
                                        {
                                            var dataGridCell = arr[0] as DataGridCell;
                                            if (dataGridCell != null)
                                            {
                                                var LabelInfo = dataGridCell.Content as LabelInfo;
                                                if (LabelInfo != null)
                                                {
                                                    dataGridCell.Content = LabelInfo.Tag as CoreComboBoxGrid;
                                                    var comboBox = arr[1] as CoreComboBoxGrid;
                                                    if (comboBox != null)
                                                        OnInitializeComboBox(new ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>(new KeyValuePair<object, CoreComboBoxGrid>(comboBox.Temp, comboBox)));
                                                    Thread.Sleep(10);
                                                    if (comboBox != null) comboBox.FocusControl();
                                                    this.Tag = cell;
                                                    OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Edit));
                                                }
                                            }

                                        }
                                    });
                                }, new object[] { cell, coreComboBoxGrid });

                                coreComboBoxGrid.LostFocus += CoreComboBoxGridOnLostFocus;
                                DataGridOwner.KeyUp += DataGridOwnerOnKeyDown;
                                DataGridOwner.KeyUp += DataGridOwnerOnKeyDown2;
                                HasChangeCombo = false;
                            }
                        }
                        else if (cell.Content is CoreComboBoxGrid)
                        {
                            LoadDefaultStateContent(cell);
                        }
                    }
                }
            }
            else if (e.Key == Key.Return && HasChangeCombo)
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Thread.Sleep(100);
                    HasChangeCombo = false;
                });
        }

        public event EventHandler<ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>> InitializeComboBox;
        //private bool firstLoad;
        //private bool canHide;

        protected virtual void OnInitializeComboBox(ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>> e)
        {
            EventHandler<ItemEventArgs<KeyValuePair<object, CoreComboBoxGrid>>> handler = InitializeComboBox;
            if (handler != null) handler(this, e);
        }


        #endregion Methods

        #region Implementation of IMultipleHeader

        public void SetHeader()
        {
            if (Header is string)
                if (Header.ToString().Contains("^"))
                {
                    string[] arr = Header.ToString().Split(new[] { '^' });
                    var stackPanel = new StackPanel();
                    foreach (string s in arr)
                    {
                        var text = new LabelInfo();
                        text.Text = s.ToUpper();
                        text.FontWeight = FontWeights.SemiBold;
                        text.Margin = new Thickness(0);
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        stackPanel.Children.Add(text);
                    }
                    Header = stackPanel;
                }
        }

        #endregion Implementation of IMultipleHeader

        public object TempPropertyChange { get; set; }

        protected virtual void OnChangeState(ItemEventArgs<StateInfo> e)
        {
            CurrentState = e.Item;
            var handler = ChangeState;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var handler = PreviewKeyUp;
            if (handler != null) handler(sender, e);
        }

        public object Tag { get; set; }

        protected virtual void OnInitialize(ItemEventArgs<object> e)
        {
            var handler = Initialize;
            if (handler != null) handler(this, e);
        }

        public StateInfo CurrentState { get; set; }

        protected virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown.Invoke(sender, e);
        }
    }
}