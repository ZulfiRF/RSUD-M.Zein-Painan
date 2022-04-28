using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Helper;
using Core.Framework.Helper.Extention;
using Core.Framework.Model.Attr;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Implementations.InputGrid;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDataGridTextBlockTextColumn : DataGridComboBoxColumn, IMultipleHeader, IFocusControl
    {
        public enum StateInfo
        {
            Info, Edit, Commit
        }
        public event EventHandler<ItemEventArgs<StateInfo>> ChangeState;
        private List<DataGridCell> listCellLoad;
        public void Focus(object dataItem = null)
        {

            Manager.Timeout(Dispatcher, () =>
            {
                if (dataItem == null)
                {
                    if (listCellLoad != null)
                    {
                        var firstOrDefault = listCellLoad.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            while (!firstOrDefault.IsFocused)
                            {
                                firstOrDefault.Focus();
                            }
                        }
                    }
                }
                else
                if (listCellLoad != null)
                    foreach (var dataGridCell in listCellLoad.Where(n => n.IsLoaded))
                    {
                        if (dataGridCell.DataContext != null)
                        {
                            if (dataGridCell.DataContext.Equals(dataItem))
                            {
                                var local = dataGridCell;
                                var i = 0;
                                while (!local.IsFocused)
                                {
                                    local.Focus();
                                    i++;
                                    if (i > 100)
                                        break;
                                }
                                if (!this.DataGridOwner.SelectedItem.Equals(dataItem))
                                    this.DataGridOwner.SelectedItem = dataItem;
                                break;
                            }
                        }
                    }
            });

        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set
            { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CoreDataGridTextBlockTextColumn), new UIPropertyMetadata(true));

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(CoreDataGridTextBlockTextColumn), new UIPropertyMetadata(32767));

        private CoreDictionary<object, DataGridCell> dictionaryControlCells = new CoreDictionary<object, DataGridCell>();

        public event KeyEventHandler TextBoxKeyDown;
        public event KeyEventHandler TextBoxKeyUp;

        public bool ChangeFromLocal { get; set; }

        //public bool HasChangeCombo { get; set; }
        private bool hasChangeCombo;

        public bool HasChangeCombo
        {
            get { return hasChangeCombo; }
            set
            {
                hasChangeCombo = value;
                if (!value)
                {
                    Console.WriteLine();
                }
            }
        }


        public void FocusControl()
        {
            foreach (var dataGridCell in dictionaryControlCells.Where(n => n.Value.IsLoaded))
            {
                dataGridCell.Value.Focus();
                break;
            }
        }

        public void SetHeader()
        {
            if (Header is string) if (Header.ToString().Contains("^"))
                {
                    var arr = Header.ToString().Split(new[] { '^' });
                    var stackPanel = new StackPanel();
                    foreach (var s in arr)
                    {
                        var text = new LabelInfo();
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
        #region Methods
        //private DataGridCell currentCell;
        CoreDictionary<object, CoreTextBox> dictionary = new CoreDictionary<object, CoreTextBox>();
        public event EventHandler<ItemEventArgs<object>> Initialize;

        private List<CoreTextBox> listTextBox = new List<CoreTextBox>();
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            this.DataGridOwner.KeyDown -= DataGridOwnerOnKeyDown;
            base.CancelCellEdit(editingElement, uneditedValue);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (listCellLoad == null)
                listCellLoad = new List<DataGridCell>();
            if (!listCellLoad.Any(n => n.Equals(cell)))
                listCellLoad.Add(cell);
            cell.GotFocus += (sender, args) =>
            {
                cell.Background = new SolidColorBrush(Colors.RoyalBlue);
                CanPressReturn = false;
            };
            cell.LostFocus += (sender, args) =>
            {
                cell.Background = new SolidColorBrush(Colors.Transparent);
            };
            cell.KeyDown += delegate (object sender, KeyEventArgs args)
            {
                if (args.Key == Key.Return)
                {
                    cell.IsEditing = true;
                    args.Handled = true;
                }
            };
            var label = new LabelInfo();
            if (dataItem != null)
            {
                var property = dataItem.GetType().GetProperty(SelectedValuePath);
                if (property != null)
                {
                    var reference = property.GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute);
                    if (reference != null)
                    {
                        var referenceAttribute = reference as ReferenceAttribute;
                        if (referenceAttribute != null)
                        {
                            property = dataItem.GetType().GetProperty(referenceAttribute.Property);
                            var result = HelperManager.BindPengambilanObjekDariSource(property.GetValue(dataItem, null), SelectedValuePath);
                            label.Text = (result == null ? "" : result.ToString());
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(DisplayMemberPath))
                        {
                            var result = HelperManager.BindPengambilanObjekDariSource(dataItem, DisplayMemberPath);
                            label.Text = (result == null ? "" : result.ToString());
                        }
                        else
                        {
                            var result = HelperManager.BindPengambilanObjekDariSource(dataItem, SelectedValuePath);
                            label.Text = (result == null ? "" : result.ToString());
                        }
                    }


                }
                if (dataItem is INotifyPropertyChanged)
                {
                    (dataItem as INotifyPropertyChanged).PropertyChanged += delegate (object sender, PropertyChangedEventArgs args)
                    {
                        Manager.Timeout(Dispatcher, () =>
                                                        {
                                                            if (!this.SelectedValuePath.Equals(args.PropertyName)) return;
                                                            if (!string.IsNullOrEmpty(DisplayMemberPath))
                                                            {
                                                                var result = HelperManager.BindPengambilanObjekDariSource(dataItem, DisplayMemberPath);
                                                                label.Text = (result == null ? "" : result.ToString());
                                                            }
                                                            else
                                                            {
                                                                var result = HelperManager.BindPengambilanObjekDariSource(dataItem, SelectedValuePath);
                                                                label.Text = (result == null ? "" : result.ToString());
                                                            }
                                                        });
                    };
                }
            }

            Manager.Timeout(Dispatcher, () => OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Info)));
            return label;
        }

        //protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        //{
        //    if (listCellLoad == null)
        //        listCellLoad = new List<DataGridCell>();
        //    if (!listCellLoad.Any(n => n.Equals(cell)))
        //        listCellLoad.Add(cell);
        //    this.DataGridOwner.PreviewKeyDown += DataGridOwnerOnPreviewKeyDown;
        //    cell.GotFocus += (sender, args) =>
        //    {
        //        cell.Background = new SolidColorBrush(Colors.RoyalBlue);

        //        CanPressReturn = false;
        //    };
        //    cell.LostFocus += (sender, args) =>
        //    {
        //        cell.Background = new SolidColorBrush(Colors.Transparent);
        //        //this.DataGridOwner.PreviewKeyDown -= DataGridOwnerOnPreviewKeyDown;
        //    };
        //    DataGridCell obj;
        //    if (!dictionaryControlCells.TryGetValue(dataItem, out obj))
        //        dictionaryControlCells.Add(dataItem, cell);
        //    CoreTextBox combo;

        //    //if (!dictionary.TryGetValue(dataItem , out combo))
        //    {
        //        combo = new CoreTextBox();
        //        combo.IsEnabled = IsEnabled;
        //        combo.Tag = dataItem;
        //        combo.Loaded += ComboOnLoaded;
        //        combo.Height = cell.Height;
        //        combo.ParentCell = cell;
        //        if (dataItem.GetType().GetProperty(SelectedValuePath) != null)
        //            combo.FilterType = dataItem.GetType().GetProperty(SelectedValuePath).PropertyType;
        //        combo.GotFocus += ComboOnGotFocus;
        //        combo.KeyDown += (sender, args) => OnTextBoxKeyDown(sender as CoreTextBox, args);
        //        combo.KeyUp += (sender, args) => OnTextBoxKeyUp(sender as CoreTextBox, args);
        //        combo.LostFocus += (sender, args) =>
        //        {
        //            BindingValueFromTextBox(dataItem, sender, combo);
        //            var coreTextBox = sender as CoreTextBox;
        //            if (coreTextBox != null) LoadDefaultStateContent(coreTextBox.ParentCell);
        //        };
        //        combo.KeyDown += (sender, args) =>
        //        {
        //            if (args.Key == Key.Return)
        //            {
        //                OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit));
        //                HasChangeCombo = true;
        //                CanPressReturn = false;
        //                BindingValueFromTextBox(dataItem, sender, combo);
        //                var coreTextBox = sender as CoreTextBox;
        //                if (coreTextBox != null) LoadDefaultStateContent(coreTextBox.ParentCell);
        //            }
        //        };
        //        combo.TextChanged += delegate(object sender, TextChangedEventArgs args)
        //        {
        //            BindingValueFromTextBox(dataItem, sender, combo);
        //        };

        //    }
        //    if (dataItem is INotifyPropertyChanged)
        //    {
        //        (dataItem as INotifyPropertyChanged).PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
        //        {
        //            object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
        //                sender,
        //                this.SelectedValuePath);
        //            if (propertyValuePath != null)
        //            {
        //                combo.Text = propertyValuePath.ToString();
        //            }
        //        };
        //    }
        //    if (dataItem != null)
        //    {
        //        object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
        //            dataItem,
        //            this.SelectedValuePath);
        //        if (propertyValuePath != null)
        //        {
        //            combo.Text = propertyValuePath.ToString();
        //        }
        //    }

        //    var LabelInfo2 = new LabelInfo();
        //    if (dataItem != null)
        //    {
        //        var property = dataItem.GetType().GetProperty(SelectedValuePath);
        //        if (property != null)
        //        {
        //            var reference = property.GetCustomAttributes(true).FirstOrDefault(n => n is ReferenceAttribute);
        //            if (reference != null)
        //            {
        //                var referenceAttribute = reference as ReferenceAttribute;
        //                if (referenceAttribute != null)
        //                {
        //                    property = dataItem.GetType().GetProperty(referenceAttribute.Property);
        //                    var result = HelperManager.BindPengambilanObjekDariSource(property.GetValue(dataItem, null), SelectedValuePath);
        //                    LabelInfo2.Text = (result == null ? "" : result.ToString());
        //                }

        //            }
        //            else
        //            {
        //                var result = HelperManager.BindPengambilanObjekDariSource(dataItem, SelectedValuePath);
        //                LabelInfo2.Text = (result == null ? "" : result.ToString());
        //            }


        //        }
        //    }
        //    LabelInfo2.Tag = combo;
        //    cell.KeyDown += LabelInfo2OnKeyUp;
        //    cell.KeyDown+= delegate(object sender, KeyEventArgs args)
        //    {
        //        cell.IsEditing = true;
        //    };
        //    combo.Tag = LabelInfo2;
        //    listTextBox.Add(combo);
        //    EventHandler<ItemEventArgs<object>> handler = Initialize;
        //    if (handler != null) handler(combo, new ItemEventArgs<object>(dataItem));
        //    return LabelInfo2;
        //}

        //private void DataGridOwnerOnPreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return)
        //    {
        //        //if (!canPressReturn)
        //        //    e.Handled = true;
        //        //else 

        //        if (e.OriginalSource is CoreTextBox)
        //        {

        //            var textBox = e.OriginalSource as CoreTextBox;
        //            if (!listTextBox.Any(n => n.Equals(textBox))) return;
        //            CanPressReturn = false;
        //            BindingValueFromTextBox(textBox.Tag, textBox, textBox);
        //            var coreTextBox = textBox as CoreTextBox;
        //            LoadDefaultStateContent(coreTextBox.ParentCell);
        //            e.Handled = true;
        //            OnTextBoxKeyDown(textBox, e);
        //            OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit));
        //        }
        //        else
        //        {

        //        }

        //    }
        //}

        private void BindingValueFromTextBox(object dataItem, object sender, CoreTextBox combo)
        {
            var comboLocal = sender as CoreTextBox;
            if (dataItem == null) return;
            PropertyInfo propertyValuePath = null;
            if (!string.IsNullOrEmpty(DisplayMemberPath))
                propertyValuePath = dataItem.GetType().GetProperty(DisplayMemberPath);
            else
                propertyValuePath = dataItem.GetType().GetProperty(this.SelectedValuePath);
            if (propertyValuePath != null)
            {
                if (comboLocal != null)
                {
                    try
                    {
                        if (propertyValuePath.PropertyType.IsGenericType &&
                            propertyValuePath.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            propertyValuePath.SetValue(
                                dataItem,
                                Convert.ChangeType(combo.Text, propertyValuePath.PropertyType.GetGenericArguments()[0]),
                                null);
                        }
                        else
                        {
                            if (propertyValuePath.PropertyType == typeof(double) || propertyValuePath.PropertyType == typeof(decimal))
                            {
                                var result = combo.Text.ToDouble();
                                propertyValuePath.SetValue(
                                    dataItem,
                                    Convert.ChangeType(result, propertyValuePath.PropertyType, new CultureInfo("id-ID")),
                                    null);
                            }
                            else if (propertyValuePath.PropertyType == typeof(float))
                            {
                                var result = combo.Text.ToFloat();
                                propertyValuePath.SetValue(
                                    dataItem,
                                    Convert.ChangeType(result, propertyValuePath.PropertyType, new CultureInfo("id-ID")),
                                    null);
                            }
                            else
                                propertyValuePath.SetValue(
                                    dataItem,
                                    Convert.ChangeType(combo.Text, propertyValuePath.PropertyType, new CultureInfo("id-ID")),
                                    null);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }


        private void ComboOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var combo = sender as CoreTextBox;
            if (combo != null) combo.BorderThickness = new Thickness(0);
        }


        private void DataGridOwnerOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left ||
                e.Key == Key.Up ||
                e.Key == Key.Down ||
                e.Key == Key.Right
                )
                e.Handled = true;
        }



        //private void LoadDefaultStateContent(DataGridCell cell)
        //{
        //    if (cell == null) return;

        //    var textBox = cell.Content as CoreTextBox;
        //    ThreadPool.QueueUserWorkItem(delegate(object state)
        //    {
        //        Manager.Timeout(Dispatcher, () =>
        //        {
        //            while (textBox != null && textBox.IsLoaded)
        //            {
        //                Thread.Sleep(2);
        //                break;
        //            }
        //            HasChangeCombo = false;
        //        });
        //    });

        //    if (textBox != null)
        //    {
        //        var LabelInfo = textBox.Tag as LabelInfo;
        //        if (LabelInfo != null) LabelInfo.Text = textBox.Text;
        //        cell.Content = textBox.Tag as LabelInfo;
        //        OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Info));
        //    }
        //    ThreadPool.QueueUserWorkItem(delegate(object state)
        //    {
        //        Manager.Timeout(Dispatcher, () =>
        //        {
        //            while (!((DataGridCell)state).IsFocused)
        //            {
        //                Thread.Sleep(2);
        //                ((DataGridCell)state).Focus();
        //            }

        //        });
        //    }, cell);
        //    this.DataGridOwner.KeyUp -= DataGridOwnerOnKeyDown;

        //}

        //private void LabelInfo2OnKeyUp(object sender, KeyEventArgs e)
        //{

        //    if (e.Key == Key.Return && !HasChangeCombo)
        //    {
        //        var cell = sender as DataGridCell;
        //        if (cell != null)
        //        {
        //            currentCell = cell;
        //            if (cell.Content != null)
        //            {
        //                if (cell.Content is LabelInfo)
        //                {
        //                    var coreComboBox = (cell.Content as LabelInfo).Tag as CoreTextBox;
        //                    if (coreComboBox != null)
        //                    {
        //                        //cell.KeyUp -= LabelInfo2OnKeyUp;
        //                        var LabelInfo = (LabelInfo)cell.Content;
        //                        if (LabelInfo != null)
        //                        {
        //                            ((CoreTextBox)LabelInfo.Tag).SelectAll();

        //                        }
        //                        cell.Content = (cell.Content as LabelInfo).Tag as CoreTextBox;
        //                        OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Edit));
        //                        coreComboBox.LostFocus += CoreTextBoxOnLostFocus;
        //                        this.DataGridOwner.KeyUp += DataGridOwnerOnKeyDown;
        //                        this.DataGridOwner.KeyUp += DataGridOwnerOnKeyDown2;
        //                        HasChangeCombo = false;
        //                        Manager.Timeout(Dispatcher, () => coreComboBox.Focus());
        //                    }

        //                }
        //                else if (cell.Content is CoreTextBox)
        //                {
        //                    LoadDefaultStateContent(cell);
        //                }
        //            }
        //        }
        //    }
        //    //else if (e.Key == Key.Return && HasChangeCombo)
        //    //    ThreadPool.QueueUserWorkItem(delegate(object state)
        //    //    {
        //    //        Thread.Sleep(100);
        //    //        HasChangeCombo = false;
        //    //    });
        //}
        #endregion

        public bool CanPressReturn { get; set; }

        protected virtual void OnChangeState(ItemEventArgs<StateInfo> e)
        {
            var handler = ChangeState;
            if (handler != null) handler(this, e);
        }






        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var combo = new CoreTextBox();
            combo.MaxLength = MaxLength;
            combo.MaxLines = MaxLength;
            combo.IsEnabled = IsEnabled;
            combo.Tag = dataItem;
            combo.Loaded += ComboOnLoaded;
            combo.Height = cell.Height;
            combo.ParentCell = cell;
            if (dataItem.GetType().GetProperty(SelectedValuePath) != null)
                combo.FilterType = dataItem.GetType().GetProperty(SelectedValuePath).PropertyType;
            combo.KeyDown += (sender, args) => OnTextBoxKeyDown(sender as CoreTextBox, args);
            combo.KeyUp += (sender, args) => OnTextBoxKeyUp(sender as CoreTextBox, args);
            combo.LostFocus += (sender, args) =>
            {
                BindingValueFromTextBox(dataItem, sender, combo);
                var coreTextBox = sender as CoreTextBox;
                cell.IsEditing = false;
            };
            combo.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Return)
                {
                    BindingValueFromTextBox(dataItem, sender, combo);
                    args.Handled = true;
                    cell.IsEditing = false;
                    OnChangeState(new ItemEventArgs<StateInfo>(StateInfo.Commit));
                }
            };
            //combo.TextChanged += delegate(object sender, TextChangedEventArgs args)
            //{
            //    BindingValueFromTextBox(dataItem, sender, combo);
            //};

            if (dataItem is INotifyPropertyChanged)
            {
                (dataItem as INotifyPropertyChanged).PropertyChanged += delegate (object sender, PropertyChangedEventArgs args)
                {
                    Manager.Timeout(Dispatcher, () =>
                                                    {
                                                        object propertyValuePath = HelperManager.BindPengambilanObjekDariSource(
                                                            sender,
                                                            this.SelectedValuePath);
                                                        if (propertyValuePath != null)
                                                        {
                                                            combo.Text = propertyValuePath.ToString();
                                                        }
                                                    });
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
            combo.SelectAll();

            Manager.Timeout(Dispatcher, () =>
            {
                combo.FocusControl();
            });
            EventHandler<ItemEventArgs<object>> handler = Initialize;
            if (handler != null) handler(combo, new ItemEventArgs<object>(dataItem));
            this.DataGridOwner.KeyDown += DataGridOwnerOnKeyDown;
            this.DataGridOwner.KeyDown += HandleReturn;
            return combo;
            //return base.GenerateEditingElement(cell, dataItem);
        }

        private void HandleReturn(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                e.Handled = true;
        }


        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            var result = base.CommitCellEdit(editingElement);
            return result;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        protected override void OnSelectedValueBindingChanged(BindingBase oldBinding, BindingBase newBinding)
        {
            base.OnSelectedValueBindingChanged(oldBinding, newBinding);
        }
    }
}