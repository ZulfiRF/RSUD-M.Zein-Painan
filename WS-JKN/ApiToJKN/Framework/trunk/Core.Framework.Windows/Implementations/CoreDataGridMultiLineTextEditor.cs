using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Framework.Helper;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridMultiLineTextEditor : DataGridComboBoxColumn, IMultipleHeader
    {
        public event KeyEventHandler TextBoxKeyDown;
        public void OnTextBoxKeyDown(MultiLineTextEditor control,KeyEventArgs e)
        {
            KeyEventHandler handler = TextBoxKeyDown;
            if (handler != null) handler(this, e);
        }

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

        CoreDictionary<object, MultiLineTextEditor> dictionary = new CoreDictionary<object, MultiLineTextEditor>();
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            MultiLineTextEditor combo;
            //if (!dictionary.TryGetValue(dataItem , out combo))
            {
                combo = new MultiLineTextEditor();
                combo.Loaded += ComboOnLoaded;
                combo.Height = cell.Height;
                combo.GotFocus += (sender, args) => this.DataGridOwner.CurrentColumn = this;
                combo.KeyDown += (sender, args) => OnTextBoxKeyDown(sender as MultiLineTextEditor, args);
                combo.LostFocus += delegate(object sender, RoutedEventArgs args)
                {
                    var comboLocal = sender as MultiLineTextEditor;
                    PropertyInfo propertyValuePath = dataItem.GetType().GetProperty(this.SelectedValuePath);
                    if (propertyValuePath != null)
                    {
                        if (comboLocal != null)
                        {
                            try
                            {
                                if (propertyValuePath.PropertyType.IsGenericType && propertyValuePath.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    propertyValuePath.SetValue(
                                        dataItem,
                                        Convert.ChangeType(combo.Text, propertyValuePath.PropertyType.GetGenericArguments()[0]),
                                        null);
                                }
                                else
                                {
                                    propertyValuePath.SetValue(
                                        dataItem,
                                        Convert.ChangeType(combo.Text, propertyValuePath.PropertyType),
                                        null);
                                }

                            }
                            catch (Exception)
                            {
                            }

                        }
                    }
                };
                //dictionary.Add(dataItem , combo);

            }
            if (dataItem is INotifyPropertyChanged)
            {
                (dataItem as INotifyPropertyChanged).PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                {
                    object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                        sender,
                        this.SelectedValuePath);
                    if (propertyValuePath != null)
                    {
                        combo.Text = propertyValuePath.ToString();
                    }
                };
            }
            if (dataItem != null)
            {
                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                    dataItem,
                    this.SelectedValuePath);
                if (propertyValuePath != null)
                {
                    combo.Text = propertyValuePath.ToString();
                }
            }
           
            return combo;
        }

        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var combo = sender as MultiLineTextEditor;
            if (combo != null) combo.BorderThickness = new Thickness(0);
        }
        
    }
}