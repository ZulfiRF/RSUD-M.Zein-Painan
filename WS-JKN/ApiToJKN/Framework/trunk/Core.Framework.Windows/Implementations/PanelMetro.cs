namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for Panel.xaml
    /// </summary>
    public class PanelMetro : UserControl
    {
        // Using a DependencyProperty as the backing store for CurrentForm.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty CurrentFormProperty = DependencyProperty.Register(
            "CurrentForm",
            typeof(PanelMetro),
            typeof(PanelMetro),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(PanelMetro),
            new PropertyMetadata("TITLE"));

        #endregion

        #region Fields

        private Dictionary<string, object> dataCollection;

        private object selectedItem;

        #endregion

        #region Constructors and Destructors

        public PanelMetro()
        {
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.PanelMetro.xaml"),
                    UriKind.Relative);
            this.Style = rDictionary["PanelMetroStyle"] as Style;
            this.CurrentForm = this;
        }

        #endregion

        #region Public Events

        public event EventHandler<HeaderDataArgs> AddHeaders;

        #endregion

        #region Public Properties

        public PanelMetro CurrentForm
        {
            get
            {
                return (PanelMetro)this.GetValue(CurrentFormProperty);
            }
            set
            {
                this.SetValue(CurrentFormProperty, value);
            }
        }

        public Dictionary<string, object> DataInForm
        {
            get
            {
                var args = new HeaderDataArgs();
                this.OnAddHeaders(args);
                this.dataCollection = args.Headers;
                if (this.Selecteditem != null)
                {
                    foreach (PropertyInfo propertyInfo in this.Selecteditem.GetType().GetProperties())
                    {
                        try
                        {
                            this.dataCollection.Add(propertyInfo.Name, propertyInfo.GetValue(this.Selecteditem, null));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                foreach (
                    IValueElement element in
                        Manager.FindVisualChildren<FrameworkElement>(this)
                            .Where(element => element is IValueElement)
                            .Cast<IValueElement>())
                {
                    IValueConverter converter = null;
                    var frameworkElement = element as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Tag != null)
                    {
                        converter = Activator.CreateInstance(frameworkElement.Tag as Type) as IValueConverter;
                    }
                    object obj;
                    if (!string.IsNullOrEmpty(element.Key))
                    {
                        if (this.dataCollection.TryGetValue(element.Key, out obj))
                        {
                            if (converter == null)
                            {
                                this.dataCollection[element.Key] = element.Value;
                            }
                            else
                            {
                                this.dataCollection[element.Key] = converter.ConvertBack(
                                    element.Value,
                                    null,
                                    null,
                                    null);
                            }
                        }
                        else
                        {
                            if (converter == null)
                            {
                                if (element.Key.Contains(","))
                                {
                                    int index = 0;
                                    foreach (
                                        string key in
                                            element.Key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        object result;
                                        if (!this.dataCollection.TryGetValue(key, out result))
                                        {
                                            var objects = element.Value as object[];
                                            if (objects != null)
                                            {
                                                this.dataCollection.Add(key, objects[index]);
                                            }
                                        }
                                        else
                                        {
                                            var objects = element.Value as object[];
                                            if (objects != null)
                                            {
                                                this.dataCollection[key] = objects[index];
                                            }
                                        }
                                        index++;
                                    }
                                }
                                else
                                {
                                    this.dataCollection.Add(element.Key, element.Value);
                                }
                            }
                            else
                            {
                                if (element.Key.Contains(","))
                                {
                                    int index = 0;
                                    foreach (
                                        string key in
                                            element.Key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        object result;
                                        if (!this.dataCollection.TryGetValue(key, out result))
                                        {
                                            var objects = element.Value as object[];
                                            if (objects != null)
                                            {
                                                this.dataCollection.Add(
                                                    key,
                                                    converter.ConvertBack(objects[index], null, null, null));
                                            }
                                        }
                                        else
                                        {
                                            var objects = element.Value as object[];
                                            if (objects != null)
                                            {
                                                this.dataCollection[key] = converter.ConvertBack(
                                                    objects[index],
                                                    null,
                                                    null,
                                                    null);
                                            }
                                        }
                                        index++;
                                    }
                                }
                                else
                                {
                                    this.dataCollection.Add(
                                        element.Key,
                                        converter.ConvertBack(element.Value, null, null, null));
                                }
                            }
                        }
                    }
                }
                return this.dataCollection;
            }
        }

        public object Selecteditem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                this.selectedItem = value;
                if (value == null)
                {
                    if (this.dataCollection != null)
                    {
                        this.dataCollection.Clear();
                    }
                }
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnAddHeaders(HeaderDataArgs e)
        {
            EventHandler<HeaderDataArgs> handler = this.AddHeaders;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        //public object Selecteditem { get; set; }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
    }
}