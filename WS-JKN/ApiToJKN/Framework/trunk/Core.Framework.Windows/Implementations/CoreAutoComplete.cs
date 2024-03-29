﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Model;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    [ContentProperty("Columns")]
    [DefaultProperty("Columns")]
    [TemplatePart(Name = "CustomPART_EditableTextBox", Type = typeof(CoreTextBox))]
    [TemplatePart(Name = "DropDownScrollViewer", Type = typeof(CoreDataGrid))]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    public class CoreAutoComplete : ComboBox, IValueElement, IValidateControl
    {

        public bool DisableSearchFrameworkWhenInitializeIsObject
        {
            get { return (bool)GetValue(DisableSearchFrameworkWhenInitializeIsObjectProperty); }
            set { SetValue(DisableSearchFrameworkWhenInitializeIsObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableSearchFrameworkWhenInitializeIsObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableSearchFrameworkWhenInitializeIsObjectProperty =
            DependencyProperty.Register("DisableSearchFrameworkWhenInitializeIsObject", typeof(bool), typeof(CoreAutoComplete), new PropertyMetadata(false));



        #region Static Fields

        public static readonly DependencyProperty AutoCorrectionProperty = DependencyProperty.Register(
            "AutoCorrection",
            typeof(bool),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(true));

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(ObservableCollection<DataGridColumn>),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(null, ColumnsCallback));

        // Using a DependencyProperty as the backing store for CurrentDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentDataGridProperty =
            DependencyProperty.Register(
                "CurrentDataGrid",
                typeof(CoreDataGrid),
                typeof(CoreAutoComplete),
                new PropertyMetadata(ChangeCurrentDataGrid));

        // Using a DependencyProperty as the backing store for DomainNameSpaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DomainNameSpacesProperty =
            DependencyProperty.Register(
                "DomainNameSpaces",
                typeof(string),
                typeof(CoreAutoComplete),
                new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for FrozenColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrozenColumnProperty = DependencyProperty.Register(
            "FrozenColumn",
            typeof(int),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(0));

        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(CoreAutoComplete),
            new PropertyMetadata(ChangeSource));

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for SearchTypeWith.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTypeWithProperty = DependencyProperty.Register(
            "SearchTypeWith",
            typeof(SearchType),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(SearchType.Contains));

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDefaultItemProperty =
            DependencyProperty.Register(
                "SelectedDefaultItem",
                typeof(object),
                typeof(CoreAutoComplete),
                new PropertyMetadata(null, ChangeSelectedItem));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata("", TextCallback));

        // Using a DependencyProperty as the backing store for UsingSearchByFramework.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsingSearchByFrameworkProperty =
            DependencyProperty.Register(
                "UsingSearchByFramework",
                typeof(bool),
                typeof(CoreAutoComplete),
                new UIPropertyMetadata(false));

        #endregion

        #region Fields

        private readonly DispatcherTimer timerForSearch;
        internal List<object> ListSource;

        internal bool Searching;

        private CoreDataGrid coreDataGrid;

        private CoreTextBox editableTextBox;



        private bool isError;


        private bool isPressKeyDown;

        private bool pressEnter;


        private string textInControl;

        #endregion

        #region Constructors and Destructors


        static CoreAutoComplete()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CoreAutoComplete),
                new FrameworkPropertyMetadata(typeof(CoreAutoComplete)));
        }

        public CoreAutoComplete()
        {
            IsEditable = true;
            DataContextChanged += CustomComboBoxDataContextChanged;
            DropDownClosed += CoreAutoCompleteDropDownClosed;
            IsError = false;
            AutoCorrection = true;
            Columns = new ObservableCollection<DataGridColumn>();
            Loaded += OnLoaded;
            timerForSearch = new DispatcherTimer(
                TimeSpan.FromMilliseconds(500),
                DispatcherPriority.DataBind,
                CallbackSearchData,
                Dispatcher);
            timerForSearch.Stop();

            timerForTextChanged = new DispatcherTimer(
                TimeSpan.FromMilliseconds(500),
                DispatcherPriority.DataBind,
                CallbackTextChanged,
                Dispatcher);
            timerForTextChanged.Stop();
        }

        private void CallbackTextChanged(object sender, EventArgs eventArgs)
        {
            timerForTextChanged.Stop();
            TextChangedEventHandler handler = TextChanged;
            if (handler != null)
            {
                handler(this, currentTextChangedEventArgs);
            }
        }

        #endregion

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

        public event EventHandler ItemsSourceChanged;

        public event SelectionChangedEventHandler SelectedItemChanged;

        public event TextChangedEventHandler TextChanged;

        #endregion

        #region Public Properties

        public bool AutoCorrection
        {
            get { return (bool)GetValue(AutoCorrectionProperty); }
            set { SetValue(AutoCorrectionProperty, value); }
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public CoreDataGrid CurrentDataGrid
        {
            get { return (CoreDataGrid)GetValue(CurrentDataGridProperty); }
            set { SetValue(CurrentDataGridProperty, value); }
        }

        public string DomainNameSpaces
        {
            get { return (string)GetValue(DomainNameSpacesProperty); }
            set { SetValue(DomainNameSpacesProperty, value); }
        }

        public int FrozenColumn
        {
            get { return (int)GetValue(FrozenColumnProperty); }
            set { SetValue(FrozenColumnProperty, value); }
        }

        //public string DomainNameSpaces { get; set; }
        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Models { get; set; }

        public SearchType SearchTypeWith
        {
            get { return (SearchType)GetValue(SearchTypeWithProperty); }
            set { SetValue(SearchTypeWithProperty, value); }
        }

        public new object SelectedItem
        {
            get
            {
                object temp = GetValue(SelectedDefaultItemProperty);
                if (temp == null)
                {
                    if (coreDataGrid != null)
                    {
                        if (coreDataGrid.SelectedItem != null)
                        {
                            return coreDataGrid.SelectedItem;
                        }
                    }
                }
                if (ChooseItem != null)
                {
                    return ChooseItem;
                }
                return temp;
            }
            set
            {
                SetValue(SelectedDefaultItemProperty, null);
                SetValue(SelectedDefaultItemProperty, value);
            }
        }

        private string selectedText;
        public string SelectedText
        {
            get
            {
                if (editableTextBox != null) return editableTextBox.Text;
                return selectedText;
            }
            set
            {
                if (editableTextBox != null)
                {
                    editableTextBox.Text = value;
                }
                selectedText = value;
                FillSelectedItem();
            }
        }

        //public SearchType SearchTypeWith { get; set; }
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool UsingSearchByFramework
        {
            get { return (bool)GetValue(UsingSearchByFrameworkProperty); }
            set { SetValue(UsingSearchByFrameworkProperty, value); }
        }

        public string ValuePath { get; set; }

        #region IValidateControl Members

        public bool IsError
        {
            get { return isError; }
            set
            {
                isError = value;
                BorderBrush = value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                Background = value ? new SolidColorBrush(Colors.Tomato) : new SolidColorBrush(Colors.White);
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
                if (editableTextBox == null)
                    return true;
                return string.IsNullOrEmpty(editableTextBox.Text);
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
                    return null;
                }
                if (string.IsNullOrEmpty(SelectedItem.ToString()))
                    return null;
                if (string.IsNullOrEmpty(ValuePath))
                {
                    return SelectedItem;
                }
                PropertyInfo property = SelectedItem.GetType().GetProperty(ValuePath);
                if (property != null)
                {
                    if (editableTextBox != null)
                    {
                        if (!string.IsNullOrEmpty(editableTextBox.Text))
                            return property.GetValue(SelectedItem, null);
                    }
                    else
                        return property.GetValue(SelectedItem, null);
                }
                SearchDataUseFramework(editableTextBox.Text);
                if (ItemsSource != null)
                {
                    IEnumerable<object> source = ItemsSource.Cast<object>();
                    object[] enumerable = source as object[] ?? source.ToArray();
                    SelectedItem = enumerable.FirstOrDefault();
                }
                if (SelectedItem == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(ValuePath))
                {
                    return SelectedItem;
                }
                property = SelectedItem.GetType().GetProperty(ValuePath);
                if (property != null)
                {
                    return property.GetValue(SelectedItem, null);
                }
                return null;
            }
            set
            {
                if (ReferenceEquals(value, string.Empty))
                    value = null;
                SelectedItem = value;
                ChooseItem = value;
                if (value == null)
                {
                    ChooseItem = null;
                }
                if (value != null && value.ToString() == string.Empty)
                    ChooseItem = null;
            }
        }

        #endregion

        #endregion

        #region Properties

        private object chooseItem;

        protected object ChooseItem
        {
            get { return chooseItem; }
            set
            {
                chooseItem = value;

            }
        }

        #endregion

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            editableTextBox.Text = "";
            SelectedItem = null;
            IsError = false;
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(Dispatcher, () => editableTextBox.Focus()));
        }

        //public new string Text { get { return editableTextBox.Text; } set { editableTextBox.Text = value; } }
        //public bool UsingSearchByFramework { get; set; }
        public override void OnApplyTemplate()
        {
            var myTextBox = GetTemplateChild("CustomPART_EditableTextBox") as CoreTextBox;
            var scroll = GetTemplateChild("DropDownScrollViewer") as CoreDataGrid;
            var toogleButton = GetTemplateChild("DropDownToggle") as ToggleButton;
            if (myTextBox != null)
            {
                editableTextBox = myTextBox;
                editableTextBox.IsSkipSentenceCase = true;
                editableTextBox.TextChanged += EditableTextBoxTextChanged;
                editableTextBox.KeyDown += EditableTextBoxOnKeyDown;
                editableTextBox.PreviewKeyDown += EditableTextBoxPreviewKeyDown;
                //   editableTextBox.LostFocus += EditableTextBoxLostFocus;
                editableTextBox.GotFocus += EditableTextBoxGotFocus;
                if (ChooseItem != null)
                {
                    FillText(ChooseItem);
                }
                else
                {
                    editableTextBox.Text = Text;
                }
            }
            if (toogleButton != null)
            {
                toogleButton.Click += ToogleButtonOnClick;
            }
            if (scroll != null)
            {
                if (coreDataGrid != null)
                    coreDataGrid.Columns.Clear();
                coreDataGrid = scroll;
                coreDataGrid.PreviewKeyDown += ListBoxPopUpKeyDown;

                coreDataGrid.SelectionChanged += ListBoxPopUpSelectionChanged;
                coreDataGrid.MouseDoubleClick += ListBoxPopUpMouseDoubleClick;
                coreDataGrid.MouseLeftButtonUp += ListBoxPopUpMouseLeftButtonUp;
                coreDataGrid.LostFocus += CoreDataGridOnLostFocus;
                coreDataGrid.Columns.Clear();
                var listString = new List<string>();
                coreDataGrid.FrozenColumnCount = FrozenColumn;
                foreach (DataGridColumn dataGridColumn in Columns)
                {
                    if (listString.Any(n => n.Equals(dataGridColumn.Header.ToString())))
                    {
                        continue;
                    }
                    try
                    {
                        if (coreDataGrid.Columns.FirstOrDefault(n => n.Header.Equals(dataGridColumn.Header))
                            == null)
                        {
                            coreDataGrid.Columns.Add(dataGridColumn);
                        }
                        listString.Add(dataGridColumn.Header.ToString());
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                    }
                }
            }
            base.OnApplyTemplate();
            HasLoad = true;
        }

        public bool HasLoad { get; set; }

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

        public void OnSelectedItemChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectedItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnTextChanged(TextChangedEventArgs e)
        {
            timerForTextChanged.Stop();
            timerForTextChanged.Start();
            currentTextChangedEventArgs = e;
        }

        #endregion

        #region Methods

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(CoreAutoComplete),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(CoreAutoComplete),
            new UIPropertyMetadata(""));

        private DispatcherTimer timerForTextChanged;
        private TextChangedEventArgs currentTextChangedEventArgs;

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        internal void FillText(object item)
        {
            if (editableTextBox != null)
            {
                editableTextBox.TextChanged -= EditableTextBoxTextChanged;
            }
            try
            {
                if (item == null)
                {
                    if (editableTextBox != null)
                    {
                        coreDataGrid.SelectedItem = null;

                        //ListSource = null;
                        if (string.IsNullOrEmpty(SelectedText))
                        {
                            SelectedText = "";
                            editableTextBox.Clear();
                            editableTextBox.SelectAll();
                        }
                        //else
                        //{
                        //    SelectedText = "";
                        //    editableTextBox.Clear();
                        //    editableTextBox.SelectAll();
                        //}                        
                    }
                    return;
                }
                PropertyInfo property = null;
                if (string.IsNullOrEmpty(Models))
                {
                    foreach (
                        string display in
                            DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        property = item.GetType().GetProperty(display);
                        break;
                    }
                }
                else
                {
                    property = item.GetType().GetProperty("Description");
                }
                object value = null;
                if (property != null)
                {
                    value = property.GetValue(item, null);
                }
                else
                {
                    try
                    {
                        foreach (string display in DisplayMemberPath.Split(new[] { ',' }))
                        {
                            object data = HelperManager.BindPengambilanObjekDariSource(item, display);
                            if (data != null)
                            {
                                value = data;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        value = item.ToString();
                    }
                }
                if (value != null)
                {
                    if (editableTextBox != null)
                    {
                        editableTextBox.Clear();
                        editableTextBox.AppendText(value.ToString());
                        textInControl = value.ToString();
                        editableTextBox.SelectAll();
                        IsError = false;
                    }
                }
            }
            finally
            {
                if (editableTextBox != null)
                {
                    editableTextBox.TextChanged += EditableTextBoxTextChanged;
                }
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            editableTextBox.Focus();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        private static void ChangeCurrentDataGrid(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreAutoComplete;
            if (form != null)
            {
                form.CurrentDataGrid = e.NewValue as CoreDataGrid;
            }
        }

        private static void ChangeSelectedItem(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreAutoComplete;
            if (form != null)
            {

                if (e.NewValue != null)
                {
                    object model = e.NewValue;
                    if (!string.IsNullOrEmpty(form.Models) && e.NewValue is string)
                    {
                        List<ModelItem> modelItems = form.RebindModels();
                        model = modelItems.FirstOrDefault(n => n.Key.Equals(e.NewValue));
                    }
                    //if (form.ItemsSource == null)
                    PropertyInfo property = e.NewValue.GetType().GetProperty(form.ValuePath);
                    if (property == null)
                    {
                        form.SearchDataUseFramework(e.NewValue.ToString());
                    }
                    else
                    {
                        if (!(e.NewValue is TableItem))
                            form.SearchDataUseFramework(property.GetValue(e.NewValue, null));
                    }
                    if (form.ItemsSource == null && !(e.NewValue is TableItem))
                    {
                        return;
                    }
                    if ((e.NewValue is TableItem))
                    {
                        model = e.NewValue;
                    }
                    else
                    {
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
                                        if (e.NewValue is IComparable)
                                        {
                                            foreach (var mdl in source)
                                            {
                                                var data = HelperManager.BindPengambilanObjekDariSource(mdl,
                                                                                                        form.ValuePath);
                                                if (data != null)
                                                {
                                                    if (data.ToString().Equals(e.NewValue.ToString()))
                                                    {
                                                        model = mdl;
                                                        break;
                                                    }
                                                }
                                            }
                                            //model =
                                            //    source.FirstOrDefault(
                                            //        n =>
                                            //        HelperManager.BindPengambilanObjekDariSource(n, form.ValuePath)
                                            //            .ToString()
                                            //            .Equals(e.NewValue.ToString()));
                                        }
                                        else
                                        {
                                            model =
                                                source.FirstOrDefault(
                                                    n =>
                                                    HelperManager.BindPengambilanObjekDariSource(n, form.ValuePath)
                                                        .ToString()
                                                        .Equals(
                                                            HelperManager.BindPengambilanObjekDariSource(
                                                                e.NewValue,
                                                                form.ValuePath)));
                                        }
                                    }
                                }
                            }
                        }
                        foreach (object o in source)
                        {
                            form.Items.Add(o);
                        }
                    }
                    form.ChooseItem = model;
                    form.Items.Clear();

                    (form as ComboBox).SelectedItem = model;
                    form.FillText(form.ChooseItem);
                }
                else
                {
                    form.ChooseItem = null;
                    if (form.coreDataGrid != null)
                    {
                        //form.coreDataGrid.SelectedItem = null;
                        //form.ListSource = null;
                        //form.SelectedText = "";
                    }
                }
            }
        }

        private static void ChangeSource(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //sample
            var form = sender as CoreAutoComplete;
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

        private static void ColumnsCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        private static void TextCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var form = sender as CoreAutoComplete;
            if (form != null)
            {
                if (form.editableTextBox != null && e.NewValue != null)
                {
                    form.editableTextBox.Text = e.NewValue.ToString();
                }
            }
        }

        private void CallbackSearchData(object sender, EventArgs eventArgs)
        {
            timerForSearch.Stop();
            SearchData();
        }

        private void CoreAutoCompleteDropDownClosed(object sender, EventArgs e)
        {
            if (pressEnter)
            {
                if (!SkipAutoFocus)
                {
                    editableTextBox.Focus();
                    editableTextBox.SelectAll();
                }
            }
        }

        private void CoreDataGridOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (isPressKeyDown)
            {
                return;
            }
            if (coreDataGrid.SelectedItem == null)
            {
                return;
            }
            //if (textInControl != null)
            //    if (!textInControl.Equals(editableTextBox.Text))
            //SelectedItem = listBoxPopUp.SelectedItem;
            //ChooseItem = coreDataGrid.SelectedItem;
            //selectionChangedEvent = e;
            SelectedItem = coreDataGrid.SelectedItem;
            OnSelectedItemChanged(null);
            //SelectedItem = listBoxPopUp.SelectedItem;
            //eventArgs = e;

            FillText(coreDataGrid.SelectedItem);
        }

        private void CustomComboBoxDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void EditableTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            isPressKeyDown = false;
            SearchData();
            if (SelectedItem != null && coreDataGrid.SelectedItem == null)
            {
                coreDataGrid.SelectedItem = SelectedItem;
            }
            pressEnter = false;
            if (CurrentDataGrid != null)
            {
                CurrentDataGrid.FreezeRetrun = true;
            }
        }

        private void EditableTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (isPressKeyDown)
            {
                return;
            }
            Searching = false;
            if (CurrentDataGrid != null)
            {
                CurrentDataGrid.FreezeRetrun = false;
            }
            if (AutoCorrection)
            {
                bool valid = false;
                if (SelectedItem != null)
                {
                    foreach (PropertyInfo propertyInfo in SelectedItem.GetType().GetProperties())
                    {
                        try
                        {
                            if (propertyInfo.GetValue(SelectedItem, null).Equals(editableTextBox.Text))
                            {
                                valid = true;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (!valid)
                {
                    VerifyDataItem();
                }
            }
            if (SelectedItem == null && !isPressKeyDown)
            {
                editableTextBox.Text = "";
            }
            var requiredControl = Manager.FindVisualParent<RequiredGrid>(this);
            if (requiredControl != null)
            {
                if (requiredControl.IsRequired)
                {
                    if (IsRequired)
                    {
                        IsError = IsNull;
                    }
                }
            }
            else
            {
                if (IsRequired)
                {
                    IsError = IsNull;
                }
            }
        }

        private void EditableTextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
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
                            && !findVisualChild.Equals(coreDataGrid))
                        {
                            var parenCombo = Manager.FindVisualParent<CoreComboBox>(findVisualChild);
                            if (parenCombo == null)
                            {
                                FrameworkElement frameworkElement = findVisualChild;
                                if (frameworkElement.IsEnabled)
                                {
                                    frameworkElement.Focus();
                                    break;
                                }
                            }
                            else if (!parenCombo.Equals(this))
                            {
                                FrameworkElement frameworkElement = findVisualChild;
                                if (frameworkElement.IsEnabled)
                                {
                                    frameworkElement.Focus();
                                }
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
            else if (keyEventArgs.Key == System.Windows.Input.Key.Down)
            {
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
                OnTextChanged(null);
                isPressKeyDown = true;
                Searching = false;
                //  listBoxPopUp.SelectedIndex = 0;
                if (coreDataGrid.Items.Count == 0)
                {
                    if (UsingSearchByFramework)
                    {
                        SearchDataUseFramework();
                    }
                    SearchData();
                }
                //if (coreDataGrid.Items.MoveCurrentToFirst())
                //    coreDataGrid.SelectedItem = coreDataGrid.Items.CurrentItem;
                //coreDataGrid.SelectedIndex = 0;

                coreDataGrid.Focus();
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        Manager.Timeout(
                            Dispatcher,
                            () =>
                            {
                                try
                                {
                                    object objectItem = null;
                                    if (coreDataGrid.ItemsSource != null)
                                    {
                                        IEnumerator reader = coreDataGrid.ItemsSource.GetEnumerator();
                                        while (reader.MoveNext())
                                        {
                                            objectItem = reader.Current;
                                        }
                                    }
                                    else
                                    {
                                        if (coreDataGrid.Items.Count != 0)
                                        {
                                            objectItem = coreDataGrid.Items[0];
                                        }
                                    }
                                    if (objectItem != null)
                                    {
                                        var cell =
                                            Manager.FindVisualParent<DataGridCell>(
                                                coreDataGrid.Columns[0].GetCellContent(objectItem));
                                        if (cell != null)
                                        {
                                            if (!cell.IsFocused)
                                            {
                                                cell.Focus();
                                            }
                                            var dataGrid = Manager.FindVisualParent<DataGrid>(cell);
                                            if (dataGrid != null)
                                            {
                                                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                                                {
                                                    if (!cell.IsSelected)
                                                    {
                                                        cell.IsSelected = true;
                                                    }
                                                }
                                                else
                                                {
                                                    var row = Manager.FindVisualParent<DataGridRow>(cell);
                                                    if (row != null && !row.IsSelected)
                                                    {
                                                        row.IsSelected = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    coreDataGrid.SelectedIndex = 0;
                                }
                                catch (Exception exception)
                                {
                                    Log.Error(exception);
                                }
                            });
                    });
                pressEnter = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                IsDropDownOpen = false;
                Manager.Timeout(Dispatcher, () => editableTextBox.Focus());
            }
        }
        private void EditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            IsBusy = true;
            ThreadPool.QueueUserWorkItem(state =>
                                             {
                                                 Manager.Timeout(Dispatcher, () =>
                                                                                 {
                                                                                     try
                                                                                     {
                                                                                         timerForSearch.Stop();
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
                                                                                                 timerForSearch.Start();
                                                                                                 //SearchData();
                                                                                                 
                                                                                             }
                                                                                             //timerForSearch.Stop();
                                                                                             //timerForSearch.Start();
                                                                                             //  SelectedItem = null;
                                                                                         }
                                                                                     }
                                                                                     catch (Exception exception)
                                                                                     {
                                                                                         Log.Error(exception);
                                                                                     }
                                                                                 });
                                             });
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
                    foreach (
                        string display in
                            DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        object pengambilanObjekDariSource = !string.IsNullOrEmpty(display)
                                                                ? HelperManager.BindPengambilanObjekDariSource(model,
                                                                                                               display)
                                                                : model.ToString();
                        if (pengambilanObjekDariSource != null && pengambilanObjekDariSource.ToString()
                                                                      .ToLower()
                                                                      .Contains(editableTextBox.Text.ToLower()))
                        {
                            coreDataGrid.Items.Add(model);
                            item = model;
                            break;
                            // Items.Add(model);
                        }
                    }
                }
            }
            coreDataGrid.SelectedItem = item;
            SelectedItem = item;
        }

        private void ListBoxPopUpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up)
            {
                if (coreDataGrid.SelectedIndex == 0)
                {
                    editableTextBox.Focus();
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                isPressKeyDown = true;
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
            WhenSelectedItem(e.OriginalSource);
        }

        private void ListBoxPopUpSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            FillText(ChooseItem);
            Manager.RegisterFormGrid(this);

        }

        private List<ModelItem> RebindModels()
        {
            var modelItems = new List<ModelItem>();
            foreach (string model in Models.Split(';'))
            {
                if (string.IsNullOrEmpty(model))
                {
                    continue;
                }
                string[] arr = model.Split('*');
                var modelItem = new ModelItem(arr[0], arr[1]);
                modelItems.Add(modelItem);
            }
            return modelItems;
        }

        private void SearchData()
        {
            try
            {
                var tempSearch = new List<object>();
                coreDataGrid.Items.Clear();
                Items.Clear();
                coreDataGrid.DisplayMemberPath = DisplayMemberPath;
                if (ListSource != null)
                {
                    foreach (object model in ListSource.Distinct())
                    {
                        if (string.IsNullOrEmpty(DisplayMemberPath))
                        {
                            if (
                                model.ToString()
                                    .ToLower()
                                    .Contains(editableTextBox.Text.ToLower()))
                            {
                                Manager.Timeout(Dispatcher, () => coreDataGrid.Items.Add(model));
                                tempSearch.Add(model);
                                break;
                                // Items.Add(model);
                            }
                        }
                        else
                            foreach (
                                string display in
                                    DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                try
                                {
                                    object pengambilanObjekDariSource = !string.IsNullOrEmpty(display)
                                                                       ? HelperManager.BindPengambilanObjekDariSource(
                                                                           model, display)
                                                                       : model.ToString();
                                    if (pengambilanObjekDariSource.ToString()
                                            .ToLower()
                                            .Contains(editableTextBox.Text.ToLower()))
                                    {
                                        coreDataGrid.Items.Add(model);
                                        tempSearch.Add(model);
                                        break;
                                        // Items.Add(model);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }

                            }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        private void SearchDataUseFramework(object parameter = null)
        {
            string value = "";
            TextChangedEventArgs e;
            bool isDisplay = false;
            if (parameter is TextChangedEventArgs)
            {
                e = parameter as TextChangedEventArgs;
                if (editableTextBox != null)
                {
                    value = editableTextBox.Text;
                }
                isDisplay = true;
            }
            else if (parameter is string)
            {
                value = parameter.ToString();
            }
            else if (parameter == null)
            {
                value = editableTextBox.Text;
            }
            var repository = BaseDependency.Get<IControlCommonRepository>();
            if (!string.IsNullOrEmpty(Models))
            {
                List<ModelItem> modelItems = RebindModels();
                ItemsSource =
                    modelItems.Where(
                        n => n.Description != null && n.Description.ToString().ToLower().Contains(value.ToLower()));
            }
            else if (repository == null)
            {
                OnTextChanged(null);
            }
            else
            {
                if (isDisplay)
                {
                    var item = new List<object>();
                    foreach (
                        string display in
                            DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        item.AddRange(
                            repository.GetBaseData(DomainNameSpaces, display, value, SearchTypeWith, this));
                    }
                    ItemsSource = item.Distinct();
                }
                else
                {
                    ItemsSource = repository.GetBaseData(
                        DomainNameSpaces,
                        ValuePath,
                        value,
                        SearchTypeWith,
                        this);
                }
            }
        }

        private void ToogleButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (UsingSearchByFramework)
            {
                SearchDataUseFramework();
            }
            SearchData();
        }

        private void VerifyDataItem()
        {
            bool valid = false;
            if (ListSource != null)
            {
                foreach (object item in ListSource)
                {
                    PropertyInfo property;
                    foreach (
                        string display in
                            DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrEmpty(display))
                        {
                            if (Manager.CompareObject(item, SelectedItem))
                            {
                                valid = true;
                                break;
                            }
                        }
                        else
                        {
                            property = item.GetType().GetProperty(display);
                            if (property != null)
                            {
                                object value = property.GetValue(item, null);
                                if (
                                    editableTextBox.Text.Equals(
                                        item.GetType().GetProperty(display).GetValue(item, null)))
                                {
                                    valid = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!valid)
            {
                SelectedItem = null;
            }
        }

        private void WhenSelectedItem(object originalSource = null)
        {
            try
            {
                //if (eventArgs == null) return;
                pressEnter = true;
                isPressKeyDown = false;
                if (coreDataGrid.SelectedItem != null)
                {
                    SelectedItem = coreDataGrid.SelectedItem;
                }
                else
                {
                    Items.Clear();

                    var textBlock = Manager.FindVisualChild<TextBlock>((originalSource as DependencyObject));

                    foreach (object item in coreDataGrid.Items)
                    {
                        bool valid = false;
                        foreach (
                            string display in
                                DisplayMemberPath.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (string.IsNullOrEmpty(display))
                            {
                                if (textBlock != null && item.ToString().Equals(textBlock.Text))
                                {
                                    SelectedItem = item;
                                    valid = true;
                                    break;
                                }
                            }
                            else
                            {
                                object model = HelperManager.BindPengambilanObjekDariSource(item, display);
                                if (model != null)
                                {
                                    if (textBlock != null)
                                    {
                                        if (model.ToString().Equals(textBlock.Text))
                                        {
                                            SelectedItem = item;
                                            valid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (valid)
                        {
                            break;
                        }
                    }
                }
                if (coreDataGrid.SelectedItem != null)
                {
                    object data = coreDataGrid.SelectedItem;

                    FillText(data);
                    object selectedItem = null;
                    if (coreDataGrid.Items != null)
                    {
                        foreach (object item in coreDataGrid.Items)
                        {
                            if (!string.IsNullOrEmpty(ValuePath))
                            {
                                if (item.GetType().GetProperty(ValuePath) != null
                                    && SelectedItem.GetType().GetProperty(ValuePath) != null)
                                {
                                    var itemValue = item.GetType()
                                                        .GetProperty(ValuePath)
                                                        .GetValue(item, null) ?? string.Empty;
                                    if (itemValue.Equals(
                                                SelectedItem.GetType()
                                                    .GetProperty(ValuePath)
                                                    .GetValue(SelectedItem, null)))
                                    {
                                        selectedItem = item;
                                    }
                                }
                            }
                            Items.Add(item);
                        }
                    }
                    SelectedItem = selectedItem;
                    ChooseItem = SelectedItem;
                }

                //  base.OnSelectionChanged(eventArgs);
                IsDropDownOpen = false;
                if (CurrentDataGrid != null)
                {
                    CurrentDataGrid.Refresh();
                }
            }
            catch (Exception exception)
            {
                //Manager.HandleException(exception, "Seleted Combo Box");
                Log.ThrowError(exception, "300");
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for AutoCorrection.  This enables animation, styling, binding, etc...

        //public string ValuePath { get; set; }

        //public double WidthGridAutoComplete { get; set; }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
    }
}