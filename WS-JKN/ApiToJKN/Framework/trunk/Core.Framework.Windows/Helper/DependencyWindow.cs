using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Helper
{
    /// <summary>
    /// Interaction logic for DependencyWindow.xaml
    /// </summary>
    public partial class DependencyWindow
    {
        public DependencyWindow()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            listMain.MouseDoubleClick += ListMainOnMouseDoubleClick;
            dgcKey.ChangeState += DgcKeyOnChangeState;
            dgcValue.ChangeState += DgcValueOnChangeState;
            gridCondition.CanUserAddRows = true;
            btnDelete.Click += BtnDeleteOnClick;
            btnSave.Click += BtnSaveOnClick;
            KeyDown += OnKeyDown;
            IsNew = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                IsNew = true;
                cbKey.ClearValueControl();
                cbValue.ClearValueControl();
                gridCondition.ItemsSource = new List<KeyValue>()
                {
                    new KeyValue()
                };
                //var list = listMain.ItemsSource as List<DependencyValue>;
                //if (list != null)
                //{
                //    var item = new DependencyValue();
                //    list.Add(item);
                //    listMain.ItemsSource = list;
                //    listMain.Items.Refresh();
                //    listMain.SelectedItem = item;
                //}
            }
        }

        private void BtnSaveOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var list = listMain.ItemsSource as List<DependencyValue>;
            if (list != null)
            {

                var item = new DependencyValue();
                item.Key = cbKey.Value;
                item.Value = cbValue.Value;
                item.Conditions = gridCondition.ItemsSource as List<KeyValue>;
                if (IsNew)
                    list.Add(item);
                else
                {
                    CurrentItem.Key = cbKey.Value;
                    CurrentItem.Value = cbValue.Value;
                    CurrentItem.Conditions = gridCondition.ItemsSource as List<KeyValue>;

                }

                listMain.ItemsSource = list;
                listMain.Items.Refresh();
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = location + "\\" +
                               (string.IsNullOrEmpty(ConfigurationFile) ? "CoreInjenction.diconfig" : ConfigurationFile);
                var doc = new XElement("DependencyInjections");

                foreach (var dependencyValue in list.Where(n => n.Value != null && n.Key != null))
                {
                    var elemet = new XElement("dependency", new XAttribute("contract", dependencyValue.Key)
                        , new XAttribute("implementation", dependencyValue.Value));
                    if (dependencyValue.Conditions.Any(n => n.Key != null && !string.IsNullOrEmpty(n.Key.ToString())))
                    {
                        elemet.Add(new XAttribute("usecondition", true));
                        var element = new XElement("conditions");
                        elemet.Add(element);
                        foreach (var condition in dependencyValue.Conditions.Where(n => n.Key != null && n.Value != null))
                        {
                            element.Add(new XElement("condition", new XAttribute("Key", condition.Key), new XAttribute("Value", condition.Value)));
                        }
                    }
                    doc.Add(elemet);

                }
                doc.Save(filePath);
            }
        }

        private void BtnDeleteOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var list = listMain.ItemsSource as List<DependencyValue>;
            if (list != null)
            {
                list.Remove(listMain.SelectedItem as DependencyValue);
                listMain.ItemsSource = list;
                listMain.Items.Refresh();
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = location + "\\" +
                               (string.IsNullOrEmpty(ConfigurationFile) ? "CoreInjenction.diconfig" : ConfigurationFile);
                var doc = new XElement("DependencyInjections");

                foreach (var dependencyValue in list)
                {
                    var elemet = new XElement("dependency", new XAttribute("contract", dependencyValue.Key)
                        , new XAttribute("implementation", dependencyValue.Value));
                    if (dependencyValue.Conditions.Any(n => n.Key != null && !string.IsNullOrEmpty(n.Key.ToString())))
                    {
                        elemet.Add(new XAttribute("usecondition", true));
                        var element = new XElement("conditions");
                        elemet.Add(element);
                        foreach (var condition in dependencyValue.Conditions)
                        {
                            elemet.Add(new XElement("condition", new XAttribute("Key", condition.Key), new XAttribute("Value", condition.Value)));
                        }
                    }
                    doc.Add(elemet);

                }
                doc.Save(filePath);
            }

        }


        private void DgcValueOnChangeState(object sender, ItemEventArgs<CoreDataGridTextBlockTextColumn.StateInfo> e)
        {
            if (e.Item == CoreDataGridTextBlockTextColumn.StateInfo.Commit)
            {
                //var item = gridCondition.CreateNewItem();
                gridCondition.SelectedIndex++;
                dgcKey.Focus(gridCondition.SelectedItem);
            }
        }

        private void DgcKeyOnChangeState(object sender, ItemEventArgs<CoreDataGridTextBlockTextColumn.StateInfo> e)
        {
            if (e.Item == CoreDataGridTextBlockTextColumn.StateInfo.Commit)
            {
                dgcValue.Focus(gridCondition.SelectedItem);
            }
        }

        private void ListMainOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var dependencyValue = listMain.SelectedItem as DependencyValue;
            if (dependencyValue != null)
            {
                cbKey.Value = dependencyValue.Key;
                Rebind();
                cbValue.Value = dependencyValue.Value;
                IsNew = false;
                CurrentItem = dependencyValue;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var list = new List<DependencyValue>();
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = location + "\\" +
                           (string.IsNullOrEmpty(ConfigurationFile) ? "CoreInjenction.diconfig" : ConfigurationFile);
            if (File.Exists(filePath))
            {
                var xdocument = XElement.Load(filePath);
                foreach (var descendant in xdocument.Descendants("dependency"))
                {
                    try
                    {
                        var keyValue = new DependencyValue();
                        keyValue.Key = HelperManager.GetType(descendant.Attribute("contract").Value);
                        keyValue.Value = HelperManager.GetType(descendant.Attribute("implementation").Value);
                        var conditions = new List<KeyValue>();
                        if (descendant.Attribute("usecondition") != null)
                        {

                            foreach (var xElement in descendant.Descendants("condition"))
                            {
                                var itemValue = new KeyValue()
                                {
                                    Key = xElement.Attribute("Key").Value.ToString(),
                                    Value = xElement.Attribute("Value").Value.ToString(),
                                };
                                conditions.Add(itemValue);
                            }
                            keyValue.Conditions = conditions;
                        }
                        else
                        {
                            conditions.Add(new KeyValue());
                            keyValue.Conditions = conditions;
                        }
                        list.Add(keyValue);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                    }

                }
            }
            listMain.ItemsSource = list;
            cbKey.SelectionChanged += CbKeyOnSelectionChanged;
            cbKey.ItemsSource = HelperManager.GetTypes().Where(n => n.IsInterface).Distinct();
        }

        private void CbKeyOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Rebind();
        }

        private void Rebind()
        {
            var currentType = cbKey.SelectedItem as Type;
            var items = new List<Type>();

            foreach (var type in HelperManager.GetTypes().Where(n => n.IsClass))
            {
                if (type.FullName.Contains("AuthenticationLogin"))
                {
                    Console.WriteLine();
                }
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface.FullName != null && @interface.FullName.Equals(currentType.FullName))
                    {
                        items.Add(type);
                        break;
                    }

                }
            }
            cbValue.ItemsSource = items;
            var dependencyValue = listMain.SelectedItem as DependencyValue;
            if (dependencyValue != null)
                gridCondition.ItemsSource = dependencyValue.Conditions;
        }

        public string ConfigurationFile { get; set; }

        public bool IsNew { get; set; }

        internal DependencyValue CurrentItem { get; set; }
    }
}