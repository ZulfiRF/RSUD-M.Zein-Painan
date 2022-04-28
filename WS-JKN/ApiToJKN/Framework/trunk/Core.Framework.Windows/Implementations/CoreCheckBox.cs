using System;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Contracts;

    public class CoreCheckBox : CheckBox, IValueElement
    {


        public CoreCheckBox()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Manager.RegisterFormGrid(this);
        }

        // Using a DependencyProperty as the backing store for UseTriggerSpace.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty UseTriggerSpaceProperty =
            DependencyProperty.Register(
                "UseTriggerSpace",
                typeof(bool),
                typeof(CoreCheckBox),
                new UIPropertyMetadata(false));

        #endregion

        #region Fields

        private string key;

        #endregion

        #region Public Properties

        public bool IsRequired { get; set; }

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

        public bool UseTriggerSpace
        {
            get
            {
                return (bool)this.GetValue(UseTriggerSpaceProperty);
            }
            set
            {
                this.SetValue(UseTriggerSpaceProperty, value);
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
                    if (value == null)
                    {
                        this.IsChecked = false;
                    }
                    else
                    {
                        if (value is int)
                            value = (int)value == 1;
                        else if (value is byte)
                            value = (byte)value == 1;
                        else if (value is Int16)
                            value = (Int16)value == 1;
                        this.IsChecked = value as bool?;
                    }
                }
            }
        }

        #endregion

        #region Methods

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.UseTriggerSpace)
            {
                if (e.Key == System.Windows.Input.Key.Space)
                {
                    this.IsChecked = !this.IsChecked;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Return)
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
            base.OnPreviewKeyDown(e);
        }

        #endregion
    }
}