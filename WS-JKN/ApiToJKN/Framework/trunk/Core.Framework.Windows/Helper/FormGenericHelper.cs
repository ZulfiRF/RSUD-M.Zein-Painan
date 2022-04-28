namespace Core.Framework.Windows.Helper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using Core.Framework.Helper.Contracts;
    using Core.Framework.Model;
    using Core.Framework.Model.Attr;
    using Core.Framework.Model.QueryBuilder.Clausa;
    using Core.Framework.Model.QueryBuilder.Enums;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Implementations;

    public class FormGenericHelper<T> : BaseFormHelper
        where T : TableItem
    {
        #region Constructors and Destructors

        public FormGenericHelper()
        {
            var rDictionary = new ResourceDictionary
                              {
                                  Source =
                                      new Uri(
                                      string.Format(
                                          "/Core.Framework.Windows;component/Styles/Controls.Form.xaml"),
                                      UriKind.Relative)
                              };
            this.Style = rDictionary["FormBasicStyle"] as Style;
            var item = Activator.CreateInstance<T>();
            object[] listHeaderAtribute = item.GetType().GetCustomAttributes(true);

            if (listHeaderAtribute.Any(n => n.GetType() == typeof(SizeAttribute)))
            {
                var size = (SizeAttribute)listHeaderAtribute.FirstOrDefault(n => n.GetType() == typeof(SizeAttribute));
                if (size != null)
                {
                    if (size.Width != 0)
                    {
                        this.Width = size.Width;
                    }
                    if (size.Height != 0)
                    {
                        this.Height = size.Height;
                    }
                    if (size.CountColumn != 0)
                    {
                        this.CountColumn = size.CountColumn;
                    }
                }
            }
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Public Events

        public event EventHandler<SourceEventArgs<T>> FilteringData;

        #endregion

        #region Properties

        protected short CountColumn { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnFilteringData(SourceEventArgs<T> e)
        {
            EventHandler<SourceEventArgs<T>> handler = this.FilteringData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        protected virtual Func<T, bool> FunctionToFilter(string keyword)
        {
            return
                model =>
                    (from property in
                        model.GetType()
                            .GetProperties()
                            .Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any())
                        let propertyPath =
                            property.GetCustomAttributes(true).OfType<SearchAttribute>().First().PropertyPath
                        where string.IsNullOrEmpty(propertyPath)
                        select property.GetValue(model, null)
                        into value
                        where value != null
                        select value).Any(value => value.ToString().ToLower().Contains(keyword.ToLower()));
        }

        protected virtual IEnumerable<WhereClause> GenereateClause(string keyword)
        {
            var item = Activator.CreateInstance<T>();
            TableAttribute tableAttribute =
                item.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            if (tableAttribute != null)
            {
                string tableName = tableAttribute.TabelName;
                //return item.GetType().GetProperties().Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any()).Select(property => new WhereClause(property.GetCustomAttributes(true).OfType<SearchAttribute>().First().PropertyPath, Comparison.Like, keyword) { LogicOperator = LogicOperator.Or }).ToArray();
                foreach (PropertyInfo source in
                    item.GetType()
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any()))
                {
                    if (source.PropertyType.IsClass && source.PropertyType.Name.ToLower().Contains("string"))
                    {
                        SearchAttribute property = source.GetCustomAttributes(true).OfType<SearchAttribute>().First();
                        yield return
                            new WhereClause(tableName + "." + property.PropertyPath, Comparison.Like, keyword)
                            {
                                LogicOperator
                                    =
                                    LogicOperator
                                    .Or
                            };
                    }
                    else
                    {
                        object tableRelation = Activator.CreateInstance(source.PropertyType);
                        TableAttribute firstOrDefault =
                            tableRelation.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            string tableNameRelation = firstOrDefault.TabelName;
                            SearchAttribute property =
                                source.GetCustomAttributes(true).OfType<SearchAttribute>().First();
                            yield return
                                new WhereClause(
                                    tableNameRelation + "." + property.PropertyPath,
                                    Comparison.Like,
                                    keyword) { LogicOperator = LogicOperator.Or };
                        }
                    }
                }
            }
        }

        private void DeleteData(PanelMetro panelForm, CoreDataGrid dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            try
            {
                using (var manager = new ContextManager(this.ConnectionString, this.ManagerConnection))
                {
                    var listItem = new List<object>();
                    if (dataGrid.SelectedItems != null)
                    {
                        if (Manager.Confirmation("Delete Item", "Apakah anda yakin akan menghapus data ?"))
                        {
                            foreach (object selectedItem in dataGrid.SelectedItems)
                            {
                                manager.DeleteObject(selectedItem as TableItem);
                                listItem.Add(selectedItem);
                            }
                        }
                    }
                    else
                    {
                        if (panelForm.Selecteditem == null)
                        {
                            return;
                        }

                        if (Manager.Confirmation("Delete Item", "Apakah anda yakin akan menghapus data ?"))
                        {
                            if (panelForm.Selecteditem is TableItem)
                            {
                                manager.DeleteObject(panelForm.Selecteditem as TableItem);
                                dataGrid.Items.Remove(panelForm.Selecteditem);
                            }
                        }
                    }
                    manager.Commit();
                    foreach (object data in listItem)
                    {
                        dataGrid.Items.Remove(data);
                    }

                    buttonTemplate.ClearItem();
                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Delete Item");
            }
        }

        private void InitializeDataInGrid(FormGrid grid, CoreDataGrid dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            ThreadPool.QueueUserWorkItem(
                state => Manager.Timeout(
                    grid.Dispatcher,
                    () =>
                    {
                        try
                        {
                            using (var manager = new ContextManager(this.ConnectionString, this.ManagerConnection))
                            {
                                var coreData = new CoreContext<T>(manager);

                                IEnumerable<T> results = coreData.Take(dataGrid.LazyLoadItem).Render();
                                var evt = new SourceEventArgs<T>(results);
                                this.OnFilteringData(evt);
                                dataGrid.DataSource = evt.ListSource;

                                buttonTemplate.IsBusy = true;
                            }
                        }
                        catch (Exception exception)
                        {
                            Manager.HandleException(exception, "Load Data");
                        }
                    }));
        }

        private void OnLoaded(object objectSender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var item = Activator.CreateInstance<T>();
                object[] listHeaderAtribute = item.GetType().GetCustomAttributes(true);
                var grid = new FormGrid
                           {
                               VerticalAlignment = VerticalAlignment.Stretch,
                               HorizontalAlignment = HorizontalAlignment.Stretch,
                               Margin = new Thickness(0)
                           };
                var panelForm = new PanelMetro
                                {
                                    Title =
                                        listHeaderAtribute.Any(
                                            n => n.GetType() == typeof(DisplayFormAttribute))
                                            ? ((DisplayFormAttribute)
                                        listHeaderAtribute.First(
                                            n => n.GetType() == typeof(DisplayFormAttribute))).Title
                                            : "Form Panel"
                                };
                var wrapPanelForm = new WrapPanel();
                var dataGrid = new CoreDataGrid { AutoGenerateColumns = false };
                foreach (PropertyInfo propertyInfo in
                    item.GetType()
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes(true).Any(p => p is CoreInputAttribute))
                        .OrderBy(n => n.GetCustomAttributes(true).OfType<CoreInputAttribute>().First().Index))
                {
                    DataGridBoundColumn column = null;
                    IEnumerable<CoreInputAttribute> a =
                        propertyInfo.GetCustomAttributes(true).OfType<CoreInputAttribute>();
                    GridAttribute attribute =
                        propertyInfo.GetCustomAttributes(true)
                            .OfType<GridAttribute>()
                            .FirstOrDefault(o => o.GetType().Name.Equals("GridAttribute"));
                    if (attribute == null)
                    {
                        continue;
                    }

                    if (attribute.Type == FormType.Checkbox)
                    {
                        column = new DataGridCheckBoxColumn();
                        column.IsReadOnly = true;
                    }
                    else
                    {
                        column = new DataGridTextColumn();
                    }
                    column.Header = attribute.Title;
                    Binding binding;
                    if (string.IsNullOrEmpty(attribute.DisplayPath))
                    {
                        binding = new Binding(propertyInfo.Name);
                    }
                    else
                    {
                        binding = new Binding(attribute.DisplayPath);
                    }
                    column.Binding = binding;
                    if (attribute.Converter != null)
                    {
                        var converter = Activator.CreateInstance(attribute.Converter) as IValueConverter;
                        binding.Converter = converter;
                    }
                    dataGrid.Columns.Add(column);
                }
                dataGrid.EditMode = true;
                dataGrid.IsReadOnly = true;
                grid.SizeChanged += (sender, args) =>
                {
                    foreach (FrameworkElement element in wrapPanelForm.Children.OfType<FrameworkElement>())
                    {
                        element.Width = args.NewSize.Width / 2 - 8;
                    }
                };
                wrapPanelForm.SizeChanged +=
                    (sender, args) => { dataGrid.Margin = new Thickness(2, args.NewSize.Height + 40, 2, 2); };
                foreach (PropertyInfo propertyInfo in
                    item.GetType()
                        .GetProperties()
                        .Where(n => n.GetCustomAttributes(true).Any(p => p.GetType() == typeof(FormInputAttribute)))
                        .OrderBy(n => n.GetCustomAttributes(true).OfType<FormInputAttribute>().First().Index))
                {
                    FormInputAttribute attribute =
                        propertyInfo.GetCustomAttributes(true)
                            .OfType<FormInputAttribute>()
                            .FirstOrDefault(o => o.GetType().Name.Equals("FormInputAttribute"));

                    object[] a = propertyInfo.GetCustomAttributes(true);
                    FieldAttribute fieldAttribute =
                        propertyInfo.GetCustomAttributes(true)
                            .OfType<FieldAttribute>()
                            .FirstOrDefault(o => o.GetType().Name.Equals("FieldAttribute"));
                    IValidateControl displayTextbox = null;
                    if (attribute != null && attribute.Type == FormType.TextBox)
                    {
                        displayTextbox = new DisplayWithTextBox
                                         {
                                             Margin = new Thickness(4),
                                             Width = wrapPanelForm.Width / this.CountColumn,
                                             WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                                             DisplayText =
                                                 propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title,
                                             Tag = attribute.Converter,
                                             Name = propertyInfo.Name,
                                             IsReadOnly =
                                                 (fieldAttribute != null
                                                  && fieldAttribute.IsPrimary)
                                                 || propertyInfo.GetCustomAttributes(true)
                                                 .OfType<ReadOnlyAttribute>()
                                                 .Any(),
                                             IsRequired =
                                                 !(fieldAttribute != null
                                                   && fieldAttribute.IsPrimary),
                                             FilterType = propertyInfo.PropertyType,
                                             MaxLength = fieldAttribute.Length,
                                             Watermark =
                                                 "Type "
                                                 + propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title + "..."
                                         };
                    }
                    else if (attribute.Type == FormType.ListItem)
                    {
                        displayTextbox = new DisplayWithComboBox
                                         {
                                             Margin = new Thickness(4),
                                             Width = wrapPanelForm.Width / this.CountColumn,
                                             WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                                             DisplayText =
                                                 propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title,
                                             Tag = attribute.Converter,
                                             Name = propertyInfo.Name,
                                             DisplayMemberPath = attribute.DisplayPath,
                                             UsingSearchByFramework = true,
                                             SearchTypeWith = SearchType.Contains,
                                             Key = attribute.ValuePath,
                                             Watermark =
                                                 "Type "
                                                 + propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title + "..."
                                         };
                        if (attribute.Models != null)
                        {
                            {
                                (displayTextbox as DisplayWithComboBox).Models = attribute.Models;
                                (displayTextbox as DisplayWithComboBox).DisplayMemberPath = "Description";
                                (displayTextbox as DisplayWithComboBox).Key = fieldAttribute.FieldName;
                            }
                        }
                        if (attribute.ModelItem != null)
                        {
                            (displayTextbox as DisplayWithComboBox).DomainNameSpaces =
                                attribute.ModelItem.GetType().Name;
                        }
                    }
                    else if (attribute.Type == FormType.Checkbox)
                    {
                        displayTextbox = new DisplayWithCheckBox
                                         {
                                             Margin = new Thickness(4),
                                             Width = wrapPanelForm.Width / this.CountColumn,
                                             WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                                             DisplayText =
                                                 propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title,
                                             Tag = attribute.Converter,
                                             Name = propertyInfo.Name,
                                         };
                    }
                    else
                    {
                        displayTextbox = new DisplayWithTextBlock
                                         {
                                             Margin = new Thickness(4),
                                             Width = wrapPanelForm.Width / this.CountColumn,
                                             WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                                             DisplayText =
                                                 propertyInfo.GetCustomAttributes(true)
                                                 .OfType<FormInputAttribute>()
                                                 .First()
                                                 .Title,
                                             Tag = attribute.Converter,
                                             Name = propertyInfo.Name,
                                         };
                    }
                    IEnumerable<FieldAttribute> propertyField =
                        propertyInfo.GetCustomAttributes(true).OfType<FieldAttribute>();
                    FieldAttribute[] atributeFields = propertyField as FieldAttribute[] ?? propertyField.ToArray();
                    if (atributeFields.Any())
                    {
                        switch (atributeFields.First().IsAllowNull)
                        {
                            case SpesicicationType.AllowNull:
                                displayTextbox.IsRequired = false;
                                break;

                            case SpesicicationType.AutoIncrement:
                                break;

                            case SpesicicationType.NotAllowNull:
                                SkipAttribute skipAttribute =
                                    propertyInfo.GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();
                                if (skipAttribute == null)
                                {
                                    displayTextbox.IsRequired = true;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    wrapPanelForm.Children.Add(displayTextbox as FrameworkElement);
                }
                dataGrid.SelectionChanged += (sender, args) =>
                {
                    try
                    {
                        if (dataGrid.SelectedItem == null)
                        {
                            return;
                        }
                        panelForm.Selecteditem = dataGrid.SelectedItem;
                        foreach (FrameworkElement result in wrapPanelForm.Children.OfType<FrameworkElement>())
                        {
                            if (result is DisplayWithTextBox)
                            {
                                var displayTextBoxTemp = result as DisplayWithTextBox;
                                object value =
                                    dataGrid.SelectedItem.GetType()
                                        .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                        .GetValue(dataGrid.SelectedItem, null);
                                if (value != null)
                                {
                                    displayTextBoxTemp.Text = value.ToString();
                                }
                                else
                                {
                                    displayTextBoxTemp.Text = "";
                                }
                            }
                            else if (result is DisplayWithCheckBox)
                            {
                                var displayTextBoxTemp = result as DisplayWithCheckBox;
                                if (displayTextBoxTemp.Tag is Type)
                                {
                                    var converter =
                                        Activator.CreateInstance(displayTextBoxTemp.Tag as Type) as IValueConverter;
                                    if (converter != null)
                                    {
                                        displayTextBoxTemp.IsChecked =
                                            converter.Convert(
                                                dataGrid.SelectedItem.GetType()
                                                    .GetProperty(
                                                        displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                                    .GetValue(dataGrid.SelectedItem, null),
                                                null,
                                                null,
                                                CultureInfo.InvariantCulture) as bool?;
                                    }
                                }
                            }
                            else if (result is DisplayWithComboBox)
                            {
                                var displayTextBoxTemp = result as DisplayWithComboBox;
                                displayTextBoxTemp.SelectedItem =
                                    dataGrid.SelectedItem.GetType()
                                        .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                        .GetValue(dataGrid.SelectedItem, null);
                            }
                            else if (result is DisplayWithTextBlock)
                            {
                                var displayTextBoxTemp = result as DisplayWithTextBlock;
                                displayTextBoxTemp.Text =
                                    dataGrid.SelectedItem.GetType()
                                        .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                        .GetValue(dataGrid.SelectedItem, null)
                                        .ToString();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Manager.HandleException(exception, "Datagrid Selection Changed");
                    }
                };

                dataGrid.KeyDown += (sender, args) =>
                {
                    this.InModeSearch = true;
                    if (this.PartTextBox != null)
                    {
                        this.PartTextBox.Focus();
                    }
                };
                dataGrid.VerticalAlignment = VerticalAlignment.Stretch;
                dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                dataGrid.Margin = new Thickness(0);
                panelForm.Content = wrapPanelForm;
                grid.Children.Add(dataGrid);
                grid.Children.Add(panelForm);

                dataGrid.SizeChanged += (sender, args) =>
                {
                    var dataGrid1 = sender as DataGrid;
                    if (dataGrid1 != null)
                    {
                        dataGrid1.Tag = args.NewSize.Height;
                    }
                };
                var buttonTemplate = new ButtonCommandTemplate { IsBusy = false };
                buttonTemplate.ResetItem += (sender, args) =>
                {
                    panelForm.Selecteditem = null;
                    //dataGrid.SelectedIndex = -1;
                    this.PartTextBox.Text = "";
                    //RefreshData(grid, 0, dataGrid, buttonTemplate);
                };
                buttonTemplate.SaveItem += (sender, args) =>
                {
                    try
                    {
                        using (var manager = new ContextManager(this.ConnectionString, this.ManagerConnection))
                        {
                            TableItem data = null;
                            bool isNewItem = false;
                            if (panelForm.Selecteditem is TableItem)
                            {
                                if (Manager.Confirmation("Update Item", "Apakah anda yakin akan merubah data ?"))
                                {
                                    data = panelForm.Selecteditem as TableItem;
                                    data.InitializeManager(manager);
                                    data.OnInit(panelForm.DataInForm);
                                    data.IsNew = false;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                data = Activator.CreateInstance(item.GetType()) as TableItem;
                                if (data != null)
                                {
                                    data.OnInit(panelForm.DataInForm);
                                    manager.InsertObject(data);
                                    isNewItem = true;
                                }
                            }

                            manager.Commit();
                            if (isNewItem)
                            {
                                data.IsNew = false;
                                var list = new List<T> { data as T };
                                data.RebindDataToPrevious();
                                var evt = new SourceEventArgs<T>(list);
                                dataGrid.AddItemSource(evt.ListSource);
                            }
                            else
                            {
                                data.IsNew = false;
                                dataGrid.SelectedItem = data;

                                dataGrid.Items.Refresh();
                                if (dataGrid.SelectedItem != null)
                                {
                                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                                }
                            }
                            buttonTemplate.ClearItem();
                        }
                    }
                    catch (Exception exception)
                    {
                        Manager.HandleException(exception, "Saved Item");
                    }
                };
                buttonTemplate.DeleteItem += (sender, args) => { this.DeleteData(panelForm, dataGrid, buttonTemplate); };
                buttonTemplate.SetFormSubmit(panelForm);
                dataGrid.DeletePress += (sender, args) => { this.DeleteData(panelForm, dataGrid, buttonTemplate); };
                dataGrid.RefreshData +=
                    (sender, args) =>
                        ThreadPool.QueueUserWorkItem(
                            delegate { this.RefreshData(grid, args.RowCount, dataGrid, buttonTemplate); });
                this.InitializeDataInGrid(grid, dataGrid, buttonTemplate);
                dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                this.MainContent = grid;
                this.FooterContent = new Grid();
                this.FooterContent.Children.Add(buttonTemplate);
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.KeyUp += (sender, args) =>
                    {
                        if (args.Key == Key.Return)
                        {
                            ThreadPool.QueueUserWorkItem(
                                state => Manager.Timeout(
                                    grid.Dispatcher,
                                    () =>
                                    {
                                        try
                                        {
                                            using (
                                                var manager = new ContextManager(
                                                    this.ConnectionString,
                                                    this.ManagerConnection))
                                            {
                                                var coreData = new CoreContext<T>(manager);
                                                WhereClause[] whereClause =
                                                    this.GenereateClause(this.PartTextBox.Text).ToArray();
                                                IEnumerable<T> results =
                                                    coreData.Take(dataGrid.LazyLoadItem).Where(whereClause).Render();
                                                var evt = new SourceEventArgs<T>(results);
                                                this.OnFilteringData(evt);

                                                dataGrid.DataSource = evt.ListSource;

                                                buttonTemplate.IsBusy = true;
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Manager.HandleException(exception, "Search Data");
                                        }
                                    }),
                                this.PartTextBox.Text);
                        }
                    };
                    var searchText = new StringBuilder();
                    searchText.Append("Ketik ");
                    foreach (PropertyInfo property in
                        item.GetType()
                            .GetProperties()
                            .Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any()))
                    {
                        searchText.Append(
                            property.GetCustomAttributes(true).OfType<SearchAttribute>().First().Title + " ,");
                    }
                    string resultTextSearch = searchText.ToString();
                    resultTextSearch = resultTextSearch.Substring(0, resultTextSearch.Length - 2);
                    TextboxHelper.SetWatermark(
                        this.PartTextBox,
                        resultTextSearch + " untuk mecari data di dalam data grid");
                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, " Load Form");
            }
        }

        private void RefreshData(FormGrid grid, int args, CoreDataGrid dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            Manager.Timeout(
                grid.Dispatcher,
                () =>
                {
                    try
                    {
                        using (var manager = new ContextManager(this.ConnectionString, this.ManagerConnection))
                        {
                            var coreData = new CoreContext<T>(manager);
                            IEnumerable<T> results =
                                coreData.Skip(args + 1)
                                    .Where(this.GenereateClause(this.PartTextBox.Text).ToArray())
                                    .Take(dataGrid.LazyLoadItem)
                                    .Render();
                            var evt = new SourceEventArgs<T>(results);
                            this.OnFilteringData(evt);
                            if (args == 0)
                            {
                                dataGrid.DataSource = evt.ListSource;
                            }
                            else
                            {
                                dataGrid.AddItemSource(evt.ListSource);
                            }
                            //buttonTemplate.IsBusy = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        Manager.HandleException(exception, "Refresh Data");
                    }
                });
        }

        #endregion
    }
}