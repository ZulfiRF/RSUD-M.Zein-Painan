using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Date;
using Core.Framework.Windows.Behaviours;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Windows.Implementations
{
    public class CoreDatePicker : DatePicker, IValidateControl, IValueElement
    {
        public CoreDatePicker()
        {
            Loaded+=OnLoaded;
            }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Manager.RegisterFormGrid(this);
        }

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

        #endregion

        #region Static Fields

        public static readonly DependencyProperty FormatNumberProperty = DependencyProperty.Register(
            "FormatNumber",
            typeof(string),
            typeof(CoreDatePicker),
            new UIPropertyMetadata("dd-MM-yyyy HH:mm:ss", FormatNumberCallBack));

        #endregion

        #region Fields

        //private TextBoxInputMaskBehavior behavior;

        private bool errorParse;

        private bool isError;

        #endregion

        #region Public Properties

        public bool AlwaysNow { get; set; }
        public DateTime? DefaultValue { get; set; }
        public bool AllowNull { get; set; }

        //public string FormatNumber { get; set; }

        public string FormatNumber
        {
            get { return (string)GetValue(FormatNumberProperty); }
            set { SetValue(FormatNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FormatNumber.  This enables animation, styling, binding, etc...

        public bool IsError
        {
            get { return isError; }
            set
            {
                isError = value;
                BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
                Background = value ? new SolidColorBrush(Colors.Tomato) : new SolidColorBrush(Colors.White);
                if (PartTextBoxDisplay != null)
                {
                    PartTextBoxDisplay.BorderBrush = BorderBrush;
                    PartTextBoxDisplay.Background = Background;
                }
            }
        }

        public bool IsNull
        {
            get
            {
                var args = new HandleArgs();
                OnBeforeValidate(args);
                if (args.Handled)
                {
                    return true;
                }
                if (!IsRequired)
                {
                    return false;
                }
                return string.IsNullOrEmpty(Text);
            }
        }

        public bool IsRequired { get; set; }

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key { get; set; }
        public Popup PartPopup { get; set; }
        public DatePickerTextBox PartTextBox { get; set; }
        public CoreTextBox PartTextBoxDisplay { get; set; }
        public bool SkipAutoFocus { get; set; }

        public object Value
        {
            get
            {
                if (PartTextBoxDisplay != null)
                    if (!SelectedDate.HasValue && !string.IsNullOrEmpty(PartTextBoxDisplay.Text))
                    {
                        return DateHelper.ConvertDateTime(PartTextBoxDisplay.Text, FormatNumber);
                    }
                return SelectedDate;
            }
            set
            {
                try
                {
                    if (value == null || value == DBNull.Value)
                    {
                        if (!AlwaysNow) SelectedDate = null;
                        else if (DefaultValue != null) SelectedDate = DefaultValue;
                        else SelectedDate = DateTime.Now;
                    }
                    else
                    {
                        SelectedDate = (DateTime)value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    SelectedDate = DateTime.Now;
                    value = DateTime.Now;
                }

                if (SelectedDate != null && PartTextBoxDisplay != null)
                {
                    if (value != null) PartTextBoxDisplay.Text = ((DateTime)value).ToString(FormatNumber);
                }
            }
        }

        public bool HasChildGrid { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            SelectedDate = null;
            PartTextBox.Clear();
            IsError = false;
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(Dispatcher, () => Focus()));
        }
        ~CoreDatePicker()
        {
            //Interaction.GetBehaviors(PartTextBoxDisplay).Remove(behavior);
        }
        public override void OnApplyTemplate()
        {
            PartPopup = GetTemplateChild("PART_Popup") as Popup;
            PartTextBox = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            PartTextBoxDisplay = GetTemplateChild("PART_TextBoxDisplay") as CoreTextBox;
            if (PartPopup != null)
            {
                PartPopup.Closed += PartPopupOnClosed;
            }
            if (PartTextBoxDisplay != null)
            {
                if (!HasChildGrid)
                {
                    //behavior = new TextBoxInputMaskBehavior(FormatNumber.Replace('d', '0')
                    //   .Replace('M', '0')
                    //   .Replace('y', '0')
                    //   .Replace('h', '0')
                    //   .Replace('s', '0')
                    //   .Replace('m', '0')
                    //   , PartTextBoxDisplay);
                    this.Unloaded += OnUnloaded;
                    //Interaction.GetBehaviors(PartTextBoxDisplay).Add(behavior);
                    PartTextBoxDisplay.IsSkipSentenceCase = true;
                    PartTextBoxDisplay.GotFocus += PartTextBoxDisplayOnGotFocus;
                    PartTextBoxDisplay.KeyDown += PartTextBoxDisplayOnKeyDown;
                    PartTextBoxDisplay.KeyUp += PartTextBoxDisplayOnKeyUp;

                    PartTextBoxDisplay.TextChanged += PartTextBoxDisplayOnTextChanged;
                    PartTextBoxDisplay.Loaded += PartTextBoxDisplayOnLoaded;
                }
                if (SelectedDate != null && PartTextBoxDisplay != null)
                {
                    PartTextBoxDisplay.Text = SelectedDate.Value.ToString(FormatNumber);
                }
            }
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DateTime.xaml"),
                    UriKind.Relative);

            base.OnApplyTemplate();
            if (PartPopup == null)
            {
                return;
            }
            var child = PartPopup.Child;
            if (child is System.Windows.Controls.Calendar)
            {
                (child as System.Windows.Controls.Calendar).Style = rDictionary["MetroCalendar"] as Style;
            }
            // if (PartTextBoxDisplay != null) PartTextBoxDisplay.TextChanged += PartTextBoxTextChanged;
            if (PartTextBox != null)
            {
                PartTextBox.GotFocus += PartTextBoxOnGotFocus;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //if (behavior != null)
            //{
            //    behavior.Dispose();
            //    behavior = null;
            //}
            Console.WriteLine("");
        }

        private void PartTextBoxDisplayOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (SelectedDate != null) PartTextBoxDisplay.Text = SelectedDate.Value.ToString(FormatNumber);
        }

        private void PartTextBoxDisplayOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
        }

        private void PartTextBoxDisplayOnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down)
                {
                    var isUp = e.Key == System.Windows.Input.Key.Up;
                    var indexText = PartTextBoxDisplay.SelectionStart;
                    var flag = FormatNumber[indexText];
                    if (!char.IsNumber(PartTextBoxDisplay.Text[indexText])) return;
                    int number;
                    switch (flag)
                    {
                        case 'd':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                var month = SelectedDate.Value.Month;
                                var year = SelectedDate.Value.Year;

                                number = isUp
                                    ? Convert.ToByte(PartTextBoxDisplay.Text.Substring(0, 2)) + 1
                                    : Convert.ToByte(PartTextBoxDisplay.Text.Substring(0, 2)) - 1;
                                if (number > DateTime.DaysInMonth(year, month))
                                    number = 1;
                                if (number == 0)
                                    number = DateTime.DaysInMonth(year, month);
                            }
                            PartTextBoxDisplay.Text = number.ToString("D2") +
                                                      PartTextBoxDisplay.Text.Substring(2);
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                        case 'M':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                number = isUp
                                    ? Convert.ToByte(PartTextBoxDisplay.Text.Substring(3, 2)) + 1
                                    : Convert.ToByte(PartTextBoxDisplay.Text.Substring(3, 2)) - 1;
                                if (number > 12)
                                    number = 1;
                                if (number == 0)
                                    number = 12;
                            }
                            PartTextBoxDisplay.Text = PartTextBoxDisplay.Text.Substring(0, 3) + number.ToString("D2") +
                                                      PartTextBoxDisplay.Text.Substring(5);
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                        case 'y':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                number = isUp
                                    ? Convert.ToInt16(PartTextBoxDisplay.Text.Substring(6, 4)) + 1
                                    : Convert.ToInt16(PartTextBoxDisplay.Text.Substring(6, 4)) - 1;
                                if (number == 0)
                                    number = DateTime.Now.Year;
                            }
                            PartTextBoxDisplay.Text = PartTextBoxDisplay.Text.Substring(0, 6) + number.ToString("D4") +
                                                      PartTextBoxDisplay.Text.Substring(10);
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                        case 'h':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                number = isUp
                                    ? Convert.ToByte(PartTextBoxDisplay.Text.Substring(11, 2)) + 1
                                    : Convert.ToInt16(PartTextBoxDisplay.Text.Substring(11, 2)) - 1;
                                if (number < 0)
                                    number = 24;
                                if (number > 24)
                                    number = 0;
                            }
                            PartTextBoxDisplay.Text = PartTextBoxDisplay.Text.Substring(0, 11) + number.ToString("D2") +
                                                      PartTextBoxDisplay.Text.Substring(13);
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                        case 'm':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                number = isUp
                                    ? Convert.ToByte(PartTextBoxDisplay.Text.Substring(14, 2)) + 1
                                    : Convert.ToInt16(PartTextBoxDisplay.Text.Substring(14, 2)) - 1;
                                if (number < 0)
                                    number = 59;
                                if (number > 59)
                                    number = 0;
                            }
                            PartTextBoxDisplay.Text = PartTextBoxDisplay.Text.Substring(0, 14) + number.ToString("D2") +
                                                      PartTextBoxDisplay.Text.Substring(16);
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                        case 's':
                            number = 0;
                            if (SelectedDate != null)
                            {
                                number = isUp
                                    ? Convert.ToByte(PartTextBoxDisplay.Text.Substring(17, 2)) + 1
                                    : Convert.ToInt16(PartTextBoxDisplay.Text.Substring(17, 2)) - 1;
                                if (number < 0)
                                    number = 59;
                                if (number > 59)
                                    number = 0;
                            }
                            PartTextBoxDisplay.Text = PartTextBoxDisplay.Text.Substring(0, 17) + number.ToString("D2");
                            PartTextBoxDisplay.SelectionStart = indexText;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                var repository = BaseDependency.Get<ILogRepository>();
                if (repository != null)
                    repository.Error(exception.ToString());
            }
        }

        public void OnBeforeValidate(HandleArgs e = null)
        {
            var handler = BeforeValidate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods        

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            var partTextBoxDisplay = PartTextBoxDisplay;
            if (partTextBoxDisplay != null) partTextBoxDisplay.Focus();
            base.OnGotFocus(e);
        }

        protected override void OnSelectedDateChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectedDateChanged(e);
            if (!AllowNull)
            {
                if (SelectedDate != null && PartTextBoxDisplay != null)
                {
                    PartTextBoxDisplay.Text = SelectedDate.Value.ToString(FormatNumber);
                }
            }
        }

        private static void FormatNumberCallBack(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CoreDatePicker;
            if (form != null)
            {
                form.FormatNumber = dependencyPropertyChangedEventArgs.NewValue.ToString();
            }
        }

        private void CallBack(object state)
        {
            Thread.Sleep(20);
            Manager.Timeout(Dispatcher, () => PartTextBoxDisplay.Focus());
        }

        private void PartPopupOnClosed(object sender, EventArgs eventArgs)
        {
            if (!AllowNull)
            {
                if (SelectedDate != null && PartTextBoxDisplay != null)
                {
                    PartTextBoxDisplay.Text = SelectedDate.Value.ToString(FormatNumber);
                }
            }
        }

        private void PartTextBoxDisplayOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            errorParse = false;
            PartTextBoxDisplay.LostFocus += PartTextBoxDisplayOnLostFocus;
        }

        private void PartTextBoxDisplayOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == System.Windows.Input.Key.Return && !SkipAutoFocus)
            {
                var parent = Manager.FindVisualParent<CoreUserControl>(this);
                if (parent != null)
                {
                    var valid = false;
                    foreach (var findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
                    {
                        if (valid && !findVisualChild.Name.Equals("PART_TextBoxDisplay")
                            && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus &&
                            !findVisualChild.Equals(PartTextBoxDisplay)
                            && !findVisualChild.Equals(PartTextBox) && (findVisualChild as IValueElement).CanFocus)
                        {
                            var parenCombo = Manager.FindVisualParent<CoreComboBox>(findVisualChild);
                            if (parenCombo == null)
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
                            if (!parenCombo.Equals(this))
                            {
                                var frameworkElement = findVisualChild;
                                if (frameworkElement.IsEnabled)
                                {
                                    frameworkElement.Focus();
                                    break;
                                }
                            }
                        }
                        if (findVisualChild.Equals(this))
                        {
                            valid = true;
                        }
                    }
                }
            }
        }

        private void PartTextBoxDisplayOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                PartTextBoxDisplay.LostFocus -= PartTextBoxDisplayOnLostFocus;
                if (!errorParse)
                {
                    if (!HasChildGrid)
                    {
                        var mask =
                            Interaction.GetBehaviors(PartTextBoxDisplay)
                                .OfType<TextBoxInputMaskBehavior>()
                                .FirstOrDefault();
                        if (mask != null)
                        {
                            var filter =
                                mask.InputMask.Replace('0', mask.PromptChar)
                                    .Replace('9', mask.PromptChar)
                                    .Replace('a', mask.PromptChar)
                                    .Replace('A', mask.PromptChar)
                                    .Replace('0', mask.PromptChar)
                                    .Replace('0', mask.PromptChar);
                            if (!filter.Equals(PartTextBoxDisplay.Text))
                            {
                                try
                                {
                                    var dtfi = new DateTimeFormatInfo();
                                    dtfi.ShortDatePattern = FormatNumber;
                                    dtfi.DateSeparator = "-";
                                    var currentDate = Convert.ToDateTime(PartTextBoxDisplay.Text, dtfi);
                                    SelectedDate = currentDate;
                                }
                                catch (Exception)
                                {
                                    SelectedDate = DateTime.Now;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var dtfi = new DateTimeFormatInfo();
                                dtfi.ShortDatePattern = FormatNumber;
                                dtfi.DateSeparator = "-";
                                if (PartTextBoxDisplay.Text == string.Empty)
                                    return;
                                var currentDate = Convert.ToDateTime(PartTextBoxDisplay.Text, dtfi);
                                SelectedDate = currentDate;
                            }
                            catch (Exception)
                            {
                                SelectedDate = DateTime.Now;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!errorParse)
                {
                    PartTextBoxDisplay.Text = "";
                }
                ThreadPool.QueueUserWorkItem(CallBack);
                errorParse = true;
            }

            if (!string.IsNullOrEmpty(PartTextBoxDisplay.Text))
                IsError = false;
        }

        private void PartTextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var partTextBoxDisplay = PartTextBoxDisplay;
            if (partTextBoxDisplay != null) partTextBoxDisplay.Focus();
        }

        #endregion
    }
}