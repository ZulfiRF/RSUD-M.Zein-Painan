using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Implementations.InputGrid;
using Newtonsoft.Json;
using Color = System.Drawing.Color;

namespace Core.Framework.Windows.Implementations
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;


    public class CoreListView : ListView, IGridViewControl
    {
        #region Constructors and Destructors

        public CoreListView()
        {
            IsTextSearchEnabled = false;
            this.Loaded += this.CoreListView_Loaded;
            KeyDown += OnKeyDown;
        }

        private Type commonModuleType;
        public Type CommonModuleType
        {
            get { return commonModuleType; }
            set
            {
                commonModuleType = value;
            }
        }

        private Type viewType;
        public Type ViewType
        {
            get { return viewType; }
            set
            {
                viewType = value;
            }
        }

        private Type domain;
        public Type Domain
        {
            get { return domain; }
            set
            {
                domain = value;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                ExecuteControlForm(CommonModuleType);
            }
        }

        public event EventHandler<ItemEventArgs<object>> DeleteData;
        public void OnDeleteData(ItemEventArgs<object> e)
        {
            var handler = DeleteData;
            if (handler != null) handler(this, e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var listView = (View as GridView);
            if (listView != null)
            {
                var columns = listView.Columns.ToList();
                foreach (var gridViewColumn in columns)
                {
                    if (gridViewColumn.Header is string)
                        if (gridViewColumn.Header.ToString().Contains("^"))
                        {
                            gridViewColumn.Header = gridViewColumn.Header.ToString().Replace("^", "\n");
                        }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void ExecuteControlForm(Type typeDomain)
        {
            try
            {
                var keyValuesContaintCommon = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key.ToString().Contains(typeDomain.Name) && n.Key.ToString().Contains("Common"));
                if (keyValuesContaintCommon != null)
                {
                    if (typeof(IGenericCalling).IsAssignableFrom(keyValuesContaintCommon.Value as Type))
                    {
                        if (keyValuesContaintCommon.Value is Type)
                        {
                            var type = keyValuesContaintCommon.Value as Type;
                            if (type.IsClass)
                            {
                                var instanace = Activator.CreateInstance(type);
                                if (instanace != null)
                                {
                                    try
                                    {
                                        var tag = instanace as ITag;
                                        if (tag != null)
                                        {
                                            tag.Tag = Domain;
                                            tag.View = ViewType;
                                        }

                                        var generic = instanace as IGenericCalling;
                                        if (generic != null) generic.InitView();
                                    }
                                    catch (Exception e)
                                    {
                                        //Manager.HandleException(e);
                                        Log.ThrowError(e, "300");
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //Manager.HandleException(e, "Error ExecuteControlForm : Domain Type atau DomainNameSpace!");
                Log.ThrowError(e, "300");
            }
        }

        public IEnumerable<ItemsParameters> ItemsParameters
        {
            set
            {
                Style style = null;
                var items = value;
                foreach (var itemsParameter in items)
                {
                    style = new Style(typeof(ListViewItem));
                    var dataTrigger = new DataTrigger();

                    //var generateCls = new GenerateClass();
                    //generateCls.Kondisi = itemsParameter.BindingValue;
                    //generateCls.Domain = 

                    dataTrigger.Binding = new Binding("TagObject");
                    dataTrigger.Value = itemsParameter.FillValue;

                    //var setterBack = new Setter(BackgroundProperty, new SolidColorBrush(itemsParameter.BackColor));
                    //var setterFront = new Setter(ForegroundProperty, new SolidColorBrush(itemsParameter.Frontolor));
                    var setterTooltip = new Setter(ToolTipProperty, itemsParameter.Tooltip);

                    //dataTrigger.Setters.Add(setterBack);
                    //dataTrigger.Setters.Add(setterFront);
                    dataTrigger.Setters.Add(setterTooltip);

                    style.Triggers.Add(dataTrigger);
                }
                ItemContainerStyle = style;
            }
        }


        #endregion

        #region Methods

        private void CoreListView_Loaded(object sender, RoutedEventArgs e)
        {
            var gridview = this.View as GridView;
            if (gridview != null)
            {
                foreach (GridViewColumn column in gridview.Columns)
                {
                    if (column is GridViewColumn)
                    {
                        GridViewColumn gridViewColumn = column;
                        if (gridViewColumn.Header is SortGridViewColumnHeader)
                        {
                            var sortGridViewColumn = gridViewColumn.Header as SortGridViewColumnHeader;
                            sortGridViewColumn.Click += this.SortGridViewColumnClick;
                        }
                    }
                }
            }


        }

        private void SortGridViewColumnClick(object sender, RoutedEventArgs e)
        {
            var column = sender as SortGridViewColumnHeader;
            List<object> temp = this.ItemsSource.Cast<object>().ToList();
            if (column != null)
            {
                if (column.SortType == SortGridViewColumnHeader.SortTypeDefinition.Asc)
                {
                    this.ItemsSource =
                        temp.OrderByDescending(n => n.GetType().GetProperty(column.Tag.ToString()).GetValue(n, null));
                    column.SortType = SortGridViewColumnHeader.SortTypeDefinition.Desc;
                }
                else
                {
                    this.ItemsSource =
                        temp.OrderBy(n => n.GetType().GetProperty(column.Tag.ToString()).GetValue(n, null));
                    column.SortType = SortGridViewColumnHeader.SortTypeDefinition.Asc;
                }
            }
        }

        #endregion

        //public IEnumerable DataSource { get; set; }
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
                    //this.dataSource = null;
                    //this.defaulItemSource = null;
                    return;
                }
                // this.countRowLoad = 0;
                //  this.countRowInGrid = 0;
                this.Items.Clear();
                //if (this.PartProgressRing != null)
                //{
                //    this.PartProgressRing.IsActive = true;
                //}
                //this.defaulItemSource = null;
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
                        countLoad++;
                        listModel.Add(model);

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

            }
        }

        private bool usingVirtualizaion;

        public bool UsingVirtualization
        {
            get { return usingVirtualizaion; }
            set
            {
                usingVirtualizaion = value;
                if (usingVirtualizaion)
                {
                    if (Application.Current == null) return;
                    Style = Application.Current.Resources["lvStyle"] as Style;
                    VirtualizingStackPanel.SetIsVirtualizing(this, true);
                    VirtualizingStackPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
                    ScrollViewer.SetIsDeferredScrollingEnabled(this, true);
                }
                //else
                //{
                //    VirtualizingStackPanel.SetIsVirtualizing(this, false);
                //    VirtualizingStackPanel.SetVirtualizationMode(this, VirtualizationMode.Standard);
                //    ScrollViewer.SetIsDeferredScrollingEnabled(this, false);
                //}
            }
        }


        public int LazyLoadItem { get; set; }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            //if (e.Key == Key.PageDown || e.Key == Key.PageUp)
            //{
            //    e.Handled = true;
            //}
            base.OnPreviewKeyDown(e);
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //if(e.Key == Key.PageDown || e.Key == Key.PageUp)
            //{
            //    e.Handled = true;
            //}
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Delete)
            {
                if (Manager.Confirmation("Konfirmasi", "Apakah anda akan menghapus data ini?"))
                    OnDeleteData(new ItemEventArgs<object>(this.SelectedItems as IList));
            }
            base.OnKeyDown(e);
        }

    }
}