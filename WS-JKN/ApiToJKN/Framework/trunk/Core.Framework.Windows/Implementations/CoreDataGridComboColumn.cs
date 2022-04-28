using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridComboColumn : DataGridComboBoxColumn, IMultipleHeader
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public bool DisableAutoSearchWhenSelectedItem { get; set; }
        protected virtual void OnSelectionChanged(CoreComboBoxGrid ctrl, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(ctrl, e);
            }
        }

        // Using a DependencyProperty as the backing store for DomainNameSpaces.  This enables animation, styling, binding, etc...

        #region Static Fields



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

        private Type domainType;

        public Type DomainType
        {
            get { return domainType; }
            set
            {
                domainType = value;
                if (domainType == null) return;
                DomainNameSpaces = value.FullName;
            }
        }
        #endregion

        #region Methods


        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            bool value = base.CommitCellEdit(editingElement);
            return value;
        }

        public bool DisableAutoSearch { get; set; }

        public CoreComboBoxGrid GetContent(object dataItem)
        {
            CoreComboBoxGrid combo;
            combo = GetCellContent(dataItem) as CoreComboBoxGrid;
            //tanggal 15 05 2015 di tutup karena  view yang di render ga semua row
            //if (!dictionary.TryGetValue(dataItem, out combo))
            //{
            //}
            return combo;
        }
        private bool firstLoad = false;
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            CoreComboBoxGrid combo;
            if (!firstLoad)
            {
                DataGridOwner.LoadingRow += DataGridOwnerOnLoadingRow;
                firstLoad = true;
            }
            //if (!dictionary.TryGetValue(dataItem, out combo))
            {
                combo = new CoreComboBoxGrid();
                combo.Loaded += ComboOnLoaded;
                combo.LostFocus += ComboOnGotFocus;
                combo.KeyDown+= ComboOnKeyDown;
                combo.Temp = dataItem;
                ApplyBinding(this.SelectedItemBinding, combo, Selector.SelectedItemProperty);
                ApplyBinding(this.SelectedValueBinding, combo, Selector.SelectedValueProperty);
                ApplyBinding(this.TextBinding, combo, ComboBox.TextProperty);
                combo.GotFocus += (sender, args) =>
                {
                    var comboLocal = sender as CoreComboBoxGrid;
                    if (comboLocal != null)
                    {
                        var gridCell = comboLocal.Parent as DataGridCell;
                        if (gridCell != null)
                        {
                            this.DataGridOwner.SelectedItem = gridCell.DataContext;
                        }
                    }
                    //if (this.DataGridOwner is InsertDataGrid)
                    //    (this.DataGridOwner as InsertDataGrid).FreezeKeyDown = true;
                    //this.DataGridOwner.CurrentColumn = this;
                    //this.DataGridOwner.SelectedItem = combo.Temp;
                };
                //OnBindingItemSource(new ItemEventArgs<CoreComboBoxGrid>(combo));
                combo.SelectionChanged += (sender, args) => OnSelectionChanged(combo, args);
                object content = cell.Content;
                var children = Manager.FindVisualChild<TextBlock>(content as FrameworkElement);
                combo.Height = cell.Height;
                combo.Style = Application.Current.Resources["MetroComboBoxGrid"] as Style;
                combo.Height = cell.Height;
                combo.DisableAutoSearchWhenSelectedItem = DisableAutoSearchWhenSelectedItem;
                combo.DisplayMemberPath = this.DisplayMemberPath;
                combo.UsingSearchByFramework = this.UsingSearchByFramework;
                combo.DomainNameSpaces = this.DomainNameSpaces;
                combo.ValuePath = string.IsNullOrEmpty(this.ValuePath) ? SelectedValuePath : ValuePath;

                //combo.DisableSearch = DisableAutoSearch;
                combo.Tag = children;

                if (!UsingSearchByFramework)
                    combo.ItemsSource = ItemsSource;
                combo.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args)
                {

                    var parent = this.DataGridOwner;
                    if (parent != null)
                    {
                        parent.CommitEdit();
                    }
                    var comboLocal = sender as CoreComboBoxGrid;
                    if (comboLocal != null)
                    {
                        var gridCell = comboLocal.Parent as DataGridCell;
                        if (gridCell != null)
                        {
                            PropertyInfo propertyValuePath = gridCell.DataContext.GetType().GetProperty(this.SelectedValuePath);
                            if (propertyValuePath != null)
                            {
                                if (comboLocal.SelectedItem != null)
                                {
                                    if (comboLocal.ValuePath != null)
                                    {
                                        PropertyInfo prop = comboLocal.SelectedItem.GetType().GetProperty(comboLocal.ValuePath);
                                        //PropertyInfo prop = gridCell.DataContext.GetType().GetProperty(comboLocal.ValuePath);

                                        if (prop != null)
                                        {
                                            //try
                                            //{
                                            //    object propertyValueSelected = prop.GetValue(comboLocal.SelectedItem, null);
                                            //    comboLocal.ValueTemp = propertyValueSelected;
                                            //    //propertyValuePath.SetValue(gridCell.DataContext, propertyValueSelected, null);
                                            //}
                                            //catch (Exception e)
                                            //{
                                            //    Log.Error(e);
                                            //}

                                            try
                                            {
                                                object propertyValueSelected;
                                                var checkProperty =
                                                    combo.SelectedItem.GetType().GetProperty(comboLocal.ValuePath);
                                                if (checkProperty != null)
                                                    propertyValueSelected = prop.GetValue(comboLocal.SelectedItem, null);
                                                else
                                                    propertyValueSelected = prop.GetValue(combo.Temp, null);
                                                if (TempPropertyChange != null && TempPropertyChange.Equals(propertyValueSelected))
                                                {
                                                    return;
                                                }
                                                TempPropertyChange = propertyValueSelected;
                                                propertyValuePath.SetValue(gridCell.DataContext, propertyValueSelected, null);
                                            }
                                            catch (Exception)
                                            {
                                                propertyValuePath.SetValue(gridCell.DataContext, combo.SelectedItem, null);

                                            }

                                        }
                                        else
                                        {
                                            propertyValuePath.SetValue(gridCell.DataContext, combo.SelectedItem, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    OnSelectionChanged(sender as CoreComboBoxGrid, args);
                };

            }


            if (combo.Parent != null)
            {
                var dataGridCell = combo.Parent as DataGridCell;
                if (dataGridCell != null) dataGridCell.Content = null;
            }
            if (dataItem != null)
            {
                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                    dataItem,
                    this.SelectedValuePath);
                if (propertyValuePath != null)
                {
                    //combo.Value = propertyValuePath.ToString();
                }
            }

            if (dataItem is INotifyPropertyChanged)
            {
                (dataItem as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            }


            PropertyInfo property = cell.GetType()
                .GetProperty(
                    "EditingElement",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public
                    | BindingFlags.Instance);
            if (property != null)
            {
                var textBlock = (property.GetValue(cell, null) as TextBlock);
                if (textBlock != null)
                {
                    PropertyInfo comboProperty = combo.GetType()
                        .GetProperty(
                            "CurrentText",
                            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic
                            | BindingFlags.Public | BindingFlags.Instance);
                    if (comboProperty != null)
                    {
                        comboProperty.SetValue(combo, textBlock.Text, null);
                    }
                    comboProperty = combo.GetType()
                        .GetProperty(
                            "CurrentItem",
                            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic
                            | BindingFlags.Public | BindingFlags.Instance);
                    if (comboProperty != null)
                    {
                        comboProperty.SetValue(combo, textBlock.Tag, null);
                    }
                }
            }
            return combo;
        }

        private void ComboOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            
        }

        private void DataGridOwnerOnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var changed = e.Row.Item as INotifyPropertyChanged;
            if (changed != null)
                changed.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {

            object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                                                                                    sender,
                                                                                    this.SelectedValuePath);
            if (propertyValuePath != null)
            {
                if (TempPropertyChange != null &&
                    TempPropertyChange.Equals(
                        propertyValuePath))
                {
                    return;
                }
                TempPropertyChange =
                    propertyValuePath;
                var item = this.GetCellContent(sender);
                var block = item as CoreComboBoxGrid;
                if (block != null && !block.Text.Equals(propertyValuePath.ToString()))
                    block.Value = propertyValuePath.ToString();
            }
            //if (propertyValuePath != null)
            //{
            //    if (combo.Value != null && !combo.Value.ToString().Equals(propertyValuePath.ToString()))
            //        combo.Value = propertyValuePath.ToString();
            //    else if (combo.Value == null)
            //        combo.Value = propertyValuePath.ToString();
            //}
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

        //public IEnumerable ItemsSourceLocal { get{} set; }

        public IEnumerable ItemsSourceLocal { get; set; }


        private void ComboOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.DataGridOwner is InsertDataGrid)
                (this.DataGridOwner as InsertDataGrid).FreezeKeyDown = false;
        }

        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var combo = sender as CoreComboBoxGrid;
            if (combo != null)
            {
                combo.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

        }


        #endregion

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...

        #region Implementation of IMultipleHeader

        public void SetHeader()
        {
            if (Header is string) if (Header.ToString().Contains("^"))
                {
                    var arr = Header.ToString().Split(new[] { '^' });
                    var stackPanel = new StackPanel();
                    foreach (var s in arr)
                    {
                        var text = new TextBlock();
                        text.Text = s.ToUpper();
                        text.FontWeight = FontWeights.SemiBold;
                        text.Margin = new Thickness(0);
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        stackPanel.Children.Add(text);
                    }
                    Header = stackPanel;
                }
        }

        #endregion

        public object TempPropertyChange { get; set; }
    }
}