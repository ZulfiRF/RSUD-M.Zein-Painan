using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Model;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Ribbon;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using Selector = Core.Framework.Windows.Implementations.Common.Primitives.Selector;
using TextBox = System.Windows.Controls.TextBox;
using Timer = System.Threading.Timer;

namespace Core.Framework.Windows.Implementations
{
    [TemplatePart(Name = "CustomPART_EditableTextBox", Type = typeof(CoreTextBox))]
    [TemplatePart(Name = "DropDownScrollViewer", Type = typeof(CoreListBox))]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    public class CoreComboBoxGrid : ComboBox, IValueElement, IValidateControl
    {
        #region Static Fields

        public static readonly DependencyProperty AutoCorrectionProperty = DependencyProperty.Register(
            "AutoCorrection",
            typeof(bool),
            typeof(CoreComboBoxGrid),
            new UIPropertyMetadata(true));

        public static readonly DependencyProperty CurrentDataGridProperty =
            DependencyProperty.Register(
                "CurrentDataGrid",
                typeof(CoreDataGrid),
                typeof(CoreComboBoxGrid),
                new PropertyMetadata(ChangeCurrentDataGrid));

        public static readonly DependencyProperty DomainNameSpacesProperty =
            DependencyProperty.Register(
                "DomainNameSpaces",
                typeof(string),
                typeof(CoreComboBoxGrid),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(CoreComboBoxGrid),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(CoreComboBoxGrid),
            new PropertyMetadata(ChangeSource));

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(CoreComboBoxGrid),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty LimitSelectedItemsProperty =
            DependencyProperty.Register(
                "LimitSelectedItems",
                typeof(int),
                typeof(CoreComboBoxGrid),
                new UIPropertyMetadata(5));

        public static readonly DependencyProperty MultipleSelectedProperty =
            DependencyProperty.Register(
                "MultipleSelected",
                typeof(bool),
                typeof(CoreDataGrid),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty SearchTypeWithProperty = DependencyProperty.Register(
            "SearchTypeWith",
            typeof(SearchType),
            typeof(CoreComboBoxGrid),
            new UIPropertyMetadata(SearchType.Contains));

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(CoreComboBoxGrid),
            new PropertyMetadata(ChangeSelectedItem));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(List<object>),
            typeof(CoreComboBoxGrid),
            new UIPropertyMetadata(new List<object>(), SelectedItemsChanged));

        public static readonly DependencyProperty UsingSearchByFrameworkProperty =
            DependencyProperty.Register(
                "UsingSearchByFramework",
                typeof(bool),
                typeof(CoreComboBoxGrid),
                new UIPropertyMetadata(false));

        #endregion Static Fields

        #region Fields

        private readonly List<object> selectedItems = new List<object>();

        private readonly DispatcherTimer timerForSearch;
        internal List<object> ListSource;

        internal bool Searching;

        private object chooseItem2;


        private CoreTextBox editableTextBox;

        private SelectionChangedEventArgs eventArgs;

        private bool freeze;

        private bool isError;


        private CoreListBox listBoxPopUp;

        private bool pressEnter;


        #endregion Fields

        #region Constructors and Destructors


