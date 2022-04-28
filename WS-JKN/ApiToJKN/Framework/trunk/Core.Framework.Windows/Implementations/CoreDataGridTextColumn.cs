using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Helper;

    public class CoreDataGridTextColumn : DataGridComboBoxColumn, IMultipleHeader
    {
        public event KeyEventHandler TextBoxKeyDown;
        public event KeyEventHandler TextBoxKeyUp;

        private bool firstLoad = false;
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
        protected virtual void OnTextBoxKeyDown(CoreTextBox control, KeyEventArgs e)
        {
            KeyEventHandler handler = this.TextBoxKeyDown;
            if (handler != null)
            {
                handler(control, e);
            }
        }

        protected virtual void OnTextBoxKeyUp(CoreTextBox control, KeyEventArgs e)
        {
            KeyEventHandler handler = this.TextBoxKeyUp;
            if (handler != null)
            {
                handler(control, e);
            }
        }

        public void FocusControl()
        {
            var item = this.GetCellContent(DataGridOwner.SelectedItem);
            if (item != null)
                item.Focus();
        }

        #region Methods

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            CoreTextBox combo;
            if (!firstLoad)
            {
                DataGridOwner.LoadingRow += DataGridOwnerOnLoadingRow;
                firstLoad = true;
            }

            combo = new CoreTextBox();
            combo.Loaded += ComboOnLoaded;
            combo.TempValue = dataItem;
            combo.Height = cell.Height;
            combo.GotFocus += ComboOnGotFocus;
            combo.KeyDown += (sender, args) => OnTextBoxKeyDown(sender as CoreTextBox, args);
            combo.KeyUp += (sender, args) => OnTextBoxKeyUp(sender as CoreTextBox, args);
            BindingOperations.SetBinding(combo, TextBox.TextProperty, new Binding((string.IsNullOrEmpty(this.DisplayMemberPath) ? this.SelectedValuePath : this.DisplayMemberPath))
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            combo.LostFocus += delegate(object sender, RoutedEventArgs args)
            {
                var comboLocal = sender as CoreTextBox;
                if (comboLocal != null)
                {
                    var gridCell = comboLocal.Parent as DataGridCell;
                    PropertyInfo propertyValuePath = gridCell.DataContext.GetType().GetProperty(this.SelectedValuePath);
                    if (propertyValuePath != null)
                    {
                        if (comboLocal != null)
                        {
                            try
                            {
                                if (propertyValuePath.PropertyType.IsGenericType && propertyValuePath.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    if (gridCell != null)
                                        propertyValuePath.SetValue(
                                            gridCell.DataContext,
                                            Convert.ChangeType(comboLocal.Text, propertyValuePath.PropertyType.GetGenericArguments()[0]),
                                            null);
                                }
                                else
                                {
                                    if (gridCell != null)
                                        propertyValuePath.SetValue(
                                            gridCell.DataContext,
                                            Convert.ChangeType(comboLocal.Text, propertyValuePath.PropertyType),
                                            null);
                                }

                            }
                            catch (Exception exception)
                            {
                                Log.Error(exception);

                            }

                        }
                    }
                }
            };
            //dictionary.Add(dataItem , combo);

            if (dataItem is INotifyPropertyChanged)
            {
                (dataItem as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            }


            return combo;
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
                var block = item as CoreTextBox;
                if (block != null && !block.Text.Equals(propertyValuePath.ToString()))
                    block.Text = propertyValuePath.ToString();
            }
        }

        private void DataGridOwnerOnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is INotifyPropertyChanged)
                (e.Row.Item as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
        }




        private void ComboOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var coreTextBox = sender as CoreTextBox;
            if (coreTextBox != null)
            {
                var gridCell = coreTextBox.Parent as DataGridCell;
                if (gridCell != null)
                    DataGridOwner.SelectedItem = gridCell.DataContext;
            }
        }

        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var combo = sender as CoreTextBox;
            if (combo != null) combo.BorderThickness = new Thickness(0);
        }
        public object TempPropertyChange;
        #endregion
    }

}