using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Model;
using Core.Framework.Model.Attr;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.DesignForm
{
    /// <summary>
    /// Interaction logic for DetailControl.xaml
    /// </summary>
    public partial class DetailControl
    {
        public DetailControl()
        {
            InitializeComponent();
            TbIndex.LostFocus += TbIndexOnLostFocus;
            cbPrimary.IsChecked = false;
            cbRequired.IsChecked = false;
            cbUse.IsChecked = false;
            cmbTypeForm.SelectionChanged += CmbTypeFormSelectionChanged;
            cmbDisplayPath.SelectionChanged += CmbDisplayPathOnSelectionChanged;
        }

        private void CmbDisplayPathOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var arr = cmbTypeModel.Text.Split('.');
            foreach (var customAtribute in CurrentProperty.GetCustomAttributes(true))
            {
                if (customAtribute is ReferenceAttribute)
                {
                    var referenceAtribute = customAtribute as ReferenceAttribute;
                    tbDsiplayPathGrid.Text = referenceAtribute.Property + "." + HelperManager.BindPengambilanObjekDariSource(cmbDisplayPath.SelectedItem, cmbDisplayPath.DisplayMemberPath);
                }
            }

        }

        private void TbIndexOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TbIndexGrid.Text = TbIndex.Text.Replace("_", "0");
            TbIndex.Text = TbIndex.Text.Replace("_", "0");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnLoaded();
        }
        public XElement Element { get; set; }

        public Type RootData { get; set; }
        public PropertyInfo CurrentProperty { get; set; }

        private void OnLoaded()
        {
            try
            {


                #region Fill Form


                cmbTypeForm.ItemsSource = Enum.GetNames(typeof(FormType));
                cmbTypeModel.ItemsSource = HelperManager.GetListTableItem();
                //cmbTypeModelGrid.ItemsSource = HelperManager.GetListTableItem();
                cmbTypeModel.SelectionChanged += CmbTypeModelOnSelectionChanged;
                cmbTypeForm.SelectedItem = "TextBox";
                cmbTypeGrid.ItemsSource = new[] { "TextBox", "CheckBox" };
                cmbTypeGrid.SelectedItem = "TextBox";
                cmbReadOnlyForm.ItemsSource = new[] { "ReadOnly" };
                var fieldAttribute = CurrentProperty.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault();
                if (fieldAttribute != null)
                {
                    cbPrimary.IsChecked = fieldAttribute.IsPrimary;
                    cbRequired.IsChecked = fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull;
                    if (fieldAttribute.IsPrimary)
                        cmbReadOnlyForm.SelectedItem = "ReadOnly";
                }

                if (CurrentProperty.Name.Equals("ReportDisplay"))
                {
                    cbUse.IsChecked = true;
                    cbPrimary.IsChecked = false;
                    cbRequired.IsChecked = true;
                    cmbDisplayPath.SelectedItem = null;
                    tbDsiplayPathGrid.Text = "ReportDisplay";
                    tbTitle.Text = "Report Display";
                    tbTitleGrid.Text = "Report Display";
                    cmbValuePath.SelectedItem = null;
                    TbIndex.Text = "03";
                    TbIndexGrid.Text = "03";
                    cmbReadOnlyForm.SelectedItem = "";
                    cmbTypeForm.SelectedItem = "TextBox";
                    cmbTypeGrid.SelectedItem = "TextBox";
                    cmbTypeModel.SelectedItem = null;
                    return;
                }
                if (CurrentProperty.Name.Equals("KodeExternal"))
                {
                    cbUse.IsChecked = true;
                    cbPrimary.IsChecked = false;
                    cbRequired.IsChecked = false;
                    cmbDisplayPath.SelectedItem = null;
                    tbDsiplayPathGrid.Text = "KodeExternal";
                    tbTitle.Text = "Kode External";
                    tbTitleGrid.Text = "Kode External";
                    cmbValuePath.SelectedItem = null;
                    TbIndex.Text = "04";
                    TbIndexGrid.Text = "04";
                    cmbReadOnlyForm.SelectedItem = "";
                    cmbTypeForm.SelectedItem = "TextBox";
                    cmbTypeGrid.SelectedItem = "TextBox";
                    cmbTypeModel.SelectedItem = null;
                    return;
                }
                if (CurrentProperty.Name.Equals("NamaExternal"))
                {
                    cbUse.IsChecked = true;
                    cbPrimary.IsChecked = false;
                    cbRequired.IsChecked = false;
                    cmbDisplayPath.SelectedItem = null;
                    tbDsiplayPathGrid.Text = "NamaExternal";
                    tbTitle.Text = "Nama External";
                    tbTitleGrid.Text = "Nama External";
                    cmbValuePath.SelectedItem = null;
                    TbIndex.Text = "05";
                    TbIndexGrid.Text = "05";
                    cmbReadOnlyForm.SelectedItem = "";
                    cmbTypeForm.SelectedItem = "TextBox";
                    cmbTypeGrid.SelectedItem = "TextBox";
                    cmbTypeModel.SelectedItem = null;
                    return;
                }

                var firstOrDefault = CurrentProperty.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault();
                if (firstOrDefault != null && firstOrDefault.IsPrimary)
                {
                    cbPrimary.IsChecked = firstOrDefault.IsPrimary;
                    cbRequired.IsChecked = true;
                }
                cmbDisplayPath.SelectedItem = null;
                tbDsiplayPathGrid.Text = CurrentProperty.Name;
                tbTitle.Text = CurrentProperty.Name.Replace("Kd", "Kode").ConvertToStatementText();
                tbTitleGrid.Text = CurrentProperty.Name.Replace("Kd", "Kode").ConvertToStatementText();
                cmbValuePath.SelectedItem = null;
                TbIndex.Text = "00";
                TbIndexGrid.Text = "00";
                cmbReadOnlyForm.SelectedItem = "";
                var referenceAtribute = CurrentProperty.GetCustomAttributes(true).OfType<ReferenceAttribute>().FirstOrDefault();
                if (referenceAtribute != null)
                {
                    var data = Activator.CreateInstance(CurrentProperty.DeclaringType);
                    foreach (var referenceItem in data.GetType().GetProperties().Where(n => n.PropertyType == referenceAtribute.TypeModel))
                    {
                        foreach (var dict in HelperManager.GetListTableItem())
                        {
                            if (dict.Key.Equals(referenceItem.PropertyType.FullName))
                            {
                                cmbTypeModel.SelectedItem = dict;
                                cmbTypeModel.Text = referenceItem.PropertyType.FullName;
                            }
                        }

                    }
                    //if (referenceAtribute.TypeModel)
                    cmbTypeForm.SelectedItem = "ListItem";
                }
                else
                {
                    cmbTypeForm.SelectedItem = "TextBox";
                    cmbTypeModel.SelectedItem = null;
                }


                cmbTypeGrid.SelectedItem = "TextBox";

                var model = Activator.CreateInstance(RootData) as TableItem;
                if (Element == null) return;
                XElement tableElement =
                    Element.Descendants("Table").FirstOrDefault(n => n.Attribute("Key") != null && n.Attribute("Key").Value.Equals(RootData.FullName));
                if (tableElement == null) return;
                XElement item = tableElement.Descendants("Property").FirstOrDefault(n =>
                                                                                        {
                                                                                            XAttribute xAttribute =
                                                                                                n.Attribute("Key");
                                                                                            return xAttribute != null &&
                                                                                                   xAttribute.Value.
                                                                                                       Equals(
                                                                                                           CurrentProperty
                                                                                                               .Name);
                                                                                        });
                if (item != null)
                {
                    if (item.Parent != null)
                        if (item.Parent.Name.LocalName.Equals("Design"))
                        {
                            XElement formInputElement = item.Element("FormInput");
                            if (formInputElement != null)
                            {
                                foreach (XElement xElement in formInputElement.Elements())
                                {
                                    foreach (
                                        IValueElement findVisualChild in
                                            Manager.FindVisualChildren<FrameworkElement>(stackPanelForm).OfType
                                                <IValueElement>())
                                    {
                                        var frameworkElement = findVisualChild as FrameworkElement;
                                        if (frameworkElement != null && frameworkElement.Tag != null &&
                                            frameworkElement.Tag.ToString().Equals("FormInput-" +
                                                                                   xElement.Name.LocalName))
                                        {
                                            if (!string.IsNullOrEmpty(xElement.Value.Trim()))
                                                findVisualChild.Value = xElement.Value.Trim();
                                        }
                                    }
                                }
                            }

                            formInputElement = item.Element("Grid");
                            if (formInputElement != null)
                            {
                                foreach (XElement xElement in formInputElement.Elements())
                                {
                                    foreach (
                                        IValueElement findVisualChild in
                                            Manager.FindVisualChildren<FrameworkElement>(stackPanelGrid).OfType
                                                <IValueElement>())
                                    {
                                        var frameworkElement = findVisualChild as FrameworkElement;
                                        if (frameworkElement != null && frameworkElement.Tag != null &&
                                            frameworkElement.Tag.ToString().Equals("Grid-" +
                                                                                   xElement.Name.LocalName))
                                        {
                                            if (!string.IsNullOrEmpty(xElement.Value.Trim()))
                                                findVisualChild.Value = xElement.Value.Trim();
                                        }
                                    }
                                }
                            }
                        }
                    cbUse.IsChecked = true;
                }
                else
                {
                    cbUse.IsChecked = false;
                }

                #endregion
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Load Detail Data");
            }
        }

        private void CmbTypeModelOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var data = cmbTypeModel.SelectedItem as KeyValuePair<string, Type>?;
            if (!data.HasValue) return;

            cmbDisplayPath.ItemsSource =
                Activator.CreateInstance(data.Value.Value).GetType().GetProperties();
            cmbValuePath.ItemsSource =
                Activator.CreateInstance(data.Value.Value).GetType().GetProperties();
        }

        public IEnumerable<IValueElement> GetDataForm()
        {
            if (cbUse.IsChecked.Value == false) yield break;
            foreach (
                IValueElement findVisualChild in
                    Manager.FindVisualChildren<FrameworkElement>(stackPanelForm).OfType<IValueElement>())
            {
                var frameworkElement = findVisualChild as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Tag != null)
                {
                    yield return findVisualChild;
                }
            }
        }

        public IEnumerable<IValueElement> GetDataGrid()
        {
            if (cbUse.IsChecked.Value == false) yield break;
            foreach (
                IValueElement findVisualChild in
                    Manager.FindVisualChildren<FrameworkElement>(stackPanelGrid).OfType<IValueElement>())
            {
                var frameworkElement = findVisualChild as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Tag != null)
                {
                    yield return findVisualChild;
                }
            }
        }
        private void CmbTypeFormSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTypeForm.SelectedItem == null) return;
            if (cmbTypeForm.SelectedItem.ToString().Equals("ListItem"))
            {
                gridTypeModel.Visibility = Visibility.Visible;
                gridDisplayPath.Visibility = Visibility.Visible;
                gridValuePath.Visibility = Visibility.Visible;
            }
            else
            {
                gridTypeModel.Visibility = Visibility.Collapsed;
                gridDisplayPath.Visibility = Visibility.Collapsed;
                gridValuePath.Visibility = Visibility.Collapsed;
            }
        }

        public void BindingData(PropertyInfo propertyInfo)
        {
            tbFormTitle.Text = "Configuration Form " + propertyInfo.Name;
            tbGridTitle.Text = "Configuration Grid " + propertyInfo.Name;
            CurrentProperty = propertyInfo;
        }
    }
}