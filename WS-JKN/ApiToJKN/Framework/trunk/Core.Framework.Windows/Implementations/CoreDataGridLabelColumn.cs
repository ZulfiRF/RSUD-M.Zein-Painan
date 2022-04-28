using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridLabelColumn : DataGridComboBoxColumn, IMultipleHeader
    {
        #region Methods

        private bool firstLoad = false;

        public void FocusControl()
        {
            var item = this.GetCellContent(DataGridOwner.SelectedItem);
            if (item != null)
                item.Focus();
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

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (!firstLoad)
            {
                DataGridOwner.LoadingRow += DataGridOwnerOnLoadingRow;
                firstLoad = true;
            }
            cell.GotFocus += (sender, args) => cell.Background = new SolidColorBrush(Colors.RoyalBlue);
            cell.LostFocus += (sender, args) => cell.Background = new SolidColorBrush(Colors.Transparent);
            var textBlock = new TextBlock();
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, new Binding(this.DisplayMemberPath)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            textBlock.Height = cell.Height;
            textBlock.Tag = dataItem;
            textBlock.Margin = new Thickness(5, 0, 5, 0);
            var changed = dataItem as INotifyPropertyChanged;
            if (changed != null)
                changed.PropertyChanged += OnPropertyChanged;
            return textBlock;
        }

        private void DataGridOwnerOnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var changed = e.Row.Item as INotifyPropertyChanged;
            if (changed != null)
                changed.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Manager.Timeout(Dispatcher, () =>
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
                    var item = this.GetCellContent(sender);
                    {
                        TempPropertyChange =
                            propertyValuePath;
                        var block = item as TextBlock;
                        if (block != null && !block.Text.Equals(propertyValuePath.ToString()))
                            block.Text = propertyValuePath.ToString();
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            Thread.Sleep(100);
                            TempPropertyChange = null;
                        });
                    }
                }
            });

        }
        #endregion

        public object TempPropertyChange;
    }
}