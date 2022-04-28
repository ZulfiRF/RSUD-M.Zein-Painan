// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Core.Framework.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using Core.Framework.Windows.Converters;

    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : ContentControl
    {
        #region Constants

        private const string CommonStates = "CommonStates";

        private const string DisabledState = "Disabled";

        private const string NormalState = "Normal";

        private const string SwitchPart = "Switch";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(ToggleSwitch),
            new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(ToggleSwitch),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked",
            typeof(bool?),
            typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsCheckedChanged));

        public static readonly DependencyProperty OffProperty = DependencyProperty.Register(
            "OffLabel",
            typeof(string),
            typeof(ToggleSwitch),
            new PropertyMetadata("Off"));

        public static readonly DependencyProperty OnProperty = DependencyProperty.Register(
            "OnLabel",
            typeof(string),
            typeof(ToggleSwitch),
            new PropertyMetadata("On"));

        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);

        #endregion

        #region Fields

        private ToggleButton _toggleButton;

        private bool _wasContentSet;

        #endregion

        #region Constructors and Destructors

        public ToggleSwitch()
        {
            this.DefaultStyleKey = typeof(ToggleSwitch);

            this.PreviewKeyUp += this.ToggleSwitch_PreviewKeyUp;
        }

        #endregion

        #region Public Events

        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Click;
        public event EventHandler<RoutedEventArgs> Indeterminate;
        public event EventHandler IsCheckedChanged;
        public event EventHandler<RoutedEventArgs> Unchecked;

        #endregion

        #region Public Properties

        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HeaderTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get
            {
                return (bool?)this.GetValue(IsCheckedProperty);
            }
            set
            {
                this.SetValue(IsCheckedProperty, value);
            }
        }

        public string OffLabel
        {
            get
            {
                return (string)this.GetValue(OffProperty);
            }
            set
            {
                this.SetValue(OffProperty, value);
            }
        }

        public string OnLabel
        {
            get
            {
                return (string)this.GetValue(OnProperty);
            }
            set
            {
                this.SetValue(OnProperty, value);
            }
        }

        public Brush SwitchForeground
        {
            get
            {
                return (Brush)this.GetValue(SwitchForegroundProperty);
            }
            set
            {
                this.SetValue(SwitchForegroundProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!this._wasContentSet && this.GetBindingExpression(ContentProperty) == null)
            {
                this.SetDefaultContent();
            }

            if (this._toggleButton != null)
            {
                this._toggleButton.Checked -= this.CheckedHandler;
                this._toggleButton.Unchecked -= this.UncheckedHandler;
                this._toggleButton.Indeterminate -= this.IndeterminateHandler;
                this._toggleButton.Click -= this.ClickHandler;
            }
            this._toggleButton = this.GetTemplateChild(SwitchPart) as ToggleButton;
            if (this._toggleButton != null)
            {
                this._toggleButton.Checked += this.CheckedHandler;
                this._toggleButton.Unchecked += this.UncheckedHandler;
                this._toggleButton.Indeterminate += this.IndeterminateHandler;
                this._toggleButton.Click += this.ClickHandler;
                this._toggleButton.IsChecked = this.IsChecked;
            }
            this.ChangeVisualState(false);
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ToggleSwitch IsChecked={0}, Content={1}}}",
                this.IsChecked,
                this.Content);
        }

        #endregion

        #region Methods

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this._wasContentSet = true;
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                var oldValue = (bool?)e.OldValue;
                var newValue = (bool?)e.NewValue;

                toggleSwitch._toggleButton.IsChecked = newValue;

                if (oldValue != newValue && toggleSwitch.IsCheckedChanged != null)
                {
                    toggleSwitch.IsCheckedChanged(toggleSwitch, EventArgs.Empty);
                }
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? NormalState : DisabledState, useTransitions);
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            this.IsChecked = true;
            SafeRaise.Raise(this.Checked, this, e);
        }

        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(this.Click, this, e);
        }

        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            this.IsChecked = null;
            SafeRaise.Raise(this.Indeterminate, this, e);
        }

        private void SetDefaultContent()
        {
            var binding = new Binding("IsChecked")
                          {
                              Source = this,
                              Converter = new OffOnConverter(),
                              ConverterParameter = this
                          };
            SetBinding(ContentProperty, binding);
        }

        private void ToggleSwitch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && e.OriginalSource == sender)
            {
                this.IsChecked = !this.IsChecked;
            }
        }

        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            this.IsChecked = false;
            SafeRaise.Raise(this.Unchecked, this, e);
        }

        #endregion
    }
}