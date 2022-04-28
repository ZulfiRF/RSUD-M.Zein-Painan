using System;

namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    public class CoreRadioButton : RadioButton, IValueElement
    {
        public CoreRadioButton()
        {
            Loaded+=OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Manager.RegisterFormGrid(this);
        }

        #region Fields

        private string key;

        #endregion

        #region Public Properties


        public bool CanFocus
        {
            get { return true; }
        }

        public string Key
        {
            get
            {
                if (this.Tag != null)
                {
                    return this.Tag.ToString();
                }
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public object Value
        {
            get
            {
                return this.IsChecked != null && this.IsChecked.Value;
            }
            set
            {
                if (value is string)
                {
                    this.IsChecked = value.Equals("true");
                }
                else
                {
                    this.IsChecked = value as bool?;
                }
            }
        }

        #endregion

        #region Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                this.IsChecked = true;
                var parent = Manager.FindVisualParent<CoreUserControl>(this);
                if (parent != null)
                {
                    bool valid = false;
                    foreach (FrameworkElement findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
                    {
                        if (valid && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus && !(findVisualChild is CoreRadioButton) && (findVisualChild as IValueElement).CanFocus)
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

        #endregion
    }
}