using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Framework.Helper;
    using Core.Framework.Helper.Date;
    using Core.Framework.Model;
    using Core.Framework.Model.Attr;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Controls;
    using Core.Framework.Windows.Helper;

    public class FormGrid : Grid, INotifyPropertyChanged, IValueElement
    {

        // Using a DependencyProperty as the backing store for CurrentForm.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty CurrentFormProperty = DependencyProperty.Register(
            "CurrentForm",
            typeof(FormGrid),
            typeof(FormGrid),
            new UIPropertyMetadata(null));

        #endregion

        #region Fields

        private readonly CoreDictionary<string, object> tempDictionnary = new CoreDictionary<string, object>();

        private bool isValidate;

        private CoreDictionary<string, FrameworkElement> listControl;

        #endregion

        #region Constructors and Destructors


        public FormGrid()
        {
            Manager.RegisterFormGrid(this);
            this.CurrentForm = this;
            children = new List<FrameworkElement>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties



        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(FormGrid), new PropertyMetadata(false, IsBusyCallBack));

        private static void IsBusyCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as FormGrid;
            if (form != null)
            {
                if ((bool)e.NewValue)
                    form.Children.Add(new BusyIndicator());
                else
                {
                    form.Children.Remove(form.Children.OfType<BusyIndicator>().FirstOrDefault());
                }
            }
        }


        public bool IsDisabled
        {
            get { return (bool)GetValue(IsDisabledProperty); }
            set
            {
                SetValue(IsDisabledProperty, value);
                IsEnabled = !value;
                //foreach (var findVisualChild in Manager.FindVisualChildren<FrameworkElement>(this))
                //{
                //    findVisualChild.IsEnabled = !value;

                //}
            }
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register("IsDisabled", typeof(bool), typeof(FormGrid), new UIPropertyMetadata(false));

        private List<FrameworkElement> children;


        public FormGrid CurrentForm
        {
            get
            {
                return (FormGrid)this.GetValue(CurrentFormProperty);
            }
            set
            {
                this.SetValue(CurrentFormProperty, value);
            }
        }

        public new object DataContext
        {
            get
            {
                object model = base.DataContext;
                if (model is TableItem)
                {
                    // (model as TableItem).OnInit(this.DataInForm);
                }
                return model;
            }
            set
            {
                this.DataInForm = null;
                base.DataContext = null;
                Manager.Timeout(Dispatcher, () => base.DataContext = value);
                if (value is TableItem)
                {
                    ThreadPool.QueueUserWorkItem(BindingData, (value as TableItem).GetDictionary());

                }
            }
        }

        private void BindingData(object state)
        {

            var obj = state as CoreDictionary<string, object>;
            Manager.Timeout(Dispatcher, () => this.DataInForm = obj);
        }

        public CoreDictionary<string, object> DataInForm
        {
            get
            {
                var data = new CoreDictionary<string, object>(this.tempDictionnary);

                foreach (
                    IValueElement element in children
                            .Cast<IValueElement>())
                {
                    try
                    {
                        IValueConverter converter = null;
                        var frameworkElement = element as FrameworkElement;
                        if (frameworkElement != null && frameworkElement.Tag != null)
                        {
                            var type = frameworkElement.Tag as Type;
                            if (type != null)
                            {
                                converter = Activator.CreateInstance(type) as IValueConverter;
                            }
                        }
                        object obj;
                        if (!string.IsNullOrEmpty(element.Key))
                        {
                            foreach (
                                string key in element.Key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => n.ToLower()))
                            {
                                if (data.TryGetValue(key, out obj))
                                {
                                    if (converter == null)
                                    {
                                        data[key] = element.Value;
                                    }
                                    else
                                    {
                                        data[key] = converter.ConvertBack(element.Value, null, null, null);
                                    }
                                }
                                else
                                {
                                    if (converter == null)
                                    {                                      
                                        data.Add(key, element.Value);
                                    }
                                    else
                                    {
                                        data.Add(key, converter.ConvertBack(element.Value, null, null, null));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception o)
                    {
                        Debug.Print(o.Message);
                    }
                }
                return data;//.Where(n => n.Value != string.Empty).ToCoreDictionary(n => n.Key, n => n.Value);
            }
            set
            {
                if (value == null)
                {
                    //children = Manager.FindVisualChildren<FrameworkElement>(this);
                    foreach (IValueElement frameworkElement in children)
                    {
                        Thread.Sleep(2);
                        var parent = (frameworkElement as FrameworkElement).TryFindParent<FormGrid>();
                        if (parent == null)
                            frameworkElement.Value = null;
                        else if (parent.IsCannotClearFrom == false)
                            frameworkElement.Value = null;

                    }
                    this.tempDictionnary.Clear();
                }
                else
                {
                    CoreDictionary<string, object> data = value;
                    var listControl = children
                        .Cast<IValueElement>().ToList();
                    foreach (var o in data)
                    {
                        Thread.Sleep(2);
                        IValueElement child =
                            listControl
                                .FirstOrDefault(
                                    n =>
                                        n.Key != null
                                        && n.Key.ToLower().Split(new[] { ',' }).Any(z => z.Equals(o.Key.ToLower())));
                        if (child != null)
                        {
                            child.Value = o.Value;

                            this.OnPropertyChanged(new PropertyChangedEventArgs(child.Key));
                        }
                        else
                        {
                            this.OnPropertyChanged(new PropertyChangedEventArgs(o.Key));
                            object result;
                            if (this.tempDictionnary.TryGetValue(o.Key.ToLower(), out result))
                            {
                                this.tempDictionnary[o.Key.ToLower()] = o.Value;
                            }
                            else
                            {
                                this.tempDictionnary.Add(o.Key.ToLower(), o.Value);
                            }
                        }
                    }
                }
            }
        }

        public bool IsCannotClearFrom { get; set; }

        public bool IsValidate
        {
            get
            {
                return this.isValidate;
            }
            set
            {
                this.isValidate = value;
                if (!string.IsNullOrEmpty(this.ListControlEnabled))
                {
                    foreach (string controlName in this.ListControlEnabled.Split(new[] { ',' }))
                    {
                        object control = this.FindName(controlName);
                        if (control != null)
                        {
                            var frameworkElement = control as FrameworkElement;
                            if (frameworkElement != null)
                            {
                                frameworkElement.IsEnabled = this.isValidate;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(this.ListControlDisabled))
                {
                    foreach (string controlName in this.ListControlDisabled.Split(new[] { ',' }))
                    {
                        object control = this.FindName(controlName);
                        if (control != null)
                        {
                            var frameworkElement = control as FrameworkElement;
                            if (frameworkElement != null)
                            {
                                frameworkElement.IsEnabled = !this.isValidate;
                            }
                        }
                    }
                }
            }
        }

        public string ListControlDisabled { get; set; }
        public string ListControlEnabled { get; set; }

        #endregion

        #region Public Indexers

        public object this[string key]
        {
            get
            {
                this.BindingControl();
                var control = this.listControl[key];
                if (control == null)
                {
                    object obj;
                    if (tempDictionnary.TryGetValue(key, out obj))
                        return tempDictionnary[key];
                }
                return control;
            }
            set
            {
                this.BindingControl();

                FrameworkElement control = this.listControl[key];
                if (control == null)
                {
                    object obj;
                    if (tempDictionnary.TryGetValue(key, out obj))
                        tempDictionnary[key] = value;
                    else
                        tempDictionnary.Add(key, value);
                    return;
                }
                if (control != null)
                {
                    if (control is IValidateControl)
                    {
                        if (value == null)
                        {
                            (control as IValidateControl).ClearValueControl();
                        }
                    }
                    if (control is IValueElement)
                    {
                        (control as IValueElement).Value = value;
                    }
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ClearForm()
        {
            this.DataInForm = null;
        }

        public event EventHandler<EventArgs> FinishDefine;
        public void DefineType(Type model)
        {
            if (model != null)
            {
                object intance = Activator.CreateInstance(model);
                var properties = intance.GetType().GetProperties().ToArray();
                Parallel.For(0, properties.Length, (i) =>
                {
                    var propertyInfo = properties[i];
                    Manager.Timeout(Dispatcher, () =>
                    {
                        IValueElement control =
                        Manager.FindVisualChildren<FrameworkElement>(this)
                            .OfType<IValueElement>()
                            .FirstOrDefault(n => n.Key != null && n.Key.ToLower().Equals(propertyInfo.Name.ToLower()));
                        if (control != null)
                        {
                            FieldAttribute fieldAttribute =
                                propertyInfo.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault();
                            if (fieldAttribute != null)
                            {
                                if (fieldAttribute.Length != null)
                                {
                                    if (control is CoreTextBox)
                                    {
                                        (control as CoreTextBox).MaxLength = fieldAttribute.Length.Value;
                                        (control as CoreTextBox).FilterType = propertyInfo.PropertyType;
                                    }
                                }
                                if (control is IValidateControl)
                                {
                                    (control as IValidateControl).IsRequired = fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull;
                                }
                                if (control is CoreDatePicker)
                                {
                                    var defaultProperty = propertyInfo.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault();
                                    if (defaultProperty != null)
                                        if (defaultProperty.Value.ToString().Equals("Now"))
                                        {
                                            (control as CoreDatePicker).AlwaysNow = true;
                                        }
                                        else
                                        {
                                            (control as CoreDatePicker).AlwaysNow = true;
                                            (control as CoreDatePicker).DefaultValue = DateHelper.ConvertDateTime(defaultProperty.Value.ToString());
                                        }
                                }
                            }
                        }

                        if (i + 1== properties.Length)
                            OnFinishDefine();
                    });
                });
                //foreach (PropertyInfo propertyInfo in properties)
                //{

                //}
            }
            else
            {
                foreach (
                    IValidateControl control in
                        Manager.FindVisualChildren<FrameworkElement>(this).OfType<IValidateControl>())
                {
                    control.IsRequired = false;
                }
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Reset()
        {
            foreach (var element in this.DataInForm)
            {
                this[element.Key] = null;
            }
        }

        public void SetValue(string key, object value)
        {
            object result;
            if (this.tempDictionnary.TryGetValue(key, out result))
            {
                this.tempDictionnary[key] = value;
            }
            else
            {
                this.tempDictionnary.Add(key, value);
            }
        }

        #endregion

        #region Methods

        private void BindingControl()
        {
            if (this.listControl == null)
            {
                this.listControl = new CoreDictionary<string, FrameworkElement>();
                foreach (
                    FrameworkElement result in
                        Manager.FindVisualChildren<FrameworkElement>(this).Where(element => element is IValueElement))
                {
                    var valueElement = result as IValueElement;
                    if (valueElement != null)
                    {
                        FrameworkElement obj;
                        if (string.IsNullOrEmpty(valueElement.Key))
                        {
                            continue;
                        }

                        if (!this.listControl.TryGetValue(valueElement.Key, out obj))
                        {
                            this.listControl.Add(valueElement.Key, result);
                        }
                    }
                }
            }
        }

        #endregion

        public bool CanFocus
        {
            get { return false; }
        }

        public string Key
        {
            get { return ""; }
        }

        public object Value
        {
            get { return DataInForm; }
            set
            {
                DataInForm = value as CoreDictionary<string, object>;
            }
        }

        public void AddChlidValidation(FrameworkElement control)
        {
            children.Add(control);
        }

        protected virtual void OnFinishDefine()
        {
            FinishDefine?.Invoke(this, EventArgs.Empty);
        }
    }
}