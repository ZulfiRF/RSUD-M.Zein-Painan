namespace Core.Framework.Windows.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml.Linq;

    using Core.Framework.Helper;
    using Core.Framework.Helper.Contracts;
    using Core.Framework.Model;
    using Core.Framework.Model.Attr;
    using Core.Framework.Model.QueryBuilder.Clausa;
    using Core.Framework.Model.QueryBuilder.Enums;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Implementations;

    public class FormHelperMappingUseCoreSetting : BaseFormHelper
    {
        #region Constants

        private const int HeightDataGridInput = 200;

        private const int HeightRow = 45;

        #endregion

        #region Constructors and Destructors

        public FormHelperMappingUseCoreSetting(Type modelType)
        {
            this.ModelType = modelType;
            var rDictionary = new ResourceDictionary
                              {
                                  Source =
                                      new Uri(
                                      string.Format(
                                          "/Core.Framework.Windows;component/Styles/Controls.Form.xaml"),
                                      UriKind.Relative)
                              };
            this.Style = rDictionary["FormBasicMappingStyle"] as Style;
            if (modelType == null)
            {
                Manager.HandleException(new ArgumentException("Model Not Define"), "Load Form");
                return;
            }
            object item = Activator.CreateInstance(modelType);
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
        }

        #endregion

        #region Public Events

        public event EventHandler ComplateLoad;
        public event EventHandler<SourceEventArgs<TableItem>> FilteringData;

        #endregion

        #region Public Properties

        public int LazyLoadItem { get; set; }

        public Type ModelType { get; set; }

        public PanelMetro PanelForm { get; set; }
        public Grid WrapPanelForm { get; set; }

        #endregion

        #region Properties

        protected short CountColumn { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.OnLoaded();
        }

        public void OnComplateLoad(EventArgs e)
        {
            EventHandler handler = this.ComplateLoad;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnFilteringData(SourceEventArgs<TableItem> e)
        {
            EventHandler<SourceEventArgs<TableItem>> handler = this.FilteringData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        protected virtual Func<TableItem, bool> FunctionToFilter(string keyword)
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

        protected virtual IEnumerable<WhereClause> GenereateClause<T>(string keyword)
        {
            object item = Activator.CreateInstance<T>();
            item = (this.ModelType != null) ? Activator.CreateInstance(this.ModelType) : item;
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
                        if (!string.IsNullOrEmpty(property.PropertyPath))
                        {
                            yield return
                                new WhereClause(tableName + "." + property.PropertyPath, Comparison.Like, keyword)
                                {
                                    LogicOperator
                                        =
                                        LogicOperator
                                        .And
                                };
                        }
                        else
                        {
                            yield return
                                new WhereClause(
                                    tableName + "."
                                    + source.GetCustomAttributes(true).OfType<FieldAttribute>().First().FieldName,
                                    Comparison.Like,
                                    keyword) { LogicOperator = LogicOperator.Or };
                        }
                    }
                    else
                    {
                        object tableRelation = Activator.CreateInstance(source.PropertyType);
                        if (tableRelation is TableItem)
                        {
                            TableAttribute firstOrDefault =
                                tableRelation.GetType()
                                    .GetCustomAttributes(true)
                                    .OfType<TableAttribute>()
                                    .FirstOrDefault();
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
                        else
                        {
                            foreach (object customAttribute in source.GetCustomAttributes(true))
                            {
                                bool valid = false;
                                var firstOrDefault = customAttribute as ReferenceAttribute;
                                if (firstOrDefault != null)
                                {
                                    string tableNameRelation = firstOrDefault.TableName;
                                    if (firstOrDefault.TypeModel != null)
                                    {
                                        foreach (PropertyInfo propertyType in
                                            Activator.CreateInstance(firstOrDefault.TypeModel)
                                                .GetType()
                                                .GetProperties()
                                                .Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any())
                                            )
                                        {
                                            if (!string.IsNullOrEmpty(propertyType.Name))
                                            {
                                                FieldAttribute field =
                                                    propertyType.GetCustomAttributes(true)
                                                        .OfType<FieldAttribute>()
                                                        .First();
                                                valid = true;
                                                yield return
                                                    new WhereClause(
                                                        firstOrDefault.TableName + "." + field.FieldName,
                                                        Comparison.Like,
                                                        keyword) { LogicOperator = LogicOperator.Or };
                                            }
                                        }
                                    }

                                    SearchAttribute property =
                                        source.GetCustomAttributes(true).OfType<SearchAttribute>().First();
                                    if (!string.IsNullOrEmpty(property.PropertyPath))
                                    {
                                        yield return
                                            new WhereClause(
                                                tableNameRelation + "." + property.PropertyPath,
                                                Comparison.Like,
                                                keyword) { LogicOperator = LogicOperator.Or };
                                    }
                                    else
                                    {
                                        if (!valid)
                                        {
                                            yield return
                                                new WhereClause(
                                                    tableNameRelation + "."
                                                    + source.GetCustomAttributes(true)
                                                        .OfType<SearchAttribute>()
                                                        .Cast<FieldAttribute>()
                                                        .First()
                                                        .FieldName,
                                                    Comparison.Like,
                                                    keyword) { LogicOperator = LogicOperator.Or };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateForm(XElement tableElement, Grid wrapPanelForm, int countRow)
        {
            int row = 0;

            #region Create Form

            foreach (XElement propertiInfo in
                tableElement.Element("Design")
                    .Elements("Property")
                    .OrderBy(n => Convert.ToByte(n.Element("FormInput").Element("Index").Value))
                    .Select(n => n.Element("FormInput")))
            {
                var itemProperty = Activator.CreateInstance(this.ModelType) as TableItem;
                FieldAttribute fieldAttribute =
                    itemProperty.GetType()
                        .GetProperty(propertiInfo.Parent.Attribute("Key").Value.Trim())
                        .GetCustomAttributes(true)
                        .OfType<FieldAttribute>()
                        .FirstOrDefault();
                IValidateControl displayTextbox = null;
                if (propertiInfo.Element("Type").Value.Trim().Equals("TextBox"))
                {
                    displayTextbox = new DisplayWithTextBox
                                     {
                                         Margin = new Thickness(4),
                                         Width = wrapPanelForm.Width / this.CountColumn,
                                         WitdhDisplayText = new GridLength(150, GridUnitType.Pixel),
                                         DisplayText = propertiInfo.Element("Title").Value.Trim(),
                                         //Tag = attribute.Converter,
                                         Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                                         IsReadOnly =
                                             propertiInfo.Element("IsPrimary")
                                             .Value.Trim()
                                             .Equals("true")
                                             || propertiInfo.Element("ReadOnly")
                                             .Value.Trim()
                                             .Equals("ReadOnly"),
                                         IsRequired =
                                             !propertiInfo.Element("IsPrimary")
                                             .Value.Trim()
                                             .Equals("true")
                                             && propertiInfo.Element("IsRequired")
                                             .Value.Trim()
                                             .Equals("true"),
                                         MaxLength = fieldAttribute.Length,
                                         FilterType =
                                             itemProperty.GetType()
                                             .GetProperty(
                                                 propertiInfo.Parent.Attribute("Key").Value.Trim())
                                             .PropertyType,
                                         Watermark =
                                             propertiInfo.Element("Watermark") != null
                                             && !string.IsNullOrEmpty(
                                                 propertiInfo.Element("Watermark").Value)
                                                 ? propertiInfo.Element("Watermark").Value
                                                 : "Ketik "
                                                   + propertiInfo.Element("Title").Value.Trim()
                                                   + "..."
                                     };
                }
                else if (propertiInfo.Element("Type").Value.Trim().Equals("ListItem"))
                {
                    displayTextbox = new DisplayWithComboBox
                                     {
                                         Margin = new Thickness(4),
                                         Width = wrapPanelForm.Width / this.CountColumn,
                                         WitdhDisplayText =
                                             new GridLength(150, GridUnitType.Pixel),
                                         DisplayText = propertiInfo.Element("Title").Value.Trim(),
                                         // Tag = attribute.Converter,
                                         Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                                         IsRequired =
                                             (fieldAttribute != null
                                              && fieldAttribute.IsAllowNull
                                              == SpesicicationType.NotAllowNull),
                                         DisplayMemberPath =
                                             propertiInfo.Element("DisplayPath").Value.Trim(),
                                         UsingSearchByFramework = true,
                                         SearchTypeWith = SearchType.Contains,
                                         Key = propertiInfo.Element("ValuePath").Value.Trim(),
                                         Watermark =
                                             "Ketik " + propertiInfo.Element("Title").Value.Trim()
                                             + "..."
                                     };
                    (displayTextbox as DisplayWithComboBox).SelectionChanged += this.OnSelectionChanged;
                    if (!string.IsNullOrEmpty(propertiInfo.Element("TypeModel").Value.Trim()))
                    {
                        (displayTextbox as DisplayWithComboBox).DomainNameSpaces =
                            propertiInfo.Element("TypeModel").Value;
                    }
                }
                else if (propertiInfo.Element("Type").Value.Trim().Equals("Checkbox"))
                {
                    displayTextbox = new DisplayWithCheckBox
                                     {
                                         Margin = new Thickness(4),
                                         Width = wrapPanelForm.Width / this.CountColumn,
                                         WitdhDisplayText =
                                             new GridLength(150, GridUnitType.Pixel),
                                         DisplayText = propertiInfo.Element("Title").Value.Trim(),
                                         // Tag = attribute.Converter,
                                         Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                                     };
                }
                else if (propertiInfo.Element("Type").Value.Trim().Equals("TextBoxWithCheckBox"))
                {
                    displayTextbox = new DisplayTextBoxWithCheckBox
                                     {
                                         Margin = new Thickness(4),
                                         Width = wrapPanelForm.Width / this.CountColumn,
                                         WitdhDisplayText =
                                             new GridLength(150, GridUnitType.Pixel),
                                         DisplayText =
                                             propertiInfo.Element("Title").Value.Trim(),
                                         Name =
                                             propertiInfo.Parent.Attribute("Key")
                                             .Value.Trim(),
                                         IsReadOnly =
                                             propertiInfo.Element("IsPrimary")
                                             .Value.Trim()
                                             .Equals("true")
                                             || propertiInfo.Element("ReadOnly")
                                             .Value.Trim()
                                             .Equals("ReadOnly"),
                                         IsRequired =
                                             !propertiInfo.Element("IsPrimary")
                                             .Value.Trim()
                                             .Equals("true")
                                             && propertiInfo.Element("IsRequired")
                                             .Value.Trim()
                                             .Equals("true"),
                                         FilterType =
                                             itemProperty.GetType()
                                             .GetProperty(
                                                 propertiInfo.Parent.Attribute("Key")
                                             .Value.Trim())
                                             .PropertyType,
                                         MaxLength = fieldAttribute.Length,
                                         Watermark =
                                             "Ketik "
                                             + propertiInfo.Element("Title").Value.Trim()
                                             + "..."
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
                                         DisplayText = propertiInfo.Element("Title").Value.Trim(),
                                         //Tag = attribute.Converter,
                                         Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                                     };
                }
                //var     propertyField as FieldAttribute[] ?? propertyField.ToArray();
                //if (atributeFields.Any())
                {
                    switch (fieldAttribute.IsAllowNull)
                    {
                        case SpesicicationType.AllowNull:
                            displayTextbox.IsRequired = false;
                            break;

                        case SpesicicationType.AutoIncrement:
                            break;

                        case SpesicicationType.NotAllowNull:
                            SkipAttribute skipAttribute =
                                itemProperty.GetType()
                                    .GetProperty(propertiInfo.Parent.Attribute("Key").Value.Trim())
                                    .GetCustomAttributes(true)
                                    .OfType<SkipAttribute>()
                                    .FirstOrDefault();
                            if (skipAttribute == null)
                            {
                                displayTextbox.IsRequired = true;
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                short index = Convert.ToInt16(propertiInfo.Element("Index").Value.Trim());
                int column = (index % countRow == 0) ? index / countRow - 1 : index / countRow;
                row = (index % countRow == 0) ? countRow : index % countRow - 1;
                if (index / countRow > 0)
                {
                    //column++;
                    row += 1;
                }
                if (index / countRow > 1 && index % countRow == 0)
                {
                    column++;
                    row += 1;
                }
                if (column == -1)
                {
                    column = 0;
                }
                Grid.SetColumn(displayTextbox as FrameworkElement, column);

                Grid.SetRow(displayTextbox as FrameworkElement, row);
                wrapPanelForm.Children.Add(displayTextbox as FrameworkElement);
            }

            #endregion Create Form

            var itemPanel = new CoreListBox();
            itemPanel.Margin = new Thickness(0);
            itemPanel.Height = Double.NaN;
            itemPanel.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(itemPanel, 0);
            Grid.SetColumnSpan(itemPanel, this.CountColumn);
            Grid.SetRow(itemPanel, row - 1);
            var wrapFactory = new FrameworkElementFactory(typeof(CoreWrapPanel));

            var wrapFactoryTemplate = new ItemsPanelTemplate();
            wrapFactoryTemplate.VisualTree = wrapFactory;
            itemPanel.ItemsPanel = wrapFactoryTemplate;

            try
            {
                XElement item =
                    tableElement.Element("Design")
                        .Elements("Property")
                        .Where(n => n.Element("FormInput").Element("Type").Value.Contains("InGrid"))
                        .FirstOrDefault();
                Type model = HelperManager.GetTableItem(item.Element("FormInput").Element("TypeModel").Value);
                using (var manager = new ContextManager(this.ConnectionString, this.ManagerConnection))
                {
                    var coreData = new CoreContext<TableItem>(manager) { Model = Activator.CreateInstance(model) };
                    this.PartTextBox.Text = "";
                    var arrWhereClause = new List<WhereClause>();
                    var repository = BaseDependency.Get<IProfileRepository>();
                    if (repository != null && coreData.Model is ProfileTable)
                    {
                        ProfileItem profile = repository.CurrentProfile();
                        if (profile != null)
                        {
                            string data = profile.CodeProfile;
                            arrWhereClause.Add(
                                new WhereClause(
                                    (coreData.Model as TableItem).TableName + ".KdProfile",
                                    Comparison.Equals,
                                    data));
                        }
                    }
                    IEnumerable<TableItem> results = coreData.Where(arrWhereClause.ToArray()).Render();
                    var evt = new SourceEventArgs<TableItem>(results);
                    this.OnFilteringData(evt);
                    var textFactory = new FrameworkElementFactory(typeof(CoreCheckBox));

                    var bind = new Binding(item.Element("FormInput").Element("DisplayPath").Value);
                    bind.Mode = BindingMode.OneWay;
                    textFactory.SetValue(ContentProperty, bind);
                    textFactory.SetValue(WidthProperty, 200.0);
                    var checkboxTemplate = new DataTemplate();
                    checkboxTemplate.VisualTree = textFactory;
                    itemPanel.ItemTemplate = checkboxTemplate;
                    foreach (
                        TableItem tableItem in
                            evt.ListSource.OrderBy(
                                n =>
                                    n.GetType()
                                        .GetProperty(item.Element("FormInput").Element("DisplayPath").Value)
                                        .GetValue(n, null)))
                    {
                        itemPanel.Items.Add(tableItem);
                    }
                    ScrollViewer.SetHorizontalScrollBarVisibility(itemPanel, ScrollBarVisibility.Disabled);
                    itemPanel.KeyUp += this.ItemPanelOnKeyUp;
                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Load Data");
            }

            wrapPanelForm.Children.Add(itemPanel);

            this.WrapPanelForm = wrapPanelForm;
        }

        private void DeleteData(PanelMetro PanelForm, CoreDataGrid dataGrid, ButtonCommandTemplate buttonTemplate)
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
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (PanelForm.Selecteditem == null)
                        {
                            return;
                        }

                        if (Manager.Confirmation("Delete Item", "Apakah anda yakin akan menghapus data ?"))
                        {
                            if (PanelForm.Selecteditem is TableItem)
                            {
                                manager.DeleteObject(PanelForm.Selecteditem as TableItem);
                                dataGrid.Items.Remove(PanelForm.Selecteditem);
                            }
                        }
                        else
                        {
                            return;
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
                                var coreData = new CoreContext<TableItem>(manager)
                                               {
                                                   Model =
                                                       Activator.CreateInstance(
                                                           this.ModelType)
                                               };
                                this.PartTextBox.Text = "";
                                var arrWhereClause = new List<WhereClause>();
                                var repository = BaseDependency.Get<IProfileRepository>();
                                if (repository != null && coreData.Model is ProfileTable)
                                {
                                    ProfileItem profile = repository.CurrentProfile();
                                    if (profile != null)
                                    {
                                        string data = profile.CodeProfile;
                                        arrWhereClause.Add(
                                            new WhereClause(
                                                (coreData.Model as TableItem).TableName + ".KdProfile",
                                                Comparison.Equals,
                                                data));
                                    }
                                }
                                IEnumerable<TableItem> results =
                                    coreData.Take(dataGrid.LazyLoadItem).Where(arrWhereClause.ToArray()).Render();
                                var evt = new SourceEventArgs<TableItem>(results);
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

        private void ItemPanelOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Space || keyEventArgs.Key == Key.Enter)
            {
                if (keyEventArgs.OriginalSource is DependencyObject)
                {
                    var child = Manager.FindVisualChild<CoreCheckBox>(keyEventArgs.OriginalSource as DependencyObject);
                    if (child != null)
                    {
                        if (child.IsChecked.HasValue)
                        {
                            child.IsChecked = !child.IsChecked;
                        }
                    }
                }
            }
        }

        private void OnLoaded()
        {
            try
            {
                string path = this.ModelType.Module.FullyQualifiedName;
                string file = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".corefrm";
                if (!File.Exists(file))
                {
                    return;
                }
                XElement element = XElement.Load(file);
                XElement tableElement = element.Descendants("Table").FirstOrDefault(
                    n =>
                    {
                        XAttribute xAttribute = n.Attribute("Key");
                        return xAttribute != null && xAttribute.Value.Equals(this.ModelType.FullName);
                    });
                object item = Activator.CreateInstance(this.ModelType);
                object[] listHeaderAtribute = item.GetType().GetCustomAttributes(true);

                var grid = new FormGrid
                           {
                               VerticalAlignment = VerticalAlignment.Stretch,
                               HorizontalAlignment = HorizontalAlignment.Stretch,
                               Margin = new Thickness(0, 0, 0, 5)
                           };
                this.PanelForm = new PanelMetro
                                 {
                                     Title =
                                         listHeaderAtribute.Any(
                                             n => n.GetType() == typeof(DisplayFormAttribute))
                                             ? "Form "
                                               + ((DisplayFormAttribute)
                                         listHeaderAtribute.First(
                                             n => n.GetType() == typeof(DisplayFormAttribute))).Title
                                             : "Form Panel"
                                 };

                if (tableElement == null)
                {
                    return;
                }
                XElement titleElement = tableElement.Element("Title");
                if (titleElement != null)
                {
                    this.PanelForm.Title = "Form " + titleElement.Value.Trim();
                }
                else
                {
                    this.PanelForm.Title = "Form Panel";
                }
                XElement columnElement = tableElement.Element("Column");
                if (columnElement != null)
                {
                    this.CountColumn = !string.IsNullOrEmpty(columnElement.Value)
                        ? Convert.ToInt16(columnElement.Value.Trim())
                        : Convert.ToInt16(1);
                }
                else
                {
                    this.CountColumn = 1;
                }

                if (this.CountColumn == 0)
                {
                    this.CountColumn = 1;
                }

                var wrapPanelForm = new Grid();
                wrapPanelForm.Margin = new Thickness(0);
                wrapPanelForm.VerticalAlignment = VerticalAlignment.Stretch;
                wrapPanelForm.Height = double.NaN;
                PropertyInfo[] propertiInfos = item.GetType().GetProperties();

                for (int i = 0; i < this.CountColumn; i++)
                {
                    var widthGrid = new GridLength(
                        Convert.ToDouble(1) / Convert.ToDouble(this.CountColumn),
                        GridUnitType.Star);
                    wrapPanelForm.ColumnDefinitions.Add(new ColumnDefinition { Width = widthGrid });
                }
                int countRow =
                    tableElement.Descendants("Property")
                        .Where(
                            n =>
                                n.Element("FormInput") != null && n.Element("FormInput").Element("Index") != null
                                && !string.IsNullOrEmpty(n.Element("FormInput").Element("Index").Value))
                        .Max(n => Convert.ToInt16(n.Element("FormInput").Element("Index").Value)) / this.CountColumn + 1;
                if (countRow == 0)
                {
                    countRow =
                        tableElement.Descendants("Property")
                            .Count(
                                n =>
                                    n.Element("FormInput") != null && n.Element("FormInput").Element("Index") != null
                                    && !string.IsNullOrEmpty(n.Element("FormInput").Element("Index").Value))
                        / this.CountColumn + 1;
                }

                for (int i = 0; i < countRow; i++)
                {
                    wrapPanelForm.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(HeightRow, GridUnitType.Pixel) });
                }
                if (
                    tableElement.Element("Design")
                        .Elements("Property")
                        .Where(n => n.Element("FormInput").Element("Type").Value.Contains("InGrid"))
                        .Any())
                {
                    wrapPanelForm.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(300, GridUnitType.Star) });
                }

                this.CreateForm(tableElement, wrapPanelForm, countRow);

                foreach (XElement propertyInfo in
                    tableElement.Element("Design")
                        .Elements("Property")
                        .Select(n => n.Element("FormInput"))
                        .Where(
                            n =>
                                n.Element("ReportDisplay") != null
                                && n.Element("ReportDisplay").Value.Trim().Equals("true")))
                {
                    FrameworkElement triggerControl =
                        wrapPanelForm.Children.OfType<FrameworkElement>()
                            .FirstOrDefault(n => n.Name.Equals(propertyInfo.Parent.Attribute("Key").Value.Trim()));
                    if (triggerControl != null)
                    {
                        triggerControl.LostFocus += delegate(object sender, RoutedEventArgs args)
                        {
                            string triggerAtribute = "ReportDisplay";
                            if (triggerAtribute != null)
                            {
                                FrameworkElement reportDisplayControl =
                                    wrapPanelForm.Children.OfType<FrameworkElement>()
                                        .FirstOrDefault(n => n.Name.Equals(triggerAtribute));
                                if (reportDisplayControl is IValueElement && triggerControl is IValueElement)
                                {
                                    (reportDisplayControl as IValueElement).Value = (sender as IValueElement).Value;
                                }
                            }
                            ;
                        };
                    }
                }

                //PanelForm.Content = wrapPanelForm;
                //PanelForm.Background= new SolidColorBrush(Colors.Purple);
                //PanelForm.Margin=new Thickness(0);
                //PanelForm.VerticalAlignment=VerticalAlignment.Stretch;
                //grid.Children.Add(wrapPanelForm);
                this.PanelForm.Background = new SolidColorBrush(Colors.Blue);
                grid.Margin = new Thickness(0);
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                var buttonTemplate = new ButtonCommandTemplate { IsBusy = false };
                buttonTemplate.ResetItem += delegate
                {
                    this.PanelForm.Selecteditem = null;
                    this.PartTextBox.Text = "";
                };
                //buttonTemplate.SaveItem += (sender, args) => SaveData(PanelForm, item, dataGrid, buttonTemplate);
                //buttonTemplate.DeleteItem += (sender, args) => DeleteData(PanelForm, dataGrid, buttonTemplate);

                buttonTemplate.SetFormSubmit(this.PanelForm);
                this.MainContent = wrapPanelForm;
                this.FooterContent = new Grid();
                this.FooterContent.Children.Add(buttonTemplate);
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, " Load Form");
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            //foreach (
            //    IValueElement control in
            //        Manager.FindVisualChildren<FrameworkElement>(this.WrapPanelForm).OfType<IValueElement>())
            //{
            //    Console.WriteLine(control);
            //}
        }

        private void SaveData(
            PanelMetro panelForm,
            object item,
            CoreDataGrid dataGrid,
            ButtonCommandTemplate buttonTemplate)
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

                    if (manager.Commit())
                    {
                        if (isNewItem)
                        {
                            data.IsNew = false;
                            data.InitializeManager(manager);
                            var list = new List<TableItem> { data };
                            data.RebindDataToPrevious();
                            var evt = new SourceEventArgs<TableItem>(list);
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
                dataGrid.SelectedIndex = -1;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Saved Item");
            }
        }

        #endregion
    }
}