using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Model;
using Core.Framework.Model.Attr;
using Core.Framework.Windows.Contracts;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Core.Framework.Windows.DesignForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            tbDestination.GotFocus += TbDestinationOnGotFocus;
           // tbDestination.Text = @"D:\Work\Gawe-Gawe\MEDIKALRECORD\Medical Record new\MD\Domain\Medifirst.Domain.MedicalRecords\bin\Debug";
            
            btnGenereate.Click += BtnGenereateOnClick;
            LbDataTable.SelectionChanged += LbDataTableOnSelectionChanged;
            btnSave.Click += BtnSaveOnClick;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            LbSearch.TextChanged += LbSearchOnTextChanged;

        }

        private void LbSearchOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            LbDataTable.Items.Clear();
            foreach (var source in listType.Where(n => n.Name.ToLower().Contains(LbSearch.Text.ToLower())))
            {
                LbDataTable.Items.Add(source);
            }
        }

        private void BtnSaveOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (LbDataTable.SelectedItem == null) return;
            if (Helper.Manager.Confirmation("Konfirmasi", "Apakah anda yakin akan menyimpan data"))
            {
                var type = LbDataTable.SelectedItem as Type;
                if (type != null)
                {
                    var path = type.Module.FullyQualifiedName;
                    var file =
                        Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) +
                        ".corefrm";
                    XElement element = null;
                    if (!File.Exists(file))
                    {
                        //var design = new XElement("Design");
                        //foreach (var item in Activator.CreateInstance(type).GetType().GetProperties())
                        //{
                        //    var property = new XElement("Property", new XAttribute("Key", item.Name));
                        //    var formInput = new XElement("FormInput");
                        //    var titleElement = new XElement("Title", "");
                        //    var indexElement = new XElement("Index", "");
                        //    var typeElement = new XElement("Type", "");
                        //    var typeModelElement = new XElement("TypeModel", "");
                        //    var displayPathElement = new XElement("DisplayPath", "");
                        //    var valuePathElement = new XElement("ValuePath", "");
                        //    formInput.Add(titleElement);
                        //    formInput.Add(indexElement);
                        //    formInput.Add(typeElement);
                        //    formInput.Add(displayPathElement);
                        //    formInput.Add(typeModelElement);
                        //    formInput.Add(valuePathElement);
                        //    property.Add(formInput);
                        //    design.Add(property);
                        //}
                        //var table = new XElement("Table", new XAttribute("Key", type.FullName), design);
                        element = new XElement("Tables", new XElement("Table"));
                        element.Save(file);
                    }
                    else
                    {
                        element = XElement.Load(file);
                    }
                    //      var model = Activator.CreateInstance(type) as TableItem;
                    var tableElement = element.Descendants("Table").FirstOrDefault(n => n.Attribute("Key") != null && n.Attribute("Key").Value.Equals(type.FullName));
                    if (tableElement != null) tableElement.Remove();
                    element.Save(file);
                    var design = new XElement("Design");

                    foreach (var item in tbControlPanel.Items)
                    {
                        var tabItem = item as TabItem;
                        if (tabItem != null)
                        {
                            var tabPanel = tabItem.Content as DetailControl;
                            if (tabPanel != null)
                            {
                                if (tabPanel.cbUse.IsChecked != null && tabPanel.cbUse.IsChecked.Value == false) continue;
                                var property = new XElement("Property",
                                                            new XAttribute("Key", tabPanel.CurrentProperty.Name));
                                var formInput = new XElement("FormInput");
                                foreach (
                                    var valueElement in
                                        tabPanel.GetDataForm().OfType<FrameworkElement>().OrderBy(n => n.Tag))
                                {

                                    var elementData = valueElement.Tag.ToString().Replace("FormInput-", "");
                                    if (string.IsNullOrEmpty(elementData)) continue;
                                    var valueElement1 = valueElement as IValueElement;
                                    if (valueElement1 != null)
                                    {
                                        var titleElement = new XElement(elementData, valueElement1.Value);

                                        formInput.Add(titleElement);
                                    }



                                }
                                property.Add(formInput);
                                formInput = new XElement("Grid");
                                foreach (
                                    var valueElement in
                                        tabPanel.GetDataGrid().OfType<FrameworkElement>().OrderBy(n => n.Tag))
                                {

                                    var elementData = valueElement.Tag.ToString().Replace("Grid-", "");
                                    var valueElement1 = valueElement as IValueElement;
                                    if (valueElement1 != null)
                                    {
                                        var titleElement = new XElement(elementData, valueElement1.Value);

                                        formInput.Add(titleElement);
                                    }

                                }
                                property.Add(formInput);
                                design.Add(property);
                            }

                        }
                    }
                    var tableData = new XElement("Table", new XAttribute("Key", type.FullName), design, new XElement("Title", tbTitle.Text), new XElement("Column", cmbSizeColumn.SelectedItem));


                    element.Add(tableData);
                    element.Save(file);
                }
            }
        }

        private void LbDataTableOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (LbDataTable.SelectedItem != null)
            {
                cmbSizeColumn.ItemsSource = new int[] { 1, 2, 3, 4, 5 };
                var type = LbDataTable.SelectedItem as Type;
                if (type != null)
                {
                    var path = type.Module.FullyQualifiedName;
                    var file =
                        Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) +
                                      ".corefrm";
                    XElement
                    element = null;
                    if (File.Exists(file))
                        element = XElement.Load(file);

                    tbControlPanel.Items.Clear();
                    var data = Activator.CreateInstance(LbDataTable.SelectedItem as Type) as TableItem;
                    tbTitle.Text = (LbDataTable.SelectedItem as Type).Name.ConvertToStatementText();
                    cmbSizeColumn.SelectedItem = 2;
                    if (element != null)
                    {
                        XElement tableElement = element.Descendants("Table").FirstOrDefault(n => n.Attribute("Key") != null && n.Attribute("Key").Value.Equals(type.FullName));
                        if (tableElement != null)
                        {
                            var titleElement = tableElement.Element("Title");
                            if (titleElement != null)
                                tbTitle.Text = titleElement.Value.Trim();
                            else
                                tbTitle.Text = "";
                            var columnElement = tableElement.Element("Column");
                            if (columnElement != null)
                            {
                                if (!string.IsNullOrEmpty(columnElement.Value))
                                    cmbSizeColumn.SelectedItem = Convert.ToInt32(columnElement.Value.Trim());
                                else
                                    cmbSizeColumn.SelectedItem = 1;
                            }
                            else
                                cmbSizeColumn.SelectedItem = 1;
                        }
                        else
                        {
                            tbTitle.Text = "";
                            cmbSizeColumn.SelectedItem = 1;
                        }
                    }
                    foreach (var propertyInfo in data.GetType().GetProperties())
                    {
                        if (propertyInfo.Name.Equals("TableName")
                            || propertyInfo.Name.Equals("Item")
                            || propertyInfo.Name.Equals("KdHistoryLoginI")
                            || propertyInfo.Name.Equals("KdHistoryLoginU")
                            || propertyInfo.Name.Equals("KdHistoryLoginS")
                            || propertyInfo.Name.Equals("IsNew")
                            || propertyInfo.Name.Equals("KdProfile")
                            || propertyInfo.Name.Equals("StatusEnabled")
                            || propertyInfo.Name.Equals("AutoDropTable")) continue;
                        if (propertyInfo.Name[0].Equals('Q') && !propertyInfo.Name[1].Equals('t')) continue;
                        var content = new DetailControl()
                                          {
                                              Margin = new Thickness(0)
                                          };
                        content.Element = element;
                        content.BindingData(propertyInfo);
                        content.RootData = type;
                        tbControlPanel.Items.Add(new TabItem()
                                                     {
                                                         Header = propertyInfo.Name,
                                                         Content = content
                                                     });
                    }
                }
            }
        }

        private void BtnGenereateOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            HelperManager.RegisterModule(tbDestination.Text);
            LbDataTable.Items.Clear();
            SyncronDirectory(tbDestination.Text);
            foreach (var item in LbDataTable.Items)
            {
                listType.Add(item as Type);
            }
        }

        List<Type> listType = new List<Type>();
        private void SyncronDirectory(string location)
        {
            if (!Directory.Exists(location))
                location = Path.GetDirectoryName(location);
            foreach (var directory in Directory.GetDirectories(location))
            {
                SyncronDirectory(directory);
            }

            var files = Directory.GetFiles(location).Where(n => n.Contains("Medifirst.Domain") && n.EndsWith(".dll"));
            foreach (var file in files)
            {
                try
                {
                    foreach (var exportedType in Assembly.LoadFile(file).GetExportedTypes())
                    {
                        try
                        {
                            if (exportedType.IsClass)
                            {
                                try
                                {
                                    if (Activator.CreateInstance(exportedType) as TableItem != null)
                                    {
                                        LbDataTable.Items.Add(exportedType);
                                    }
                                }
                                catch (Exception e)
                                {
                                }                                
                            }
                        }
                        catch (Exception e)
                        {
                        }
                        

                    }
                }
                catch (Exception exception)
                {
                }

            }
        }
        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {


                //.WriteLine(args);
                var directory = Path.GetDirectoryName(args.RequestingAssembly.Location);
                if (Directory.Exists(directory))
                {
                    foreach (var varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                    {
                        if (varible.Contains("Core.Framework.Windows")) continue;
                        var assembly = Assembly.LoadFile(varible);
                        if (assembly.FullName.Equals(args.Name))
                        {
                            return assembly;
                        }
                    }
                    return FindAssembly(Path.GetDirectoryName(directory), args.Name);
                }
                return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private Assembly FindAssembly(string directory, string name)
        {
            if (Directory.Exists(directory))
            {
                foreach (var varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                {
                    if (varible.Contains("Core.Framework.Windows")) continue;
                    var assembly = Assembly.LoadFile(varible);
                    if (assembly.FullName.Equals(name))
                    {
                        return assembly;
                    }
                }
                return FindAssembly(Path.GetDirectoryName(directory), name);
            }
            return null;
        }

        private void TbDestinationOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Library (.dll) |*.dll";
            var result = dialog.ShowDialog();
            if (result.Value)
            {
                tbDestination.Text = dialog.FileName;
            }
        }
    }
}
