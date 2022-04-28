using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Helper;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{

    public class CoreDataGridDateTimeColumn : DataGridComboBoxColumn, IMultipleHeader
    {
        #region Methods
        CoreDictionary<object, CoreDatePicker> dictionary = new CoreDictionary<object, CoreDatePicker>();
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            CoreDatePicker combo;

            //if (!dictionary.TryGetValue(dataItem, out combo))
            {
                combo = new CoreDatePicker();
                combo.HasChildGrid = true;
                combo.Height = cell.Height;
                combo.GotFocus += (sender, args) => this.DataGridOwner.CurrentColumn = this;
                combo.SelectedDate = DateTime.Now;
                combo.SelectedDateChanged += delegate(object sender, SelectionChangedEventArgs args)
                {
                    var comboLocal = sender as CoreDatePicker;
                    PropertyInfo propertyValuePath = dataItem.GetType().GetProperty(this.SelectedValuePath);
                    if (propertyValuePath != null)
                    {
                        if (comboLocal != null)
                        {
                            propertyValuePath.SetValue(
                                dataItem,
                                Convert.ChangeType(combo.SelectedDate, propertyValuePath.PropertyType),
                                null);
                        }
                    }
                };
                //dictionary.Add(dataItem, combo);
            }

            if (dataItem != null)
            {
                object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                    dataItem,
                    this.SelectedValuePath);
                if (propertyValuePath != null)
                {
                    combo.SelectedDate = propertyValuePath as DateTime?;
                }
            }

            return combo;
        }

        #endregion
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
    }
}