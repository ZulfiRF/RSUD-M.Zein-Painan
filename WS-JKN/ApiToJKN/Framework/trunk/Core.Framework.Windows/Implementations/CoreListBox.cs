using System.Collections;
using Core.Framework.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Extensions;
    using Core.Framework.Windows.Helper;
    using System.Collections.Generic;

    public class CoreListBox : ListBox, IValueElement, IValidateControl
    {

        public CoreListBox()
        {
            GotFocus += OnGotFocus;
            KeyDown += OnKeyDown;
            Loaded+=OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Manager.RegisterFormGrid(this);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == System.Windows.Input.Key.Delete)
            {
                if (Manager.Confirmation("Konfirmasi", "Apakah anda akan menghapus data ini?"))
                    OnControlDelete(new ItemEventArgs<object>(this.SelectedItems as IList));
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var container = this.ContainerFromElement((DependencyObject)sender);
            if (container != null)
            {
                this.SelectedItem = this.ItemContainerGenerator.ItemFromContainer(container);
            }
        }

        #region Fields

        private bool isError;

        #endregion

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;
        public event EventHandler<ItemEventArgs<object>> ControlDelete;

        public void OnControlDelete(ItemEventArgs<object> e)
        {
            var handler = ControlDelete;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Public Properties

        public string DomainNameSpaces { get; set; }

        public bool IsError
        {
            get
            {
                return this.isError;
            }
            set
            {
                this.isError = value;
                this.BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            }
        }

        public bool IsNull
        {
            get
            {
                var args = new HandleArgs();
                this.OnBeforeValidate(args);
                if (args.Handled)
                {
                    return true;
                }
                if (!this.IsRequired)
                {
                    return false;
                }
                return this.SelectedItem == null;
            }
        }

        public bool IsRequired { get; set; }

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key { get; set; }
        public bool SkipAutoFocus { get; set; }

        public object Value
        {
            get
            {
                if (this.SelectedItem == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(this.ValuePath))
                {
                    return this.SelectedItem;
                }
                PropertyInfo property = this.SelectedItem.GetType().GetProperty(this.ValuePath);
                if (property != null)
                {
                    return property.GetValue(this.SelectedItem, null);
                }
                return null;
            }
            set
            {
                this.SelectedItem = value;
            }
        }

        public string ValuePath { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            this.SelectedItem = null;
            this.IsError = false;
        }

        public void FocusControl()
        {

            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher,
                () =>
                {
                    this.SelectedIndex = 0;
                    this.Focus();
                    var item = this.GetContainerForItemOverride();
                    var frameworkElement = item as ListBoxItem;
                    if (frameworkElement != null)
                    {
                        ThreadPool.QueueUserWorkItem(
                            s => Manager.Timeout(
                                this.Dispatcher,
                                () =>
                                {
                                    Thread.Sleep(200);
                                    var control = s as ListBoxItem;
                                    if (control != null)
                                    {
                                        control.IsSelected = true;
                                        control.Focus();
                                    }
                                }), frameworkElement);
                    }

                }));
        }

        public void OnBeforeValidate(HandleArgs e = null)
        {
            EventHandler<HandleArgs> handler = this.BeforeValidate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return && !this.SkipAutoFocus)
            {
                var parent = Manager.FindVisualParent<CoreUserControl>(this);
                if (parent != null)
                {
                    bool valid = false;
                    foreach (FrameworkElement findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
                    {
                        if (valid && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus && (findVisualChild as IValueElement).CanFocus)
                        {
                            var parentTabItem = Manager.FindVisualParent<TabItem>(findVisualChild);
                            if (parentTabItem != null)
                            {
                                parentTabItem.IsSelected = true;
                            }
                            Manager.Timeout(Dispatcher, () =>
                                                            {
                                                                if (findVisualChild.IsEnabled)
                                                                {
                                                                    findVisualChild.Focus();
                                                                }
                                                            });
                            break;
                        }
                        if (findVisualChild.Equals(this))
                        {
                            valid = true;
                        }
                    }
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        #endregion
    }
}