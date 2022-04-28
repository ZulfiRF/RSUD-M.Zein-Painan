using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Presenters;
using Core.Framework.Model;
using Core.Framework.Model.Attr;
using Core.Framework.Model.QueryBuilder.Clausa;
using Core.Framework.Model.QueryBuilder.Enums;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Implementations;
using Newtonsoft.Json;
using Binding = System.Windows.Data.Binding;
using DataGrid = System.Windows.Controls.DataGrid;
using Formatting = Newtonsoft.Json.Formatting;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Core.Framework.Windows.Helper
{
    public class FormHelperUseCoreSettingListView : BaseFormHelper
    {
        #region Constants

        private const int heightDataGridInput = 200;

        private const int heightRow = 45;

        #endregion

        #region Constructors and Destructors

        public FormHelperUseCoreSettingListView(Type modelType)
        {
            gridInput = new CoreListViewAsycn();
            gridInput.UsingVirtualization = true;
            ModelType = modelType;
            var rDictionary = new ResourceDictionary
            {
                Source =
                                          new Uri(
                                          string.Format(
                                              "/Core.Framework.Windows;component/Styles/Controls.Form.xaml"),
                                          UriKind.Relative)
            };
            Style = rDictionary["FormBasicStyle"] as Style;
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
                        Width = size.Width;
                    }
                    if (size.Height != 0)
                    {
                        Height = size.Height;
                    }
                    if (size.CountColumn != 0)
                    {
                        CountColumn = size.CountColumn;
                    }
                }
            }
        }

        #endregion

        #region Public Events

        public event EventHandler<ItemEventArgs> ComplateLoad;
        public event EventHandler<ItemEventArgs> DeleteComplete;

        public event EventHandler<SourceEventArgs<TableItem>> FilteringData;
        public event EventHandler<ItemEventArgs> SaveComplete;
        public event EventHandler<ItemEventArgs> UpdateComplete;

        #endregion

        #region Public Properties

        public int LazyLoadItem { get; set; }

        public Type ModelType { get; set; }
        public PanelMetro PanelForm { get; set; }

        #endregion

        #region Properties

        protected short CountColumn { get; set; }

        #endregion

        #region Public Methods and Operators


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnLoaded();
        }

        public void OnComplateLoad(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = ComplateLoad;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnDeleteComplete(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = DeleteComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnFilteringData(SourceEventArgs<TableItem> e)
        {
            EventHandler<SourceEventArgs<TableItem>> handler = FilteringData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnSaveComplete(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = SaveComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnUpdateComplete(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = UpdateComplete;
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
            item = (ModelType != null) ? Activator.CreateInstance(ModelType) : item;
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
                                    keyword)
                                { LogicOperator = LogicOperator.Or };
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
                                        keyword)
                                    { LogicOperator = LogicOperator.Or };
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
                                        foreach (
                                            PropertyInfo propertyType in
                                                Activator.CreateInstance(firstOrDefault.TypeModel)
                                                    .GetType()
                                                    .GetProperties()
                                                    .Where(
                                                        n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any())
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
                                                        keyword)
                                                    { LogicOperator = LogicOperator.Or };
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
                                                keyword)
                                            { LogicOperator = LogicOperator.Or };
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
                                                    keyword)
                                                { LogicOperator = LogicOperator.Or };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DataGridSelectedChanged(CoreListViewAsycn dataGrid, Grid wrapPanelForm, PanelMetro PanelForm)
        {
            try
            {
                if (dataGrid.SelectedItem == null)
                {
                    return;
                }
                IEnumerable<IValidateControl> children =
                    Manager.FindVisualChildren<FrameworkElement>(PanelForm).OfType<IValidateControl>();
                foreach (IValidateControl validateControl in children)
                {
                    if (validateControl.IsRequired)
                    {
                        validateControl.IsError = false;
                    }
                }
                object model;
                using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                {
                    model = manager.GetOneDataByFieldPrimary(dataGrid.SelectedItem);
                    PanelForm.Selecteditem = model;
                }

                foreach (FrameworkElement result in wrapPanelForm.Children.OfType<FrameworkElement>())
                {
                    if (result is DisplayWithTextBox)
                    {
                        var displayTextBoxTemp = result as DisplayWithTextBox;
                        object value =
                            model.GetType()
                                .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                .GetValue(model, null);
                        displayTextBoxTemp.Text = value != null ? value.ToString() : "";
                    }
                    else if (result is DisplayTextBoxWithCheckBox)
                    {
                        var displayTextBoxTemp = result as DisplayTextBoxWithCheckBox;
                        object value =
                            model.GetType()
                                .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                .GetValue(model, null);
                        displayTextBoxTemp.Text = value != null ? value.ToString() : "";
                        PropertyInfo propertyInfo = model.GetType().GetProperty("StatusEnabled");
                        if (propertyInfo != null)
                        {
                            value = propertyInfo.GetValue(model, null);
                            if (value.ToString().Equals("3") || value.ToString().Equals("1"))
                            {
                                displayTextBoxTemp.IsChecked = true;
                            }
                            else
                            {
                                displayTextBoxTemp.IsChecked = false;
                            }
                        }
                        if (displayTextBoxTemp.IsPrimary)
                            displayTextBoxTemp.IsReadOnly = true;
                    }
                    else if (result is DisplayWithCheckBox)
                    {
                        var displayTextBoxTemp = result as DisplayWithCheckBox;
                        if (displayTextBoxTemp.Tag is Type)
                        {
                            var converter = Activator.CreateInstance(displayTextBoxTemp.Tag as Type) as IValueConverter;
                            if (converter != null)
                            {
                                displayTextBoxTemp.IsChecked =
                                    converter.Convert(
                                        model.GetType()
                                            .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                            .GetValue(model, null),
                                        null,
                                        null,
                                        CultureInfo.InvariantCulture) as bool?;
                            }
                            else
                            {
                                displayTextBoxTemp.IsChecked =
                                    model.GetType()
                                        .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                        .GetValue(model, null) as bool?;
                            }
                        }
                        else
                        {
                            displayTextBoxTemp.IsChecked =
                                model.GetType()
                                    .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                    .GetValue(model, null) as bool?;
                        }
                    }
                    else if (result is DisplayWithComboBox)
                    {
                        var displayTextBoxTemp = result as DisplayWithComboBox;
                        PropertyInfo propertyInfo =
                            model.GetType()
                                .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture));
                        if (propertyInfo != null)
                        {
                            var reference =
                                propertyInfo.GetCustomAttributes(true)
                                    .FirstOrDefault(n => n.GetType() == typeof(ReferenceAttribute)) as
                                ReferenceAttribute;
                            if (reference != null)
                            {
                                propertyInfo =
                                    model.GetType()
                                        .GetProperty(reference.Property.ToString(CultureInfo.InvariantCulture));
                                displayTextBoxTemp.SelectedItem = propertyInfo.GetValue(model, null);
                                Thread.Sleep(10);
                            }
                        }
                    }
                    else if (result is DisplayWithTextBlock)
                    {
                        var displayTextBoxTemp = result as DisplayWithTextBlock;
                        displayTextBoxTemp.Text =
                            model.GetType()
                                .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                .GetValue(model, null)
                                .ToString();
                    }
                    else if (result is DisplayWithDateTime)
                    {
                        var displayDateTime = result as DisplayWithDateTime;
                        displayDateTime.Text =
                            model.GetType()
                                .GetProperty(displayDateTime.Name)
                                .GetValue(model, null)
                                .ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Datagrid Selection Changed");
            }
        }

        private void CreateForm(XElement tableElement, Grid wrapPanelForm, int countRow)
        {
            int row = 0;

            #region Create Form

            foreach (
                XElement propertiInfo in
                    tableElement.Element("Design")
                        .Elements("Property")
                        .OrderBy(n => Convert.ToByte(n.Element("FormInput").Element("Index").Value))
                        .Select(n => n.Element("FormInput")))
            {
                var itemProperty = Activator.CreateInstance(ModelType) as TableItem;
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
                        Width = wrapPanelForm.Width / CountColumn,
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
                        Width = wrapPanelForm.Width / CountColumn,
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
                        Key = propertiInfo.Parent.Attribute("Key").Value.Trim().Contains("Head") ?
                            propertiInfo.Parent.Attribute("Key").Value.Trim() :
                            propertiInfo.Element("ValuePath").Value.Trim(),
                        ValuePath = propertiInfo.Element("ValuePath").Value.Trim(),
                        //Key = Name != propertiInfo.Element("ValuePath").Value.Trim() ? Name : propertiInfo.Element("ValuePath").Value.Trim(),
                        Watermark =
                                                 "Ketik " + propertiInfo.Element("Title").Value.Trim()
                                                 + "..."
                    };



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
                        Width = wrapPanelForm.Width / CountColumn,
                        WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                        DisplayText = propertiInfo.Element("Title").Value.Trim(),
                        // Tag = attribute.Converter,
                        Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                    };
                }
                else if (propertiInfo.Element("Type").Value.Trim().Equals("DateTime"))
                {
                    displayTextbox = new DisplayWithDateTime
                    {
                        Margin = new Thickness(4),
                        Width = wrapPanelForm.Width / CountColumn,
                        WitdhDisplayText =
                            new GridLength(150, GridUnitType.Pixel),
                        DisplayText = propertiInfo.Element("Title").Value.Trim(),
                        // Tag = attribute.Converter,
                        Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                        AllowNull = fieldAttribute.IsAllowNull == SpesicicationType.AllowNull
                    };
                }
                else if (propertiInfo.Element("Type").Value.Trim().Equals("TextBoxWithCheckBox"))
                {
                    displayTextbox = new DisplayTextBoxWithCheckBox
                    {
                        Margin = new Thickness(4),
                        Width = wrapPanelForm.Width / CountColumn,
                        WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                        DisplayText =
                                                 propertiInfo.Element("Title").Value.Trim(),
                        Name =
                                                 propertiInfo.Parent.Attribute("Key")
                                                 .Value.Trim(),
                        IsReadOnly =
                                                 propertiInfo.Element("ReadOnly")
                                                        .Value.Trim()
                                                        .Equals("ReadOnly"),
                        //        IsReadOnly =
                        //propertiInfo.Element("IsPrimary")
                        //    .Value.Trim()
                        //    .Equals("true")
                        //|| propertiInfo.Element("ReadOnly")
                        //       .Value.Trim()
                        //       .Equals("ReadOnly"),
                        IsPrimary = propertiInfo.Element("IsPrimary")
                                                      .Value.Trim()
                                                      .Equals("true"),
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
                        Width = wrapPanelForm.Width / CountColumn,
                        WitdhDisplayText =
                                                 new GridLength(150, GridUnitType.Pixel),
                        DisplayText = propertiInfo.Element("Title").Value.Trim(),
                        //Tag = attribute.Converter,
                        Name = propertiInfo.Parent.Attribute("Key").Value.Trim(),
                    };
                }
                //IEnumerable<FieldAttribute> propertyField =
                //    propertyInfo.GetCustomAttributes(true).OfType<FieldAttribute>();
                //  FieldAttribute[] atributeFields = propertyField as FieldAttribute[] ?? propertyField.ToArray();
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


            //gridInput.CanUserAddRows = true;
            gridInput.Margin = new Thickness(0);
            gridInput.Width = wrapPanelForm.Width;
            gridInput.Height = heightDataGridInput;
            //gridInput.AutoGenerateColumns = false;
            gridInput.DataContext = new List<object> { Activator.CreateInstance(ModelType) };
            bool hasAddGridInput = false;
            var gridView = new GridView();
            foreach (
                XElement propertiInfo in
                    tableElement.Element("Design")
                        .Elements("Property")
                        .Where(n => n.Element("FormInput").Element("Type").Value.Contains("InGrid"))
                        .OrderBy(n => Convert.ToByte(n.Element("FormInput").Element("Index").Value))
                        .Select(n => n.Element("FormInput")))
            {

                hasAddGridInput = true;
                var itemProperty = Activator.CreateInstance(ModelType) as TableItem;
                FieldAttribute fieldAttribute =
                    itemProperty.GetType()
                        .GetProperty(propertiInfo.Parent.Attribute("Key").Value.Trim())
                        .GetCustomAttributes(true)
                        .OfType<FieldAttribute>()
                        .FirstOrDefault();
                GridViewColumn column;

                if (propertiInfo == null)
                {
                    continue;
                }

                if (propertiInfo.Element("Type").Value.Trim().Equals("ListaItemInGrid"))
                {
                    column = new GridViewColumn();
                    var textFactory = new FrameworkElementFactory(typeof(CoreComboBox));
                    textFactory.SetValue(CoreComboBox.UsingSearchByFrameworkProperty, true);
                    textFactory.SetValue(
                        ItemsControl.DisplayMemberPathProperty,
                        propertiInfo.Element("DisplayPath").Value.Trim());
                    textFactory.SetValue(CoreComboBox.KeyProperty, propertiInfo.Element("ValuePath").Value.Trim());
                    textFactory.SetValue(
                        CoreComboBox.IsRequiredProperty,
                        (fieldAttribute != null && fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull));

                    if (!string.IsNullOrEmpty(propertiInfo.Element("TypeModel").Value.Trim()))
                    {
                        textFactory.SetValue(
                            CoreComboBox.DomainNameSpacesProperty,
                            propertiInfo.Element("TypeModel").Value.Trim());
                    }
                    var textTemplate = new DataTemplate();
                    textTemplate.VisualTree = textFactory;
                    //  bind.Mode = BindingMode.TwoWay;
                    column.CellTemplate = textTemplate;
                }
                else
                {
                    column = new GridViewColumn();
                    var textFactory = new FrameworkElementFactory(typeof(TextBlock));
                    var bind = new Binding(propertiInfo.Parent.Element("Grid").Element("DisplayPath").Value.Trim());
                    bind.Mode = BindingMode.TwoWay;
                    textFactory.SetBinding(TextBlock.TextProperty, bind);
                    var textTemplate = new DataTemplate();
                    textTemplate.VisualTree = textFactory;
                    column.CellTemplate = textTemplate;

                    textFactory = new FrameworkElementFactory(typeof(CoreComboBox));
                    textFactory.SetValue(CoreComboBox.UsingSearchByFrameworkProperty, true);
                    textFactory.SetValue(
                        ItemsControl.DisplayMemberPathProperty,
                        propertiInfo.Element("DisplayPath").Value.Trim());
                    textFactory.SetValue(CoreComboBox.KeyProperty, propertiInfo.Element("ValuePath").Value.Trim());
                    textFactory.SetValue(
                        CoreComboBox.IsRequiredProperty,
                        (fieldAttribute != null && fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull));
                    bind =
                        new Binding(
                            propertiInfo.Parent.Element("Grid").Element("DisplayPath").Value.Trim().Split('.')[0]);
                    bind.Mode = BindingMode.TwoWay;
                    textFactory.SetValue(CoreComboBox.SelectedItemProperty, bind);
                    if (!string.IsNullOrEmpty(propertiInfo.Element("TypeModel").Value.Trim()))
                    {
                        textFactory.SetValue(
                            CoreComboBox.DomainNameSpacesProperty,
                            propertiInfo.Element("TypeModel").Value.Trim());
                    }

                    var comboTemplate = new DataTemplate();
                    comboTemplate.VisualTree = textFactory;

                }
                column.Header = propertiInfo.Element("Title").Value.Trim();
                Binding binding;
                if (string.IsNullOrEmpty(propertiInfo.Element("DisplayPath").Value.Trim()))
                {
                    binding = new Binding(propertiInfo.Parent.Attribute("Key").Value.Trim());
                }
                else
                {
                    binding = new Binding(propertiInfo.Element("DisplayPath").Value.Trim());
                }
                //column.
                //  column.Binding = binding;
                //if (attribute.Converter != null)
                //{
                //    var converter = Activator.CreateInstance(attribute.Converter) as IValueConverter;
                //    binding.Converter = converter;
                //}
                gridView.Columns.Add(column);
            }
            Grid.SetColumn(gridInput, 0);
            Grid.SetColumnSpan(gridInput, CountColumn);

            Grid.SetRow(gridInput, row - 1);
            if (hasAddGridInput)
            {
                wrapPanelForm.Children.Add(gridInput);
            }
        }

        private void DeleteData(PanelMetro PanelForm, CoreListViewAsycn dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            try
            {
                using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                {
                    var listItem = new List<object>();
                    if (dataGrid.SelectedItems != null)
                    {
                        if (Manager.Confirmation("Delete Item", "Apakah anda yakin akan menghapus data ?"))
                        {
                            foreach (object selectedItem in dataGrid.SelectedItems)
                            {
                                manager.DeleteObject(selectedItem as TableItem);
                                var tableItem = selectedItem as TableItem;
                                if (tableItem != null)
                                    listItem.Add(tableItem.GetDictionary());
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
                    Manager.ContentMessage("Informasi", "Data Berhasil Di Hapus");

                    buttonTemplate.ClearItem();
                    OnDeleteComplete(
                        new ItemEventArgs(
                            JsonConvert.SerializeObject(
                                listItem,
                                Formatting.None,
                                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
                    RefreshData(buttonTemplate, 0, dataGrid, buttonTemplate);
                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Delete Item");
            }
        }

        private void InitializeDataInGrid(FormGrid grid, CoreListViewAsycn dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            ThreadPool.QueueUserWorkItem(
                state => Manager.Timeout(
                    grid.Dispatcher,
                    () =>
                    {
                        try
                        {
                            using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                            {
                                var coreData = new CoreContext<TableItem>(manager)
                                {
                                    Model =
                                                           Activator.CreateInstance(
                                                               ModelType)
                                };
                                PartTextBox.Text = "";
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
                                IEnumerable results =
                                    coreData.Take(dataGrid.LazyLoadItem).Where(arrWhereClause.ToArray()).RenderData();
                                //var evt = new SourceEventArgs<TableItem>(results);
                                //OnFilteringData(evt);
                                dataGrid.DataContext = results;

                                buttonTemplate.IsBusy = false;
                            }
                        }
                        catch (Exception exception)
                        {
                            Manager.HandleException(exception, "Load Data");
                        }
                    }));
        }

        private void OnLoaded()
        {
            try
            {
                InModeSearch = true;
                string path = ModelType.Module.FullyQualifiedName;
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
                        return xAttribute != null && xAttribute.Value.Equals(ModelType.FullName);
                    });
                object item = Activator.CreateInstance(ModelType);
                object[] listHeaderAtribute = item.GetType().GetCustomAttributes(true);

                var grid = new FormGrid
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 0, 5),
                    Background = new SolidColorBrush(Colors.White)
                };

                PanelForm = new PanelMetro
                {
                    Title =
                                        listHeaderAtribute.Any(
                                            n => n.GetType() == typeof(DisplayFormAttribute))
                                            ? "Form "
                                              + ((DisplayFormAttribute)
                                                 listHeaderAtribute.First(
                                                     n => n.GetType() == typeof(DisplayFormAttribute))).Title
                                            : "Data Form"
                };
                Canvas.SetZIndex(PanelForm, -1);
                var PanelFormGrid = new PanelMetro
                {
                    Title =
                                                listHeaderAtribute.Any(
                                                    n => n.GetType() == typeof(DisplayFormAttribute))
                                                    ? "Data "
                                                      + ((DisplayFormAttribute)
                                                         listHeaderAtribute.First(
                                                             n => n.GetType() == typeof(DisplayFormAttribute))).Title
                                                    : "Data Grid"
                };
                if (tableElement == null)
                {
                    return;
                }
                XElement titleElement = tableElement.Element("Title");
                if (titleElement != null)
                {
                    PanelForm.Title = "Form " + titleElement.Value.Trim();
                    PanelFormGrid.Title = "Data " + titleElement.Value.Trim();
                    //PanelForm.Background = new SolidColorBrush(Colors.Red);
                    PanelFormGrid.Margin = new Thickness(0, 100, 0, 20);
                }
                else
                {
                    PanelForm.Title = "Form Panel";
                    PanelFormGrid.Title = "Data Panel";
                }
                XElement columnElement = tableElement.Element("Column");
                if (columnElement != null)
                {
                    CountColumn = !string.IsNullOrEmpty(columnElement.Value)
                                      ? Convert.ToInt16(columnElement.Value.Trim())
                                      : Convert.ToInt16(1);
                }
                else
                {
                    CountColumn = 1;
                }

                if (CountColumn == 0)
                {
                    CountColumn = 1;
                }

                var wrapPanelForm = new Grid { Height = 0 };
                PropertyInfo[] propertiInfos = item.GetType().GetProperties();

                for (int i = 0; i < CountColumn; i++)
                {
                    var widthGrid = new GridLength(
                        Convert.ToDouble(1) / Convert.ToDouble(CountColumn),
                        GridUnitType.Star);
                    wrapPanelForm.ColumnDefinitions.Add(new ColumnDefinition { Width = widthGrid });
                }
                int countRow =
                    tableElement.Descendants("Property")
                        .Where(
                            n =>
                            n.Element("FormInput") != null && n.Element("FormInput").Element("Index") != null
                            && !string.IsNullOrEmpty(n.Element("FormInput").Element("Index").Value))
                        .Max(n => Convert.ToInt16(n.Element("FormInput").Element("Index").Value)) / CountColumn + 1;
                if (countRow == 0)
                {
                    countRow =
                        tableElement.Descendants("Property")
                            .Where(
                                n =>
                                n.Element("FormInput") != null && n.Element("FormInput").Element("Index") != null
                                && !string.IsNullOrEmpty(n.Element("FormInput").Element("Index").Value))
                            .Count() / CountColumn + 1;
                }
                //propertiInfos.Where(n => n.GetCustomAttributes(true).Any(p => p is CoreInputAttribute)).Count() /
                //CountColumn + 1;
                for (int i = 0; i < countRow; i++)
                {
                    wrapPanelForm.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(heightRow, GridUnitType.Pixel) });
                    wrapPanelForm.Height += heightRow;
                }

                if (
                    tableElement.Element("Design")
                        .Elements("Property")
                        .Where(n => n.Element("FormInput").Element("Type").Value.Contains("InGrid"))
                        .Any())
                {
                    wrapPanelForm.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(heightRow, GridUnitType.Pixel) });
                    wrapPanelForm.Height += heightDataGridInput;
                }

                #region Create Grid

                var dataGrid = new CoreListViewAsycn();
                dataGrid.UsingVirtualization = true;
                var view = new GridView();
                foreach (
                    XElement propertiInfo in
                        tableElement.Element("Design").Elements("Property").Select(n => n.Element("Grid")))
                {
                    //}
                    //foreach (
                    //    PropertyInfo propertyInfo in
                    //        propertiInfos.Where(n => n.GetCustomAttributes(true).Any(p => p is CoreInputAttribute)).OrderBy(
                    //            n => n.GetCustomAttributes(true).OfType<CoreInputAttribute>().First().Index))
                    //{
                    GridViewColumn column;
                    //GridAttribute attribute =
                    //    propertyInfo.GetCustomAttributes(true).OfType<GridAttribute>().FirstOrDefault(
                    //        o => o.GetType().Name.Equals("GridAttribute"));
                    if (propertiInfo == null)
                    {
                        continue;
                    }


                    column = new GridViewColumn();
                    column.Header = propertiInfo.Element("Title").Value.Trim();
                    Binding binding;
                    if (string.IsNullOrEmpty(propertiInfo.Element("DisplayPath").Value.Trim()))
                    {
                        binding = new Binding(propertiInfo.Parent.Attribute("Key").Value.Trim());
                    }
                    else
                    {
                        binding = new Binding(propertiInfo.Element("DisplayPath").Value.Trim());
                    }
                    column.DisplayMemberBinding = binding;
                    //if (attribute.Converter != null)
                    //{
                    //    var converter = Activator.CreateInstance(attribute.Converter) as IValueConverter;
                    //    binding.Converter = converter;
                    //}
                    view.Columns.Add(column);
                }
                dataGrid.View = view;
                //dataGrid.EditMode = true;
                //dataGrid.IsReadOnly = true;


                #endregion Create Grid

                //wrapPanelForm.Width = Width;
                //grid.SizeChanged += (sender, args) =>
                //{
                //    foreach (var element in wrapPanelForm.Children.OfType<FrameworkElement>())
                //    {
                //        element.Width = args.NewSize.Width / CountColumn - 8;
                //    }
                //};
                wrapPanelForm.SizeChanged +=
                    (sender, args) => { PanelFormGrid.Margin = new Thickness(0, args.NewSize.Height + 40, 0, 20); };

                CreateForm(tableElement, wrapPanelForm, countRow);

                foreach (
                    XElement propertyInfo in
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
                        triggerControl.LostFocus += delegate (object sender, RoutedEventArgs args)
                                                        {
                                                            string triggerAtribute = "ReportDisplay";
                                                            if (triggerAtribute != null)
                                                            {
                                                                FrameworkElement reportDisplayControl =
                                                                    wrapPanelForm.Children.OfType<FrameworkElement>()
                                                                        .FirstOrDefault(
                                                                            n => n.Name.Equals(triggerAtribute));
                                                                if (reportDisplayControl is IValueElement &&
                                                                    triggerControl is IValueElement)
                                                                {
                                                                    (reportDisplayControl as IValueElement).Value =
                                                                        (sender as IValueElement).Value;
                                                                }
                                                            }
                                                            ;
                                                        };
                    }
                }
                dataGrid.MouseDoubleClick +=
                    (sender, args) => DataGridSelectedChanged(dataGrid, wrapPanelForm, PanelForm);
                dataGrid.KeyDown += (sender, args) =>
                                        {
                                            InModeSearch = true;
                                            if (PartTextBox != null)
                                            {
                                                PartTextBox.Focus();
                                            }
                                        };
                dataGrid.LazyLoadItem = LazyLoadItem;
                dataGrid.VerticalAlignment = VerticalAlignment.Stretch;
                dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                dataGrid.Margin = new Thickness(0);
                PanelForm.Content = wrapPanelForm;
                PanelFormGrid.Content = dataGrid;
                grid.Children.Add(PanelFormGrid);
                Grid.SetRowSpan(PanelForm, 2);
                grid.Children.Add(PanelForm);
                dataGrid.SizeChanged += (sender, args) =>
                                            {
                                                var dataGrid1 = sender as DataGrid;
                                                if (dataGrid1 != null)
                                                {
                                                    dataGrid1.Tag = args.NewSize.Height;
                                                }
                                            };
                var buttonTemplate = new ButtonCommandTemplate { IsBusy = false };
                buttonTemplate.ResetItem += delegate
                                                {
                                                    buttonTemplate.IsBusy = true;
                                                    foreach (FrameworkElement result in wrapPanelForm.Children.OfType<FrameworkElement>())
                                                    {
                                                        if (result is DisplayTextBoxWithCheckBox)
                                                        {
                                                            var displayTextBoxTemp = result as DisplayTextBoxWithCheckBox;
                                                            if (PanelForm.Selecteditem != null)
                                                            {
                                                                object value =
                                                                    PanelForm.Selecteditem.GetType()
                                                                        .GetProperty(displayTextBoxTemp.Name.ToString(CultureInfo.InvariantCulture))
                                                                        .GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();

                                                                if (displayTextBoxTemp.IsPrimary && value == null)
                                                                    displayTextBoxTemp.IsReadOnly = false;
                                                                else
                                                                    displayTextBoxTemp.IsReadOnly = true;
                                                            }
                                                        }

                                                    }
                                                    PanelForm.Selecteditem = null;
                                                    dataGrid.SelectedIndex = -1;
                                                    PartTextBox.Text = "";
                                                    RefreshData(buttonTemplate, 0, dataGrid, buttonTemplate);

                                                };
                buttonTemplate.SaveItem +=
                    (sender, args) => SaveData(PanelForm, item, dataGrid, buttonTemplate);
                buttonTemplate.DeleteItem += (sender, args) => DeleteData(PanelForm, dataGrid, buttonTemplate);
                buttonTemplate.SetFormSubmit(PanelForm);

                MainContent = grid;
                FooterContent = new Grid() { Background = new SolidColorBrush(Colors.White) };
                FooterContent.Children.Add(buttonTemplate);
                if (PartTextBox != null)
                {
                    PartTextBox.LostFocus += delegate
                                                 {
                                                     if (dataGrid.DataContext == null)
                                                     {
                                                         return;
                                                     }
                                                     if (!(dataGrid.DataContext as IEnumerable).Cast<object>().Any())
                                                     {
                                                         InitializeDataInGrid(grid, dataGrid, buttonTemplate);
                                                     }
                                                     PartTextBox.Text = "";
                                                 };
                    PartTextBox.KeyUp +=
                        (sender, args) => PartTexboxKeyUp(args, dataGrid, buttonTemplate, PanelForm);
                    var searchText = new StringBuilder();
                    searchText.Append("Ketik ");
                    //dataGrid.SelectionMode = DataGridSelectionMode.Single;
                    foreach (PropertyInfo property in
                        propertiInfos.Where(n => n.GetCustomAttributes(true).OfType<SearchAttribute>().Any()))
                    {
                        searchText.Append(
                            property.GetCustomAttributes(true).OfType<SearchAttribute>().First().Title + " atau  ");
                    }
                    string resultTextSearch = searchText.ToString();
                    resultTextSearch = resultTextSearch.Substring(0, resultTextSearch.Length - 2);
                    TextboxHelper.SetWatermark(
                        PartTextBox,
                        resultTextSearch + " untuk mecari data di dalam data grid");
                    OnComplateLoad(new ItemEventArgs(null));
                    RefreshData(buttonTemplate, 0, dataGrid, buttonTemplate);

                }
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, " Load Form");
            }
        }

        private void PartTexboxKeyUp(
            KeyEventArgs args,
            CoreListViewAsycn dataGrid,
            ButtonCommandTemplate buttonTemplate,
            PanelMetro PanelForm)
        {
            if (args.Key == Key.Up)
            {
                //InModeSearch = false;
                if (dataGrid != null)
                {

                }
            }
            else if (args.Key == Key.Return)
            {
                //ThreadPool.QueueUserWorkItem(state => Manager.Timeout(grid.Dispatcher, () =>
                //    {
                try
                {
                    using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                    {
                        var coreData = new CoreContext<TableItem>(manager);
                        coreData.Model = Activator.CreateInstance(ModelType);
                        List<WhereClause> arrWhereClause =
                            GenereateClause<TableItem>(PartTextBox.Text).ToList();
                        var repository = BaseDependency.Get<IProfileRepository>();
                        if (repository != null && coreData.Model is ProfileTable)
                        {
                            ProfileItem profile = repository.CurrentProfile();
                            if (profile != null)
                            {
                                string data = profile.CodeProfile;
                                // arrWhereClause.Add(new WhereClause((coreData.Model as TableItem).TableName + ".KdProfile", Comparison.Equals, data));
                                coreData.BeforeExecuteQuery += delegate (object sender, QueryArgs queryArgs)
                                                                   {
                                                                       var tableItem = coreData.Model as TableItem;
                                                                       if (tableItem != null)
                                                                       {
                                                                           queryArgs.Command += " and " +
                                                                                                tableItem.TableName +
                                                                                                ".KdProfile='" + data +
                                                                                                "'";
                                                                       }
                                                                   };
                            }
                        }
                        WhereClause[] whereClause = arrWhereClause.ToArray();

                        IEnumerable results =
                            coreData.Take(dataGrid.LazyLoadItem).Where(whereClause).RenderData();
                        //var evt = new SourceEventArgs<TableItem>(results);
                        //OnFilteringData(evt);

                        dataGrid.DataContext = results;
                        buttonTemplate.ClearItem();
                        if (results.GetEnumerator().Current != null)
                        {
                            PanelForm.Selecteditem = null;
                            dataGrid.SelectedIndex = -1;
                            //PartTextBox.Text = "";
                            //RefreshData(grid, 0, dataGrid, buttonTemplate);
                        }
                        buttonTemplate.IsBusy = false;
                    }
                }
                catch (Exception exception)
                {
                    Manager.HandleException(exception, "Search Data");
                }
                // }), PartTextBox.Text);
            }
            else if (args.Key == Key.Up)
            {
                //
                if ((dataGrid.DataContext as IEnumerable).Cast<object>().Count() != 0)
                {
                    dataGrid.SelectedIndex = 0;
                    if (dataGrid.Items != null && dataGrid.Items.Count > 0)
                    {
                        var firstRow =
                            dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.Items[0]) as DataGridRow;
                        if (firstRow != null)
                        {
                            firstRow.IsSelected = true;
                            //firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            // firstRow.Focus();
                            FocusManager.SetIsFocusScope(firstRow, true);
                            FocusManager.SetFocusedElement(firstRow, firstRow);
                        }
                    }
                }
            }
        }

        private void RefreshData(UserControl grid, int args, CoreListViewAsycn dataGrid, ButtonCommandTemplate buttonTemplate)
        {
            Manager.Timeout(
                grid.Dispatcher,
                () =>
                {
                    try
                    {
                        //                 <Setter Property="ListView.T">
                        //    <Setter.Value>
                        //        <GroupStyle>
                        //            <GroupStyle.HeaderTemplate>
                        //                <DataTemplate>
                        //                    <TextBlock FontWeight="Bold" FontSize="14" Text="{Binding Name}"/>
                        //                </DataTemplate>
                        //            </GroupStyle.HeaderTemplate>
                        //        </GroupStyle>
                        //    </Setter.Value>
                        //</Setter>
                        var groupStyle = new GroupStyle();
                        var textFactory = new FrameworkElementFactory(typeof(TextBlock));
                        //textFactory.SetValue(TextBlock.TextProperty, true);
                        var bind = new Binding("Name");
                        bind.Mode = BindingMode.Default;
                        textFactory.SetBinding(TextBlock.TextProperty, bind);
                        textFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
                        //textFactory.SetValue(TextBlock.FontSizeProperty,Convert.ToDouble(14));                        
                        var textTemplate = new DataTemplate();
                        textTemplate.VisualTree = textFactory;
                        groupStyle.HeaderTemplate = textTemplate;
                        dataGrid.GroupStyle.Add(groupStyle);
                        using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                        {
                            var coreData = new CoreContext<TableItem>(manager);
                            //var coreData = new CoreQueryable<TableItem>(manager);
                            coreData.Model = Activator.CreateInstance(ModelType);
                            IEnumerable results =
                                coreData.Skip(args + 1)
                                    .Where(GenereateClause<TableItem>(PartTextBox.Text).ToArray())
                                    .RenderData().Cast<TableItem>()
                                    .Take(dataGrid.LazyLoadItem);

                            //var evt = new SourceEventArgs<TableItem>(results);
                            //OnFilteringData(evt);
                            if (args == 0)
                            {
                                var a = VirtualizingStackPanel.GetIsVirtualizing(dataGrid);

                                Manager.Timeout(Dispatcher, () =>
                                                                {
                                                                    dataGrid.DataContext = results;
                                                                    CollectionView view =
                                                                        (CollectionView)
                                                                        CollectionViewSource.GetDefaultView(
                                                                            dataGrid.ItemsSource);
                                                                    var model = Activator.CreateInstance(ModelType);

                                                                    var groups =
                                                                        model.GetType()
                                                                            .GetProperties()
                                                                            .Where(
                                                                                n =>
                                                                                    n.GetCustomAttributes(true)
                                                                                        .OfType<GroupByAttribute>()
                                                                                        .Any());
                                                                    foreach (var source in groups)
                                                                    {
                                                                        PropertyGroupDescription groupDescription = new PropertyGroupDescription(source.Name);
                                                                        view.GroupDescriptions.Add(groupDescription);
                                                                    }
                                                                });



                            }
                            else
                            {
                                //dataGrid.AddItemSource(evt.ListSource);
                            }
                            buttonTemplate.IsBusy = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        Manager.HandleException(exception, "Refresh Data");
                    }
                });
        }

        private void SaveData(
            PanelMetro panelForm,
            object item,
            CoreListViewAsycn dataGrid,
            ButtonCommandTemplate buttonTemplate)
        {
            try
            {
                using (var manager = new ContextManager(ConnectionString, ManagerConnection))
                {
                    TableItem data = null;
                    bool isNewItem = false;
                    if (panelForm.Selecteditem is TableItem)
                    {
                        bool context = Manager.Confirmation("Update Item", "Apakah anda yakin akan merubah data ?");
                        if (context)
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
                            //var list = new List<TableItem> { data.GetDictionary() };
                            data.RebindDataToPrevious();
                            //var evt = new SourceEventArgs<TableItem>(list);
                            //dataGrid.AddItemSource(evt.ListSource);
                            OnSaveComplete(
                                new ItemEventArgs(
                                    JsonConvert.SerializeObject(
                                        data.GetDictionary(),
                                        Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            ReferenceLoopHandling =
                                                    ReferenceLoopHandling.Ignore
                                        })));
                            Manager.ContentMessage("Informasi", "Data berhasil disimpan");
                            //Message = new MessageItem() { Content = "Data berhasil disimpan", MessageType = MessageType.Success };
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
                            OnUpdateComplete(
                                new ItemEventArgs(
                                    JsonConvert.SerializeObject(
                                        data.GetDictionary(),
                                        Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            ReferenceLoopHandling =
                                                    ReferenceLoopHandling.Ignore
                                        })));
                            Manager.ContentMessage("Informasi", "Data berhasil diubah");
                            //Message = new MessageItem() { Content = "Data berhasil diubah", MessageType = MessageType.Success };

                        }
                        buttonTemplate.ClearItem();
                    }
                }
                dataGrid.SelectedIndex = -1;
                RefreshData(panelForm, 0, dataGrid, buttonTemplate);
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Saved Item");
                //Message = new MessageItem() { Exceptions = exception, MessageType = MessageType.Error };
            }
        }


        #endregion

        public CoreListViewAsycn gridInput { get; set; }
    }
}