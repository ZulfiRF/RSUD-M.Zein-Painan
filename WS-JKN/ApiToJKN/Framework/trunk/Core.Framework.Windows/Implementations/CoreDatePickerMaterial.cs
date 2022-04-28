using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Date;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using Core.Framework.Windows.Behaviours;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;


    public class CoreDatePickerMaterial : DatePicker, IValidateControl, IValueElement
    {
        public override void OnApplyTemplate()
        {
            var button = GetTemplateChild("PART_HeaderButton");
            if (button != null)
            {
                Console.WriteLine(button);
            }
            base.OnApplyTemplate();
        }

        public event EventHandler<HandleArgs> BeforeValidate;

        public bool IsRequired { get; set; }

        public bool IsNull
        {
            get { return !SelectedDate.HasValue; }
        }
        bool isError;
        public bool IsError
        {
            get { return isError; }
            set
            {
                isError = value;
                this.BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
            }
        }

        public CoreDatePickerMaterial()
        {
            if (Style == null)
            {
                if (Application.Current != null)
                {
                    Style = Application.Current.Resources["MaterialDesignDatePicker"] as Style;
                }
            }
            Loaded+=OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Foreground = new SolidColorBrush(Colors.Black);
            Manager.RegisterFormGrid(this);
        }

        public void ClearValueControl()
        {
            SelectedDate = null;
        }

        public void FocusControl()
        {
            Focus();
        }

        public bool SkipAutoFocus { get; set; }

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key { get; set; }

        public object Value
        {
            get { return this.SelectedDate; }
            set { SelectedDate = value as DateTime?; }
        }

        protected virtual void OnBeforeValidate(HandleArgs e)
        {
            var handler = BeforeValidate;
            if (handler != null) handler(this, e);
        }
    }
}