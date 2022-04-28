using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using Core.Framework.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using Core.Framework.Windows.Behaviours;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    public class CoreTextBox : TextBox, IValidateControl, IValueElement, IControlAuthentication
    {
        // Using a DependencyProperty as the backing store for CurrentDataGrid.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty CurrentDataGridProperty =
            DependencyProperty.Register(
                "CurrentDataGrid",
                typeof(CoreDataGrid),
                typeof(CoreTextBox),
                new PropertyMetadata(ChangeCurrentDataGrid));

        public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register(
            "DefaultText",
            typeof(string),
            typeof(CoreTextBox),
            new UIPropertyMetadata(""));

        public static readonly DependencyProperty FilterTypeByStringProperty =
            DependencyProperty.Register(
                "FilterTypeByString",
                typeof(string),
                typeof(CoreTextBox),
                new UIPropertyMetadata("string", PropertyChangedCallback));

        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(CoreTextBox),
            new UIPropertyMetadata(false));

        #endregion

        #region Fields


        //private TextBoxInputMaskBehavior behavior;

        private bool isError;

        private string mask;

        private char prompChar;

        private Regex regex;

        private Type type;

        #endregion

        #region Constructors and Destructors


        public CoreTextBox()
        {
            Manager.RegisterFormGrid(this);
            this.FontWeight = FontWeights.Bold;
            this.IsError = false;
            this.AcceptsTab = false;
            this.Loaded += this.OnLoaded;

            this.Unloaded += OnUnloaded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += TimerOnTick;

            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            timer.Stop();
            OnAutoSearch();
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            for (int i = 0; i < Interaction.GetBehaviors(this).Count; i++)
            {
                var disposable = Interaction.GetBehaviors(this)[i] as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                Interaction.GetBehaviors(this).RemoveAt(i);
            }
        }

        #endregion

        #region Public Events

        public event EventHandler AutoSearch;
        public event EventHandler<HandleArgs> BeforeValidate;

        #endregion

        #region Public Properties

        public bool CanFocus
        {
            get { return true; }
        }

        public CoreDataGrid CurrentDataGrid
        {
            get
            {
                return (CoreDataGrid)this.GetValue(CurrentDataGridProperty);
            }
            set
            {
                this.SetValue(CurrentDataGridProperty, value);
            }
        }

        public string DefaultText
        {
            get
            {
                return (string)this.GetValue(DefaultTextProperty);
            }
            set
            {
                this.SetValue(DefaultTextProperty, value);
            }
        }

        public Type FilterType
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                if (this.type == typeof(Int16) || this.type == typeof(short?))
                {
                    this.MaxLength = 18;
                    this.MaxNumber = Int16.MaxValue;
                }
                else if (this.type == typeof(Byte) || this.type == typeof(byte?))
                {
                    this.MaxNumber = Byte.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(Int32) || this.type == typeof(Int32))
                {
                    this.MaxNumber = Int32.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(float) || this.type == typeof(float))
                {
                    this.MaxNumber = Int32.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(double) || this.type == typeof(double))
                {
                    this.MaxNumber = Int32.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(Int64) || this.type == typeof(Int64?))
                {
                    this.MaxNumber = Int64.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type.FullName.Contains("double") || this.type.FullName.Contains("float") || this.type.FullName.Contains("single")
                         || this.type.FullName.Contains("decimal"))
                {
                    this.MaxLength = 18;
                }
                else if (this.type.FullName.Contains("Char"))
                {
                    this.MaxLength = 1;
                }
            }
        }

        public string FilterTypeByString
        {
            get
            {
                return (string)this.GetValue(FilterTypeByStringProperty);
            }
            set
            {
                this.SetValue(FilterTypeByStringProperty, value);
            }
        }

        public bool IsError
        {
            get
            {
                return this.isError;
            }
            set
            {
                isError = value;
                BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
                Background = value ? new SolidColorBrush(Colors.Tomato) : new SolidColorBrush(Colors.Transparent);
            }
        }

        // Using a DependencyProperty as the backing store for FilterTypeByString.  This enables animation, styling, binding, etc...

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
                if (string.IsNullOrEmpty(this.Mask))
                {
                    return string.IsNullOrEmpty(this.Text);
                }
                //if (ValidateMethod != null)
                //{
                //    var parentUserControl = Manager.FindVisualParent<UserControl>(this);
                //    if (parentUserControl != null)
                //    {
                //        var methodUserControl = parentUserControl.GetType().GetMethod(ValidateMethod);
                //        if (methodUserControl != null)
                //        {
                //            var result = methodUserControl.Invoke(parentUserControl);
                //        }
                //    }
                //}
                //edit by chandra
                // tidak merelease memory, untuk sementari di hide
                //return this.behavior.Provider.AvailableEditPositionCount != 0;
                return true;
            }
        }

        //public bool IsRequired { get; set; }

        public bool IsRequired
        {
            get
            {
                return (bool)this.GetValue(IsRequiredProperty);
            }
            set
            {
                this.SetValue(IsRequiredProperty, value);
            }
        }
        public string Key { get; set; }

        public string Mask
        {
            get
            {
                return this.mask;
            }
            set
            {
                this.mask = value;
                if (this.IsLoaded)
                {
                    if (!string.IsNullOrEmpty(this.mask))
                    {
                        for (int i = 0; i < Interaction.GetBehaviors(this).Count; i++)
                        {
                            var disposable = Interaction.GetBehaviors(this)[i] as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                            Interaction.GetBehaviors(this).RemoveAt(i);
                        }

                        //this.behavior = new TextBoxInputMaskBehavior(this.mask, this, this.PrompChar);
                        //Interaction.GetBehaviors(this).Add(behavior);
                        // behavior.Attach(this);
                        this.Text = "";
                        //edit by chandra
                        // tidak merelease memory, untuk sementari di hide
                        //this.behavior.BindingMask();
                    }
                    else if (string.IsNullOrEmpty(this.mask))
                    {
                        //edit by chandra
                        // tidak merelease memory, untuk sementari di hide
                        //if (this.behavior != null)
                        //{
                        //    Interaction.GetBehaviors(this).Remove(this.behavior);
                        //    this.behavior.Clear();
                        //}
                        // behavior = null;
                    }
                }
            }
        }

        public decimal MaxNumber { get; set; }

        public char PrompChar
        {
            get
            {
                return this.prompChar;
            }
            set
            {
                this.prompChar = value;
                if (this.IsLoaded)
                {
                    if (!string.IsNullOrEmpty(this.mask))
                    {
                        //edit by chandra
                        // tidak merelease memory, untuk sementari di hide
                        //if (this.behavior == null)
                        //{
                        //    //this.behavior = new TextBoxInputMaskBehavior();
                        //    //Interaction.GetBehaviors(this).Add(this.behavior);

                        //    //this.behavior = new TextBoxInputMaskBehavior();
                        //    //Interaction.GetBehaviors(this).Add(this.behavior);                            
                        //}
                        //else
                        {
                            if (!string.IsNullOrEmpty(this.mask))
                            {
                                for (int i = 0; i < Interaction.GetBehaviors(this).Count; i++)
                                {
                                    var disposable = Interaction.GetBehaviors(this)[i] as IDisposable;
                                    if (disposable != null)
                                    {
                                        disposable.Dispose();
                                    }
                                    Interaction.GetBehaviors(this).RemoveAt(i);
                                }

                                //this.behavior = new TextBoxInputMaskBehavior(this.mask, this, this.prompChar);
                                //Interaction.GetBehaviors(this).Add(behavior);
                                // behavior.Attach(this);
                                this.Text = "";
                                //edit by chandra
                                // tidak merelease memory, untuk sementari di hide
                                // this.behavior.BindingMask();
                            }
                            else if (string.IsNullOrEmpty(this.mask))
                            {
                                //edit by chandra
                                // tidak merelease memory, untuk sementari di hide
                                //if (this.behavior != null)
                                //{
                                //    Interaction.GetBehaviors(this).Remove(this.behavior);
                                //    this.behavior.Clear();
                                //}
                                // behavior = null;
                            }
                        }
                    }
                }
            }
        }

        public bool SkipAutoFocus { get; set; }
        public string ValidateMethod { get; set; }


        public object Value
        {
            get
            {
                if (string.IsNullOrEmpty(this.Mask))
                {
                    return this.Text;
                }
                return this.Text.Replace(this.PrompChar.ToString(CultureInfo.InvariantCulture), "").Replace("'", "''");
            }
            set
            {
                this.Text = value == null ? this.DefaultText : value.ToString();
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            this.Text = string.Empty;
            this.IsError = false;
            //var isReuiredTemp = IsRequired;
            //IsRequired = false;
            //Text = String.Empty;
            //ThreadPool.QueueUserWorkItem(
            //    state => Manager.Timeout(Dispatcher, () =>
            //                                             {
            //                                                 IsRequired = (bool)state;
            //                                             }), isReuiredTemp);
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher, () => this.Focus()));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var rDictionary = new ResourceDictionary();
            rDictionary.Source = new Uri(
                string.Format("/Core.Framework.Windows;component/Styles/Controls.TextBox.xaml"),
                UriKind.Relative);
            //rDictionary.Source = new Uri(
            //    string.Format("/Core.Framework.Windows;component/Styles/VS/TextBox.xaml"),
            //    UriKind.Relative);
            //Style = rDictionary["MetroTextBox"] as Style;
            //Style = rDictionary["StandardTextBox"] as Style;      

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

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.CurrentDataGrid != null)
            {
                this.CurrentDataGrid.FreezeRetrun = true;
            }
            base.OnGotFocus(e);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.CurrentDataGrid != null)
                this.CurrentDataGrid.Refresh();
            if (e.Key == System.Windows.Input.Key.Return && !this.SkipAutoFocus)
            {
                if (AcceptsReturn) return;
                var parent = Manager.FindVisualParent<CoreUserControl>(this);
                if (parent != null)
                {
                    var valid = false;
                    foreach (var findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
                    {
                        if (valid && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus && (findVisualChild as IValueElement).CanFocus)
                        {
                            var parentTabItem = Manager.FindVisualParent<TabItem>(findVisualChild);
                            if (parentTabItem != null)
                                parentTabItem.IsSelected = true;
                            if (findVisualChild.IsEnabled && findVisualChild.IsVisible)
                            {
                                Manager.Timeout(Dispatcher, () =>
                                {
                                    if (findVisualChild.IsEnabled && findVisualChild.IsVisible)
                                    {
                                        findVisualChild.Focus();
                                    }
                                });
                                break;
                            }
                        }

                        if (valid && findVisualChild is PasswordBox && (findVisualChild as PasswordBox).IsEnabled)
                        {
                            if (findVisualChild.IsEnabled && findVisualChild.IsVisible)
                            {
                                Manager.Timeout(Dispatcher, () =>
                                {

                                    {
                                        findVisualChild.Focus();
                                    }
                                });
                                break;
                            }
                        }

                        if (findVisualChild.Equals(this))
                            valid = true;
                    }
                    if (valid)
                    {
                        //e.Handled = true;
                        OnNextFocus();
                    }
                }

            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == System.Windows.Input.Key.Return && !this.SkipAutoFocus)
            {

            }
            //base.OnKeyDown(e);

        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.CurrentDataGrid != null)
            {
                this.CurrentDataGrid.Refresh();
            }
            //if (e.Key == System.Windows.Input.Key.Return && !this.SkipAutoFocus)
            //{
            //    var parent = Manager.FindVisualParent<CoreUserControl>(this);
            //    if (parent != null)
            //    {
            //        bool valid = false;
            //        foreach (FrameworkElement findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
            //        {
            //            if (valid && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus && (findVisualChild as IValueElement).CanFocus)
            //            {
            //                var parentTabItem = Manager.FindVisualParent<TabItem>(findVisualChild);
            //                if (parentTabItem != null)
            //                {
            //                    parentTabItem.IsSelected = true;
            //                }
            //                Manager.Timeout(Dispatcher, () =>
            //                {
            //                    if (findVisualChild.IsEnabled)
            //                    {
            //                        findVisualChild.Focus();
            //                    }
            //                });
            //                break;
            //            }
            //            if (findVisualChild.Equals(this))
            //            {
            //                valid = true;
            //            }
            //        }
            //    }
            //}
            //base.OnKeyUp(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.CurrentDataGrid != null)
            {
                this.CurrentDataGrid.FreezeRetrun = false;
            }

            var requiredControl = Manager.FindVisualParent<RequiredGrid>(this);
            if (requiredControl != null)
            {
                if (requiredControl.IsRequired)
                {
                    if (this.IsRequired)
                    {
                        this.IsError = this.IsNull;
                    }
                    else
                    {
                        this.IsError = this.IsNull;
                    }
                }
                else
                {
                    if (this.IsRequired)
                    {
                        this.IsError = this.IsNull;
                    }
                }
            }
            else
            {
                if (this.IsRequired)
                {
                    this.IsError = this.IsNull;
                }
            }

            if (!IsSkipSentenceCase)
            {
                try
                {
                    var textBefores = Text.Split(' ');
                    var textFinal = string.Empty;
                    foreach (var textBefore in textBefores)
                    {
                        var firstText = textBefore.Substring(0, 1).ToUpper();
                        var lastText = textBefore.Substring(1, textBefore.Length - 1).ToLower();
                        if (string.IsNullOrEmpty(textFinal))
                            textFinal = firstText + lastText;
                        else
                            textFinal = textFinal + " " + (firstText + lastText);
                    }
                    Text = textFinal;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

            }

            base.OnLostFocus(e);
        }

        public bool IsSkipSentenceCase
        {
            get { return (bool)GetValue(IsSkipSentenceCaseProperty); }
            set { SetValue(IsSkipSentenceCaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSkipSentenceCaseProperty =
            DependencyProperty.Register("IsSkipSentenceCase", typeof(bool), typeof(CoreTextBox), new UIPropertyMetadata(false));



        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!this.IsEnabled)
            {
                this.IsEnabled = true;
            }
            base.OnMouseDoubleClick(e);
        }
        public enum TypeFilter
        {
            CharacterAndAlphaNumeric, NumericAndAlphaNumeric, Numeric, Character, AlphaNumeric, None
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (this.FilterType != null)
            {
                if (this.FilterType.FullName.ToLower().Contains("int")
                    || this.FilterType.FullName.ToLower().Contains("byte"))
                {
                    this.regex = new Regex(@"^[0-9]$");
                }
                else if (this.FilterType.FullName.ToLower().Contains("double")
                         || this.FilterType.FullName.ToLower().Contains("float")
                         || this.FilterType.FullName.ToLower().Contains("decimal")
                         || this.FilterType.FullName.ToLower().Contains("single"))
                {
                    this.regex = new Regex(@"^[0-9\.]$");
                }
                else if (this.FilterType.FullName.ToLower().Contains("char"))
                {
                    this.MaxLength = 1;
                    this.regex = null;
                }
                else
                {
                    this.regex = null;
                }
            }
            switch (TypeWrite)
            {
                case TypeFilter.Numeric:
                    this.regex = new Regex(@"^[0-9\.]$");
                    break;
                case TypeFilter.Character:
                    this.regex = new Regex(@"^[a-z\.]$");
                    break;
                case TypeFilter.AlphaNumeric:
                    break;
                case TypeFilter.NumericAndAlphaNumeric:
                    break;
                case TypeFilter.CharacterAndAlphaNumeric:
                    break;
                case TypeFilter.None:
                    break;

            }
            if (this.regex != null && !this.regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewTextInput(e);
            }
        }

        public TypeFilter TypeWrite { get; set; }

        private static void ChangeCurrentDataGrid(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBox;
            if (form != null)
            {
                form.CurrentDataGrid = e.NewValue as CoreDataGrid;
            }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CoreTextBox;
            if (form != null)
            {
                if (dependencyPropertyChangedEventArgs.NewValue.ToString().ToLower().Equals("string"))
                {
                    form.FilterType = typeof(string);
                }
                else if (dependencyPropertyChangedEventArgs.NewValue.ToString().ToLower().Equals("double"))
                {
                    form.FilterType = typeof(double);
                }
                else if (dependencyPropertyChangedEventArgs.NewValue.ToString().ToLower().Equals("integer"))
                {
                    form.FilterType = typeof(int);
                }
                else if (dependencyPropertyChangedEventArgs.NewValue.ToString().ToLower().Equals("char"))
                {
                    form.FilterType = typeof(char);
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!string.IsNullOrEmpty(this.mask))
            {
                

                //edit by chandra
                // tidak merelease memory, untuk sementari di hide
                ////if (this.behavior == null)
                ////{
                ////    //this.behavior = new TextBoxInputMaskBehavior();
                ////    //Interaction.GetBehaviors(this).Add(this.behavior);

                ////    for (int i = 0; i < Interaction.GetBehaviors(this).Count; i++)
                ////    {
                ////        var disposable = Interaction.GetBehaviors(this)[i] as IDisposable;
                ////        if (disposable != null)
                ////        {
                ////            disposable.Dispose();
                ////        }
                ////        Interaction.GetBehaviors(this).RemoveAt(i);
                ////    }

                ////    this.behavior = new TextBoxInputMaskBehavior(this.mask, this, this.prompChar);
                ////    //Interaction.GetBehaviors(this).Add(behavior);
                ////    // behavior.Attach(this);
                ////    this.Text = "";
                ////    this.behavior.BindingMask();
                ////}
                //else
                {
                    if (!string.IsNullOrEmpty(this.mask))
                    {
                        for (int i = 0; i < Interaction.GetBehaviors(this).Count; i++)
                        {
                            var disposable = Interaction.GetBehaviors(this)[i] as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                            Interaction.GetBehaviors(this).RemoveAt(i);
                        }

                        //this.behavior = new TextBoxInputMaskBehavior(this.mask, this, this.prompChar);
                        //Interaction.GetBehaviors(this).Add(behavior);
                        // behavior.Attach(this);
                        //this.Text = "";
                        //edit by chandra
                        // tidak merelease memory, untuk sementari di hide
                        //this.behavior.BindingMask();
                    }
                    else if (string.IsNullOrEmpty(this.mask))
                    {
                        //edit by chandra
                        // tidak merelease memory, untuk sementari di hide
                        //if (this.behavior != null)
                        //{
                        //    Interaction.GetBehaviors(this).Remove(this.behavior);
                        //    this.behavior.Clear();
                        //}
                        // behavior = null;
                    }
                }
            }
        }

        #endregion

        #region Implementation of IControlAuthentication

        public static readonly DependencyProperty UseAuthenticationProperty = DependencyProperty.Register("UseAuthentication", typeof(bool), typeof(CoreTextBox), new UIPropertyMetadata(false));

        /// <summary>
        /// Identity dari control
        /// </summary>
        public string IdentityAuthentication
        {
            get
            {
                var parent = this.TryFindParent<IControlAuthentication>();
                var str = new StringBuilder();
                int index = 0;
                while (parent != null)
                {
                    if (index == 0)
                        str.Append(parent.IdentityAuthentication);
                    else
                        str.Append("-" + parent.IdentityAuthentication);
                    index++;
                    parent = (parent as FrameworkElement).TryFindParent<IControlAuthentication>();
                }
                if (string.IsNullOrEmpty(str.ToString()))
                    return Name;
                return str + "-" + Name;
            }
        }

        /// <summary>
        /// Flag yang di gunakan untuk mengetahui Control itu harus di authentication atau tidak
        /// </summary>
        public bool UseAuthentication
        {
            get { return (bool)GetValue(UseAuthenticationProperty); }
            set { SetValue(UseAuthenticationProperty, value); }
        }

        /// <summary>
        /// Method yang di eksekusi ketika Control memiliki flag true    
        /// </summary>
        /// <returns></returns>
        public bool ExecuteAuthentication()
        {
            return false;
        }

        #endregion

        private object tempValue;
        private DispatcherTimer timer;
        public event EventHandler NextFocus;

        internal DataGridCell ParentCell { get; set; }

        internal object TempValue
        {
            get { return tempValue; }
            set { tempValue = value; }
        }

        //public object TempValue { get; set; }
        public virtual void OnNextFocus()
        {
            if (NextFocus != null)
                NextFocus.Invoke(this, EventArgs.Empty);
        }

        #region Overrides of TextBoxBase

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        /// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            timer.Stop();
            timer.Start();
            base.OnTextChanged(e);
        }

        #endregion

        protected virtual void OnAutoSearch()
        {
            if (AutoSearch != null) AutoSearch.Invoke(this, EventArgs.Empty);
        }
    }
}