        static CoreComboBoxGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CoreComboBoxGrid),
                new FrameworkPropertyMetadata(typeof(CoreComboBoxGrid)));
        }

        public CoreComboBoxGrid()
        {
            IsEditable = true;
            DataContextChanged += CustomComboBoxDataContextChanged;
            DropDownClosed += CoreComboBoxGridDropDownClosed;
            IsError = false;
            AutoCorrection = true;
            timerForSearch = new DispatcherTimer(
                TimeSpan.FromMilliseconds(500),
                DispatcherPriority.DataBind,
                CallbackSearchData,
                Dispatcher);
            timerForSearch.Stop();
        }

        #endregion Constructors and Destructors


        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

        public event EventHandler ItemsSourceChanged;

        public new event SelectionChangedEventHandler SelectionChanged;

        public event TextChangedEventHandler TextChanged;

        public event EventHandler<ItemEventArgs<object>> RemoveItem;

        protected void OnRemoveItem(ItemEventArgs<object> e)
        {
            EventHandler<ItemEventArgs<object>> handler = RemoveItem;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ItemEventArgs<object>> AddItem;

        protected void OnAddItem(ItemEventArgs<object> e)
        {
            EventHandler<ItemEventArgs<object>> handler = AddItem;
            if (handler != null) handler(this, e);
        }

        #endregion Public Events

        #region Public Properties

        private bool countNull;

        public bool AutoCorrection
        {
            get { return (bool)GetValue(AutoCorrectionProperty); }
            set { SetValue(AutoCorrectionProperty, value); }
        }

        public CoreDataGrid CurrentDataGrid
        {
            get { return (CoreDataGrid)GetValue(CurrentDataGridProperty); }
            set { SetValue(CurrentDataGridProperty, value); }
        }

        //public string DomainNameSpaces { get; set; }

        public string DomainNameSpaces
        {
            get { return (string)GetValue(DomainNameSpacesProperty); }
            set { SetValue(DomainNameSpacesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DomainNameSpaces.  This enables animation, styling, binding, etc...

        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public int LimitSelectedItems
        {
            get { return (int)GetValue(LimitSelectedItemsProperty); }
            set { SetValue(LimitSelectedItemsProperty, value); }
        }

        public string Models { get; set; }


        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(CoreComboBoxGrid), new UIPropertyMetadata(false));


        public bool MultipleSelected
        {
            get { return (bool)GetValue(MultipleSelectedProperty); }
            set
            {
                SetValue(MultipleSelectedProperty, value);
                SearchDataUseFramework();
            }
        }

        public SearchType SearchTypeWith
        {
            get { return (SearchType)GetValue(SearchTypeWithProperty); }
            set { SetValue(SearchTypeWithProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchTypeWith.  This enables animation, styling, binding, etc...

        //public SearchType SearchTypeWith { get; set; }

        public new object SelectedItem
        {
            get
            {
                object temp = GetValue(SelectedItemProperty);
                if (temp == null)
                {
                    if (listBoxPopUp != null)
                    {
                        if (listBoxPopUp.SelectedItem != null)
                        {
                            return listBoxPopUp.SelectedItem;
                        }
                    }
                }
                else
                {
                    if (temp is TableItem)
                    {
                        return temp;
                    }
                    if (ChooseItem != null)
                    {
                        return ChooseItem;
                    }
                    if (UsingSearchByFramework)
                    {
                        SearchDataUseFramework(temp.ToString());
                    }
                    if (ItemsSource != null)
                    {
                        if (ItemsSource.Cast<object>().Count() == 1)
                        {
                            if (DisableAutoSearchWhenSelectedItem) return temp;
                            ChooseItem = ItemsSource.Cast<object>().FirstOrDefault();
                            return ItemsSource.Cast<object>().FirstOrDefault();
                        }
                    }
                }

                return temp;
            }
            set
            {
                //if (value == null)
                //{
                //    Value = null;
                //    //  return;
                //}



                SetValue(SelectedItemProperty, value);

                //Ditambahkan 24/04/2014 : Dani
                //Kalo ada error di syntax ini hapus aja, mohon maaf
                if (value == null)
                {
                    if (editableTextBox != null)
                        editableTextBox.Clear();
                }
                //=============================


            }
        }

        public List<object> SelectedItems
        {
            get { return (List<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public string SelectedText
        {
            get
            {
                if (editableTextBox == null)
                    return null;
                return editableTextBox.Text;
            }
            set
            {
                if (editableTextBox != null)
                {
                    editableTextBox.Text = value;
                }
                FillSelectedItem();
            }
        }



        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }


        public new object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(CoreComboBoxGrid), new UIPropertyMetadata(null, ChangeSelectedValue));

        private static void ChangeSelectedValue(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as CoreComboBoxGrid;
            if (form != null)
            {
                if (form.editableTextBox != null)
                {
                    form.editableTextBox.Text = (string)e.NewValue;

                }
            }
        }


        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CoreComboBoxGrid), new UIPropertyMetadata("", TextPropertyChanged));

        private static void TextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as CoreComboBoxGrid;
            if (form != null)
            {
                if (form.editableTextBox != null)
                {
                    form.editableTextBox.Text = (string)e.NewValue;

                }
            }
        }


        //public new string Text
        //{
        //    get
        //    {
        //        if (editableTextBox == null)
        //        {
        //            return string.Empty;
        //        }
        //        return editableTextBox.Text;
        //    }
        //    set
        //    {
        //        editableTextBox.Text = value;
        //    }
        //}

        public bool UsingSearchByFramework
        {
            get { return (bool)GetValue(UsingSearchByFrameworkProperty); }
            set { SetValue(UsingSearchByFrameworkProperty, value); }
        }

        public string ValuePath { get; set; }

        public object CurrentSelected { get; set; }

        #region IValidateControl Members

        public bool IsError
        {
            get { return isError; }
            set
            {
                isError = value;
                BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
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
                if (MultipleSelected)
                {
                    return !SelectedItems.Any();
                }
                return string.IsNullOrEmpty(Text);
            }
        }


        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(CoreComboBoxGrid), new UIPropertyMetadata(false, ChangeReadOnly));

        private static void ChangeReadOnly(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBoxGrid;
            if (form != null)
            {
                if (e.NewValue != null)
                {
                    if (form.editableTextBox != null)
                        form.editableTextBox.IsReadOnly = (bool)e.NewValue;

                }
            }

        }

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        public bool SkipAutoFocus { get; set; }

        #endregion

        #region IValueElement Members

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public object Value
        {
            get
            {
                if (SelectedItem == null)
                {
                    if (!string.IsNullOrEmpty(SelectedText))
                    {
                        object firstData = ListSource.FirstOrDefault(
                            n => n.GetType().GetProperty(DisplayMemberPath).GetValue(n, null).Equals(SelectedText));
                        if (firstData != null)
                            return firstData.GetType().GetProperty(ValuePath).GetValue(firstData, null);
                    }
                    return null;
                }
                if (SelectedItem == null && !string.IsNullOrEmpty(SelectedText))
                {
                    return SelectedText;
                }
                if (string.IsNullOrEmpty(ValuePath))
                {
                    return SelectedItem;
                }
                PropertyInfo property = SelectedItem.GetType().GetProperty(ValuePath);
                if (property != null)
                {
                    return property.GetValue(SelectedItem, null);
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    if (countNull) return;
                    countNull = true;
                    ChooseItem = null;
                    FillText(null);
                    SelectedItem = null;
                    //Value = null;
                }
                else
                {
                    countNull = false;
                    SelectedItem = null;
                    SelectedItem = value;
                    ChooseItem = SelectedItem;
                    FillText(SelectedItem);
                    WhenSelectedItem(ChooseItem);
                }
                Manager.Timeout(Dispatcher, () => OnSelectionChanged());
            }
        }

        #endregion

        #endregion Public Properties

        #region Properties

        internal object ChooseItem
        {
            get { return chooseItem2; }
            set
            {
                try
                {
                    chooseItem2 = value;
                    if (ItemsSource != null)
                    {
                        if (Items.Count == 0)
                            Items.Clear();
                        foreach (object item in ItemsSource)
                        {
                            Items.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                }

                base.SelectedItem = chooseItem2;
            }
        }

        protected object CurrentItem { get; set; }

        protected string CurrentText { get; set; }

        #endregion Properties

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            if (editableTextBox != null) editableTextBox.Text = "";
            SelectedItem = null;
            IsError = false;
        }

        public void FocusControl()
        {

            if (editableTextBox != null)
            {
                editableTextBox.SelectAll();
                editableTextBox.Focus();
            }
        }

        //public bool UsingSearchByFramework { get; set; }

        public override void OnApplyTemplate()
        {

            var myTextBox = GetTemplateChild("CustomPART_EditableTextBox") as CoreTextBox;
            var btnClose = GetTemplateChild("PART_CLOSE") as Grid;
            var scroll = GetTemplateChild("DropDownScrollViewer") as CoreListBox;
            var toogleButton = GetTemplateChild("DropDownToggle") as ToggleButton;
            if (btnClose != null)
            {
                buttonClose = btnClose;
                buttonClose.GotFocus += ButtonCloseOnGotFocus;
                buttonClose.MouseLeftButtonUp += ButtonCloseOnMouseLeftButtonUp;
            }
            if (myTextBox != null)
            {

                editableTextBox = myTextBox;
                editableTextBox.IsSkipSentenceCase = true;
                if (DataContext != null)
                {
                    if (DataContext.GetType().GetProperty(this.DisplayMemberPath) != null)
                    {
                        if (DataContext.GetType().GetProperty(this.DisplayMemberPath).CanWrite)
                            BindingOperations.SetBinding(editableTextBox, TextBox.TextProperty, new Binding(DisplayMemberPath)
                            {
                                Mode = BindingMode.TwoWay
                            });
                        else
                            BindingOperations.SetBinding(editableTextBox, TextBox.TextProperty, new Binding(DisplayMemberPath)
                            {
                                Mode = BindingMode.OneWay
                            });
                    }
                }

                editableTextBox.IsReadOnly = ReadOnly;
                editableTextBox.TextChanged += EditableTextBoxTextChanged;
                editableTextBox.PreviewKeyDown += EditableTextBoxPreviewKeyDown;
                editableTextBox.KeyDown += EditableTextBoxOnKeyDown;
                editableTextBox.LostFocus += EditableTextBoxOnLostFocus;
                editableTextBox.GotFocus += EditableTextBoxGotFocus;
                if (ChooseItem != null)
                {
                    FillText(ChooseItem);
                }
            }
            if (toogleButton != null)
            {
                toogleButton.Click += ToogleButtonOnClick;
            }
            if (scroll != null)
            {
                //ApplyBinding(this.SelectedValueBinding, comboBox, Selector.SelectedValueProperty);

                listBoxPopUp = scroll;
                VirtualizingStackPanel.SetIsVirtualizing(listBoxPopUp, true);
                BindingOperations.SetBinding(listBoxPopUp, Selector.SelectedValueProperty, new Binding(ValuePath)
                {
                    Mode = BindingMode.TwoWay
                });
                listBoxPopUp.PreviewKeyDown += ListBoxPopUpKeyDown;
                listBoxPopUp.SelectionChanged += ListBoxPopUpSelectionChanged;
                listBoxPopUp.MouseDoubleClick += ListBoxPopUpMouseDoubleClick;
                listBoxPopUp.MouseLeftButtonUp += ListBoxPopUpMouseLeftButtonUp;
            }
            if (editableTextBox != null)
            {
                if (!string.IsNullOrEmpty(CurrentText))
                {
                    editableTextBox.Text = CurrentText;
                }
                if (CurrentItem != null)
                {
                    SelectedItem = CurrentItem;
                }
            }
            base.OnApplyTemplate();
        }

        private void ButtonCloseOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Value = null;
            pressEnter = false;
            buttonClose.Visibility = Visibility.Collapsed;
            countNull = false;
        }

        private void ButtonCloseOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {

        }

        private void EditableTextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!IsDropDownOpen)
                base.OnLostFocus(routedEventArgs);
        }

        public void OnBeforeValidate(HandleArgs e = null)
        {
            EventHandler<HandleArgs> handler = BeforeValidate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnItemsSourceChanged(EventArgs e)
        {
            EventHandler handler = ItemsSourceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private object afterSelected;
        public new void OnSelectionChanged(SelectionChangedEventArgs e = null)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {

                //Edit 26 Agustus 2014
                if (afterSelected == null)
                    handler(this, e);
                if (afterSelected != null && this.SelectedItem != null)
                {
                    //if (!afterSelected.ToString().Equals(this.SelectedItem.ToString()))
                    //    handler(this, e);
                    if (!afterSelected.Equals(SelectedItem))
                        handler(this, e);
                }
                afterSelected = this.SelectedItem;
            }
        }

        public void OnTextChanged(TextChangedEventArgs e)
        {
            TextChangedEventHandler handler = TextChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Public Methods and Operators

        #region Methods

        private object tempTemplate;
        private Grid buttonClose;

        internal void FillText(object item)
        {
            if (editableTextBox != null)
            {
                editableTextBox.TextChanged -= EditableTextBoxTextChanged;
            }
            if (item == null)
            {
                if (editableTextBox != null)
                {
                    //this.editableTextBox.Clear();
                    //this.editableTextBox.SelectAll();
                }
                return;
            }
            object value;
            if (item is KeyValue)
            {
                value = (item as KeyValue).Key;
            }
            else
            {
                PropertyInfo property = item.GetType().GetProperty(string.IsNullOrEmpty(Models) ? DisplayMemberPath : "Description");
                if (SelectedItem == null) SelectedItem = "";
                value = property != null ? property.GetValue(SelectedItem, null) : SelectedItem.ToString();
            }
            if (value != null)
            {
                if (editableTextBox != null)
                {
                    if (listBoxPopUp != null)
                        listBoxPopUp.SelectedItem = null;
                    editableTextBox.Clear();
                    editableTextBox.Text = value.ToString();
                    Text = (value.ToString());
                    //                    editableTextBox.AppendText(value.ToString());
                    editableTextBox.SelectAll();

                    IsError = false;
                }
            }
            if (editableTextBox != null)
            {
                editableTextBox.TextChanged += EditableTextBoxTextChanged;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Manager.Timeout(Dispatcher, () =>
            {
                if (editableTextBox != null)
                    editableTextBox.Focus();
            });
        }

        private static void ChangeCurrentDataGrid(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBoxGrid;
            if (form != null)
            {
                form.CurrentDataGrid = e.NewValue as CoreDataGrid;
            }
        }

        private static void ChangeSelectedItem(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBoxGrid;
            if (form != null)
            {
                if (e.NewValue != null)
                {
                    object model = e.NewValue;
                    if (form.ItemsSource == null)
                        if (form.UsingSearchByFramework)
                        {
                            form.SearchDataUseFramework();
                        }
                    if (form.ItemsSource != null)
                    {
                        var collection = form.ItemsSource.Cast<object>();
                        if (!string.IsNullOrEmpty(form.Models)
                            && (e.NewValue is string || e.NewValue is byte || e.NewValue is Int32 || e.NewValue is Int16
                                || e.NewValue is Int64 || e.NewValue is double || e.NewValue is float
                                || e.NewValue is decimal))
                        {
                            IEnumerable<ModelItem> modelItems = form.RebindModels();
                            model = modelItems.FirstOrDefault(n => n.Key.Equals(e.NewValue));
                            BindingSelectedItem(e, form, model);
                        }
                        else if ((form.ItemsSource == null || !collection.Any()) &&
                                 !form.DisableAutoSearchWhenSelectedItem)
                        {
                            form.SearchDataUseFramework(e.NewValue.ToString(), () => BindingSelectedItem(e, form, model));
                        }
                        else
                        {
                            if (form.ItemsSource != null)
                            {
                                var firstOrDefault = collection.FirstOrDefault();
                                if (firstOrDefault != null && e.NewValue.GetType() != firstOrDefault.GetType())
                                {
                                    model =
                                        collection.FirstOrDefault(
                                            n =>
                                            n.GetType().GetProperty(form.ValuePath).GetValue(n, null).Equals(e.NewValue));
                                    BindingSelectedItem(e, form, model);
                                }
                            }
                        }
                    }
                }
                else
                {
                    form.ChooseItem = null;
                }
            }
        }

        private static void BindingSelectedItem(DependencyPropertyChangedEventArgs e, CoreComboBoxGrid form, object model)
        {
            if (form.ItemsSource == null)
            {
                return;
            }
            IEnumerable<object> source = form.ItemsSource.Cast<object>();
            object[] enumerable = source as object[] ?? source.ToArray();
            if (enumerable.Count() != 0)
            {
                object firstOrDefault = enumerable.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    if (model != null && firstOrDefault.GetType() != model.GetType())
                    {
                        if (string.IsNullOrEmpty(form.ValuePath))
                        {
                            model = source.FirstOrDefault(n => n.ToString().Equals(e.NewValue.ToString()));
                        }
                        else
                        {
                            if ((e.NewValue is string || e.NewValue is byte || e.NewValue is Int32 ||
                                 e.NewValue is Int16
                                 || e.NewValue is Int64 || e.NewValue is double || e.NewValue is float
                                 || e.NewValue is decimal || e.NewValue is char))
                            {
                                if (form.DisableAutoSearchWhenSelectedItem) return;
                                var repository = BaseDependency.Get<IControlCommonRepository>();
                                if (form.UsingSearchByFramework)
                                {
                                    if (!string.IsNullOrEmpty(form.DomainNameSpaces))
                                    {
                                        model =
                                            repository.GetBaseData(
                                                form.DomainNameSpaces,
                                                form.ValuePath,
                                                e.NewValue.ToString(),
                                                SearchType.Equal,
                                                form).FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    model =
                                        form.ItemsSource.Cast<object>().FirstOrDefault(
                                            n => n.GetType().GetProperty(form.ValuePath).GetValue(n, null) != null &&
                                                 n.GetType().GetProperty(form.ValuePath).GetValue(n, null).ToString().Equals(
                                                     e.NewValue.ToString()));
                                    BindingSelectedItem(e, form, model);
                                }
                            }
                            //  model = source.FirstOrDefault(n => n.GetType().GetProperty(form.ValuePath).GetValue(n, null).ToString().Equals(e.NewValue.ToString()));
                        }
                    }
                }
            }
            form.ChooseItem = model;
            form.FillText(form.ChooseItem);
        }

        private static void ChangeSource(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBoxGrid;
            if (form != null)
            {
                form.ItemsSource = e.NewValue as IEnumerable;

                if (form.ItemsSource != null)
                {
                    form.ListSource = new List<object>();
                    foreach (object model in form.ItemsSource)
                    {
                        form.ListSource.Add(model);
                    }
                }
                form.OnItemsSourceChanged(EventArgs.Empty);
            }
        }

        private static void SelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreComboBoxGrid;
            if (form != null)
            {
                form.SelectedItems = e.NewValue as List<object>;
            }
        }

        private void CallbackSearchData(object sender, EventArgs e)
        {
            timerForSearch.Stop();
            if (UsingSearchByFramework)
            {

                ItemsSource = new List<object>();
                var text = editableTextBox.Text;
                SearchDataUseFramework(text);
            }
            else
                SearchData();
        }

        private void CoreComboBoxGridDropDownClosed(object sender, EventArgs e)
        {
            if (pressEnter)
            {
                editableTextBox.Focus();
                editableTextBox.SelectAll();
            }
        }

        private void CustomComboBoxDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void EditableTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            SearchData();
            if (SelectedItem != null && listBoxPopUp.SelectedItem == null)
            {
                listBoxPopUp.SelectedItem = SelectedItem;
            }
            pressEnter = false;
            if (CurrentDataGrid != null)
            {
                CurrentDataGrid.FreezeRetrun = true;
            }
        }


        private void EditableTextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {

            //Mereference Ke Form Lain sesuai DomainType / DomainNameSpace
            if (keyEventArgs.Key == System.Windows.Input.Key.F12)
            {
                #region Mereference Ke Form Lain sesuai DomainType

                var main = Application.Current.MainWindow as IMainWindow;
                if (main != null)
                {
                    if (DomainType != null)
                    {
                        if (SourceKeyValues.KeyValues != null)
                        {
                            var typeDomain = DomainType;
                            ExecuteControlForm(typeDomain);
                        }
                    }
                    else if (!string.IsNullOrEmpty(DomainNameSpaces))
                    {
                        var domain = HelperManager.GetListSearchFramework(DomainNameSpaces);
                        if (domain != null)
                            ExecuteControlForm(domain);
                    }
                }
                #endregion
            }

            if (keyEventArgs.Key == System.Windows.Input.Key.Return && !SkipAutoFocus)
            {
                var parent = Manager.FindVisualParent<CoreUserControl>(this);
                if (parent != null)
                {
                    bool valid = false;
                    foreach (FrameworkElement findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent))
                    {
                        if (valid && !findVisualChild.Name.Equals("PART_EditableTextBox")
                            && findVisualChild is IValueElement && (findVisualChild as IValueElement).CanFocus && !findVisualChild.Equals(editableTextBox)
                            && !findVisualChild.Equals(listBoxPopUp) && findVisualChild.IsEnabled && (findVisualChild as IValueElement).CanFocus)
                        {
                            var parenCombo = Manager.FindVisualParent<CoreComboBoxGrid>(findVisualChild);
                            if (parenCombo == null)
                            {
                                var parentTabItem = Manager.FindVisualParent<TabItem>(findVisualChild);
                                if (parentTabItem != null)
                                {
                                    parentTabItem.IsSelected = true;
                                }
                                ThreadPool.QueueUserWorkItem(delegate (object state)
                                {
                                    Thread.Sleep(50);
                                    Manager.Timeout(Dispatcher, () =>
                                    {
                                        if (((FrameworkElement)state).IsEnabled)
                                        {
                                            ((FrameworkElement)state).Focus();
                                        }
                                    });
                                }, findVisualChild);

                                break;
                            }
                            if (!parenCombo.Equals(this))
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
                        }
                        if (findVisualChild.Equals(this))
                        {
                            valid = true;
                        }
                    }
                }
            }
            OnKeyDown(keyEventArgs);
        }

        public new event KeyEventHandler KeyDown;

        /// <summary>
        /// Untuk menampilkan form master sesuai dengan Domain Type
        /// </summary>
        /// <param name="typeDomain">parameter Type Domain</param>
        private void ExecuteControlForm(Type typeDomain)
        {
            try
            {
                var keyValue = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key != null && n.Key.ToString().Replace(" ", "") == typeDomain.Name);
                if (keyValue != null)
                {
                    var coreMenuItem = keyValue.Value as CoreMenuItem;
                    if (coreMenuItem != null) coreMenuItem.ExecuteControl();
                }
                else
                {
                    var keyValuesContaintCommon = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key.ToString().Contains(typeDomain.Name) && n.Key.ToString().Contains("Common"));
                    if (keyValuesContaintCommon != null)
                    {
                        if (typeof(IGenericCalling).IsAssignableFrom(keyValuesContaintCommon.Value as Type))
                        {
                            if (keyValuesContaintCommon.Value is Type)
                            {
                                var type = keyValuesContaintCommon.Value as Type;
                                if (type.IsClass)
                                {
                                    var instanace = Activator.CreateInstance(type) as IGenericCalling;
                                    if (instanace != null)
                                    {
                                        try
                                        {
                                            instanace.InitView();
                                        }
                                        catch (Exception e)
                                        {
                                            //Manager.HandleException(e);
                                            Log.ThrowError(e, "300");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Manager.HandleException(e, "Error ExecuteControlForm : Domain Type atau DomainNameSpace!");
                Log.ThrowError(e, "300");
            }
        }

        private void EditableTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!pressEnter)
            {
                IsDropDownOpen = true;
            }
            if (e.Key == System.Windows.Input.Key.Return)
            {
                return;
            }
            Searching = true;

            if (e.Key == System.Windows.Input.Key.Down)
            {
                Searching = false;
                //  listBoxPopUp.SelectedIndex = 0;
                if (listBoxPopUp.Items.Count == 0)
                {
                    if (UsingSearchByFramework)
                    {
                        SearchDataUseFramework();
                    }
                    SearchData();
                }
                listBoxPopUp.Focus();
                pressEnter = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                IsDropDownOpen = false;
                Manager.Timeout(Dispatcher, () => editableTextBox.Focus());
            }
        }

        static int delay = 500;
        System.Threading.Timer timer = null;
        private void DisposeTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
        private void EditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            timerForSearch.Stop();
            DisposeTimer();
            if (editableTextBox != null)
            {
                buttonClose.Visibility = string.IsNullOrEmpty(editableTextBox.Text) ? Visibility.Collapsed : Visibility.Visible;
                if (string.IsNullOrEmpty(editableTextBox.Text))
                    pressEnter = false;
            }

            if (!pressEnter)
            {
                if (!UsingSearchByFramework)
                {
                    OnTextChanged(e);
                    SearchData();
                }
                else
                {
                    SearchDataUseFramework(e);
                    //timer = new Timer(TimerElapsed, e, delay, delay);
                    timerForSearch.Start();
                    //SearchDataUseFramework(e);
                }
                //timerForSearch.Start();
                //  SelectedItem = null;
            }
        }

        private void TimerElapsed(object state)
        {
            SearchDataUseFramework(state);
            DisposeTimer();
        }


        private void FillSelectedItem()
        {
            if (editableTextBox == null)
            {
                return;
            }
            object item = null;
            if (ListSource != null)
            {
                foreach (object model in ListSource)
                {
                    object pengambilanObjekDariSource = !string.IsNullOrEmpty(DisplayMemberPath)
                        ? HelperManager.BindPengambilanObjekDariSource(model,
                            DisplayMemberPath)
                        : model.ToString();
                    if (pengambilanObjekDariSource.ToString().ToLower().Contains(editableTextBox.Text.ToLower()))
                    {
                        if (!MultipleSelected)
                        {
                            listBoxPopUp.Items.Add(model);
                        }
                        else
                        {
                            //var checkbox = HelperManager.BindPengambilanObjekDariSource(model, DisplayMemberPath);
                            //listBoxPopUp.Items.Add(HelperManager.BindPengambilanObjekDariSource(model, DisplayMemberPath));
                            bool isActive = model != null
                                            && SelectedItems.FirstOrDefault(
                                                n => ((KeyValue)n).Value != null && ((KeyValue)n).Value.Equals(model))
                                            != null;
                            var value = new KeyValue
                            {
                                Value = model,
                                Key =
                                    HelperManager.BindPengambilanObjekDariSource(
                                        model,
                                        DisplayMemberPath),
                                IsActive = isActive
                            };
                            value.PropertyChanged += ValueOnPropertyChanged;
                            listBoxPopUp.Items.Add(value);
                        }
                        item = model;
                        // Items.Add(model);
                    }
                }
            }
            listBoxPopUp.SelectedItem = item;
            SelectedItem = item;
        }

        private void ListBoxPopUpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up)
            {
                if (listBoxPopUp.SelectedIndex == 0)
                {
                    editableTextBox.Focus();
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
            }
            else if (e.Key == System.Windows.Input.Key.Return)
            {
                WhenSelectedItem();
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                IsDropDownOpen = false;
                Manager.Timeout(Dispatcher, () => editableTextBox.Focus());
            }
            else
            {
                if (e.Key.ToString().Length == 1)
                {
                    SearchData();
                    editableTextBox.Focus();
                    editableTextBox.ScrollToEnd();
                }
            }
        }

        private void ListBoxPopUpMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WhenSelectedItem(e.OriginalSource);
        }

        private void ListBoxPopUpMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Manager.Timeout(Dispatcher, () =>
            {
                if (!MultipleSelected)
                {
                    WhenSelectedItem(e.OriginalSource);
                }
            });

        }

        private void ListBoxPopUpOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == System.Windows.Input.Key.Space)
            {
                UpdateSelectedItems(listBoxPopUp.SelectedItem);
            }
            else
            {
                CurrentSelected = null;
            }
        }

        private void ListBoxPopUpSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsReadOnly)
            {
                base.Text = null;
            }
            if (listBoxPopUp.SelectedItem == null)
            {
                return;
            }
            //if (textInControl != null)
            //    if (!textInControl.Equals(editableTextBox.Text))
            //SelectedItem = listBoxPopUp.SelectedItem;
            //  selectionChangedEvent = e;
            //base.OnSelectionChanged(e);
            //SelectedItem = listBoxPopUp.SelectedItem;
            eventArgs = e;
            this.SelectedItem = this.listBoxPopUp.SelectedItem;
            this.FillText(this.listBoxPopUp.SelectedItem);

        }


        private IEnumerable<ModelItem> RebindModels()
        {
            return (from model in Models.Split(';') where !string.IsNullOrEmpty(model) select model.Split('*') into arr select new ModelItem(arr[0], arr[1])).ToList();
        }

        private void SearchData()
        {
            var tempSearch = new List<object>();

            if (Items.Count == 0)
                Items.Clear();
            if (listBoxPopUp != null)
                listBoxPopUp.Items.Clear();
            if (!MultipleSelected)
            {
                if (listBoxPopUp != null)
                {
                    listBoxPopUp.DisplayMemberPath = DisplayMemberPath;
                    listBoxPopUp.PreviewKeyDown -= ListBoxPopUpOnKeyDown;
                    if (listBoxPopUp.ItemTemplate != null)
                        tempTemplate = listBoxPopUp.ItemTemplate;
                    listBoxPopUp.ItemTemplate = null;
                }
            }
            else
            {
                if (listBoxPopUp != null)
                {
                    listBoxPopUp.PreviewKeyDown += ListBoxPopUpOnKeyDown;
                    if (tempTemplate != null)
                    {
                        listBoxPopUp.DisplayMemberPath = null;
                        listBoxPopUp.ItemTemplate = tempTemplate as DataTemplate;
                        listBoxPopUp.OnApplyTemplate();
                    }
                }
            }
            if (ListSource != null)
            {
                string keyWord = FromToogle ? "" : editableTextBox.Text.ToLower();
                var filters = ListSource.Where(n =>
                    HelperManager.BindPengambilanObjekDariSource(n, DisplayMemberPath).ToString().ToLower().Contains(keyWord)).Take(30);
                if (!MultipleSelected)
                    if (listBoxPopUp != null)
                        listBoxPopUp.Items.AddRange(filters);

                if (MultipleSelected)
                {
                    foreach (var model in filters)
                    {
                        bool isActive = selectedItems.OfType<KeyValue>().Any(keyValue => keyValue.Value.Equals(model));
                        var value = new KeyValue
                        {
                            Value = model,
                            Key =
                                HelperManager.BindPengambilanObjekDariSource(
                                    model,
                                    DisplayMemberPath),
                            IsActive = isActive
                        };
                        value.PropertyChanged += ValueOnPropertyChanged;
                        listBoxPopUp.Items.Add(value);
                    }
                }


                //foreach (object model in ListSource)
                //{
                //    object pengambilanObjekDariSource = !string.IsNullOrEmpty(DisplayMemberPath)
                //        ? HelperManager.BindPengambilanObjekDariSource(model,
                //            DisplayMemberPath)
                //        : model.ToString();

                //    if (pengambilanObjekDariSource != null && editableTextBox != null)
                //    {
                //        string keyWord = FromToogle ? "" : editableTextBox.Text.ToLower();
                //        if (pengambilanObjekDariSource.ToString().ToLower().Contains(keyWord))
                //        {
                //            if (!MultipleSelected)
                //            {
                //                listBoxPopUp.Items.Add(model);
                //            }
                //            else
                //            {
                //                bool isActive = selectedItems.OfType<KeyValue>().Any(keyValue => keyValue.Value.Equals(model));
                //                var value = new KeyValue
                //                {
                //                    Value = model,
                //                    Key =
                //                        HelperManager.BindPengambilanObjekDariSource(
                //                            model,
                //                            DisplayMemberPath),
                //                    IsActive = isActive
                //                };
                //                value.PropertyChanged += ValueOnPropertyChanged;
                //                listBoxPopUp.Items.Add(value);
                //            }
                //            tempSearch.Add(model);
                //            // Items.Add(model);
                //        }
                //    }
                //}
            }
            FromToogle = false;
        }

        private void Invoke(Action func)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, func);
        }

        private void SearchDataUseFramework(object parameter = null, Action actionComplate = null)
        {
            this.Invoke(new Action(() =>
            {
                if (!UsingSearchByFramework)
                {
                    return;
                }
                string value = "";
                bool isDisplay = false;
                if (parameter is TextChangedEventArgs || parameter == null)
                {
                    if (editableTextBox != null)
                    {
                        value = editableTextBox.Text;
                    }
                    isDisplay = true;
                }
                else if (parameter is string)
                {
                    value = parameter.ToString();
                    //isDisplay = true;
                }
                if (FromToogle)
                    value = "";
                var repository = BaseDependency.Get<IControlCommonRepository>();
                if (!string.IsNullOrEmpty(Models))
                {
                    IEnumerable<ModelItem> modelItems = RebindModels();
                    var filter = modelItems.Where(
                        n => n.Description != null && n.Description.ToString().ToLower().Contains(value.ToLower())).ToList();
                    ItemsSource = filter;
                    if (listBoxPopUp != null)
                    {
                        listBoxPopUp.Items.Clear();
                        listBoxPopUp.Items.AddRange(filter);
                    }
                    if (actionComplate != null)
                        actionComplate.Invoke();
                    
                }
                else if (repository == null)
                {
                    OnTextChanged(null);
                    if (actionComplate != null)
                        actionComplate.Invoke();
                }
                else
                {
                    if (isDisplay)
                    {
                        var filter = repository.GetBaseData(
                            DomainNameSpaces,
                            DisplayMemberPath,
                            value,
                            SearchTypeWith,
                            this).ToList();
                        ItemsSource = filter;
                        if (listBoxPopUp != null)
                        {
                            listBoxPopUp.Items.Clear();
                            listBoxPopUp.Items.AddRange(filter);
                        }
                        if (actionComplate != null)
                            actionComplate.Invoke();
                    }
                    else
                    {
                        IsBusy = true;
                        var state = new object[]
                        {
                        DomainNameSpaces,
                        string.IsNullOrEmpty(ValuePath) ? DisplayMemberPath : ValuePath, value, SearchTypeWith, this
                        };
                        //ThreadPool.QueueUserWorkItem(state =>
                        //{
                        var arr = state as object[];
                        if (arr != null)
                        {
                            var data = repository.GetBaseData(arr[0] as string, arr[1] as string, arr[2], (SearchType)arr[3], arr[4]).ToList();
                            Manager.Timeout(Dispatcher, () =>
                            {
                                ItemsSource = data;
                                if (listBoxPopUp != null)
                                {
                                    listBoxPopUp.Items.Clear();
                                    listBoxPopUp.Items.AddRange(data);
                                }
                                IsBusy = false;
                                if (listBoxPopUp != null)
                                {
                                    listBoxPopUp.Items.Clear();
                                    listBoxPopUp.Items.AddRange(data);
                                }
                                if (actionComplate != null)
                                    actionComplate.Invoke();
                            });
                        }

                        //}, new object[]
                        //{
                        //    DomainNameSpaces,
                        //     string.IsNullOrEmpty(ValuePath) ? DisplayMemberPath : ValuePath, value, SearchTypeWith, this
                        //});

                    }
                }
            }
              ));

        }

        private void ToogleButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            pressEnter = false;

            FromToogle = true;
            if (UsingSearchByFramework)
            {
                SearchDataUseFramework();
            }
            SearchData();
        }

        private void UpdateSelectedItems(object model)
        {
            OnIsChecked(new ItemEventArgs(model));
            if (CurrentSelected != null && CurrentSelected.Equals(model))
            {
                return;
            }
            if (selectedItems.Count == LimitSelectedItems)
            {
                freeze = true;
                var keyValue = selectedItems[0] as KeyValue;
                if (keyValue != null)
                {
                    keyValue.IsActive = false;
                    OnRemoveItem(new ItemEventArgs<object>(((KeyValue)selectedItems[0]).Value));
                    selectedItems.RemoveAt(0);
                }

                freeze = false;
            }
            var value = model as KeyValue;
            if (selectedItems.OfType<KeyValue>().FirstOrDefault(n => value != null && n.Value.Equals(value.Value))
                == null)
            {
                CurrentSelected = model;
                var keyValue = model as KeyValue;
                if (keyValue != null)
                {
                    keyValue.IsActive = true;
                    OnAddItem(new ItemEventArgs<object>(keyValue.Value));
                }

                selectedItems.Add(model);
            }
            else
            {
                CurrentSelected = model;
                var keyValue = model as KeyValue;
                if (keyValue != null)
                {
                    keyValue.IsActive = false;
                }
                var keyValue1 = model as KeyValue;
                KeyValue modelRemove =
                    selectedItems.OfType<KeyValue>().FirstOrDefault(n => keyValue1 != null && n.Value.Equals(keyValue1.Value));
                if (modelRemove != null)
                {
                    OnRemoveItem(new ItemEventArgs<object>((modelRemove).Value));
                    selectedItems.Remove(modelRemove);
                }
            }
            SelectedItems = selectedItems.OfType<KeyValue>().Select(n => n.Value).ToList();
        }

        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (!freeze)
            {
                UpdateSelectedItems(sender);
            }
        }

        //protected object ChooseItem { get; set; }

        private void WhenSelectedItem(object originalSource = null)
        {
            try
            {
                // pressEnter = false;
                IsDropDownOpen = false;
                //if (eventArgs == null)
                //{
                //    return;
                //}
                pressEnter = true;
                if (listBoxPopUp != null && listBoxPopUp.SelectedItem != null)
                {
                    SelectedItem = listBoxPopUp.SelectedItem;
                }
                else
                {
                    if (!Items.CanGroup)
                        Items.Clear();

                    var textBlock = Manager.FindVisualChild<TextBlock>((originalSource as DependencyObject));

                    if (listBoxPopUp != null)
                        foreach (object item in listBoxPopUp.Items)
                        {
                            if (string.IsNullOrEmpty(DisplayMemberPath))
                            {
                                if (textBlock != null && item.ToString().Equals(textBlock.Text))
                                {
                                    SelectedItem = item;
                                    break;
                                }
                            }
                            else
                            {
                                object model = HelperManager.BindPengambilanObjekDariSource(item, DisplayMemberPath);
                                if (model != null)
                                {
                                    if (textBlock != null)
                                    {
                                        if (model.ToString().Equals(textBlock.Text))
                                        {
                                            SelectedItem = item;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                }
                //if (listBoxPopUp != null && listBoxPopUp.SelectedItem != null)
                //{
                //    object data = listBoxPopUp.SelectedItem;

                //    FillText(data);
                //    object selectedItem = null;
                //    if (listBoxPopUp.Items != null)
                //    {
                //        foreach (object item in listBoxPopUp.Items)
                //        {
                //            if (!string.IsNullOrEmpty(ValuePath))
                //            {
                //                if (
                //                    item.GetType()
                //                        .GetProperty(ValuePath)
                //                        .GetValue(item, null)
                //                        .Equals(
                //                            SelectedItem.GetType()
                //                                .GetProperty(ValuePath)
                //                                .GetValue(SelectedItem, null)))
                //                {
                //                    selectedItem = item;
                //                }
                //            }
                //            Items.Add(item);
                //        }
                //    }
                //    SelectedItem = selectedItem;
                //    ChooseItem = SelectedItem;
                //}
                //if (base.ItemsSource != null)
                //{
                //    Items.Clear();
                //    foreach (object obj in ItemsSource)
                //    {
                //        Items.Add(obj);
                //    }
                //}

                //base.SelectedItem = SelectedItem;
                OnSelectionChanged();
                //  base.OnSelectionChanged(e);
                Manager.Timeout(Dispatcher, () =>
                {
                    if (IsFocused)
                        editableTextBox.Focus();
                });
                if (CurrentDataGrid != null)
                {
                    CurrentDataGrid.Refresh();
                }
                if (buttonClose != null)
                    buttonClose.Visibility = string.IsNullOrEmpty(editableTextBox.Text) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception exception)
            {
                //Manager.HandleException(exception, "Seleted Combo Box");
                Log.ThrowError(exception, "300");
            }
        }

        #region EventIsChecked

        public event EventHandler<ItemEventArgs> IsChecked;

        public void OnIsChecked(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = IsChecked;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #endregion Methods

        // Using a DependencyProperty as the backing store for CurrentDataGrid.  This enables animation, styling, binding, etc...

        protected bool FromToogle { get; set; }
        protected internal bool DisableAutoSearchWhenSelectedItem { get; set; }

        public object Temp { get; set; }

        public object ValueTemp { get; set; }

        private Type domainType;

        public Type DomainType
        {
            get { return domainType; }
            set
            {
                domainType = value;
                if (domainType == null) return;
                DomainNameSpaces = value.FullName;
                UsingSearchByFramework = true;
            }
        }

        protected new void OnKeyDown(KeyEventArgs e)
        {
            var handler = KeyDown;
            if (handler != null) handler(this, e);
        }
    }
}
