using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Configuration;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Helper.Presenters;
using Core.Framework.Model;
using Core.Framework.Model.Attr;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Controls.Dialogs;
using Core.Framework.Windows.Implementations;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ContextMenu = System.Windows.Controls.ContextMenu;
using GroupBox = System.Windows.Controls.GroupBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;
using Panel = System.Windows.Controls.Panel;
using Size = System.Windows.Size;
using TabControl = System.Windows.Controls.TabControl;
using ToolTip = System.Windows.Controls.ToolTip;

namespace Core.Framework.Windows.Helper
{
    public class Manager
    {
        #region Static Fields

        private static readonly Dictionary<string, Type> ListGenericModule = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> ListGenericView = new Dictionary<string, Type>();

        #endregion

        #region Public Methods and Operators

        public static bool CompareObject(object item, object compareItem)
        {
            try
            {
                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
                {
                    if (
                        propertyInfo.GetValue(item, null)
                            .Equals(compareItem.GetType().GetProperty(propertyInfo.Name).GetValue(compareItem, null)))
                    {
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void Confirmation(string title, string content, Action<bool> complete = null)
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow != null)
            {
                Task<MessageDialogResult> result = metroWindow.ShowMessageAsync(
                    title,
                    content,
                    MessageDialogStyle.AffirmativeAndNegative,
                    complete);
            }
        }

        public static bool Confirmation(string title, string content)
        {
            var window = new ChildWindow();
            window.Title = title;
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var contentPlace = new ContentConfirmation();
            contentPlace.TbTitleMessage.Text = content;
            window.Content = contentPlace;
            contentPlace.BtnYes.Focus();
            contentPlace.NoResult += (sender, args) => window.DialogResult = false;
            contentPlace.YesResult += (sender, args) => window.DialogResult = true;
            bool? resultDialog = window.ShowDialog();
            return resultDialog != null && resultDialog.Value;
            //var metroWindow = Application.Current.MainWindow as MetroWindow;
            //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            //if (metroWindow != null)
            //{

            //    metroWindow.ShowMessageAsync("Hello!", "Welcome to the world of metro!", MessageDialogStyle.AffirmativeAndNegative);

            //    //await metroWindow.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative ? "OK" : "Cancel"));
            //    tcs.TrySetResult(true);
            //}
            //else
            //{
            //    tcs.TrySetResult(false);
            //}
            //return tcs.Task;
        }


        public static string InputDialog(string title, string content)
        {
            var window = new ChildWindow();
            window.Title = title;
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var contentPlace = new ContentInputDialog();
            contentPlace.TbTitleMessage.Text = content;
            window.Content = contentPlace;
            contentPlace.NoResult += (sender, args) => window.DialogResult = false;
            contentPlace.YesResult += (sender, args) => window.DialogResult = true;
            contentPlace.YesResult += (sender, args) => contentPlace.result = contentPlace.TbInputMessage.Text;
            window.ShowDialog();
            return contentPlace.result;
        }

        public static void ContentMessage(string title, string message)
        {
            var messageWindow = BaseDependency.Get<IMessageWindow>();
            if (messageWindow != null)
            {
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                {
                    log.Info(message);
                }
                messageWindow.Info(title, message);
                return;
            }
            Timeout(Application.Current.Dispatcher, () =>
            {
                var window = new ChildWindow();

                window.Owner = Application.Current.MainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                var contentPlace = new ContentMessage();
                contentPlace.TbTitleMessage.Text = message;
                window.Content = contentPlace;
                window.BorderBrush = contentPlace.Background;
                contentPlace.YesResult += (sender, args) => window.DialogResult = true;
                bool? resultDialog = window.ShowDialog();
            });

        }

        public static UIElement CreateForm(TableItem item, Func<string> connectionString = null)
        {
            object[] listHeaderAtribute = item.GetType().GetCustomAttributes(true);

            #region Grid Panel

            var grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Margin = new Thickness(0);

            var dataGrid = new CoreDataGrid();
            dataGrid.AutoGenerateColumns = true;
            dataGrid.EditMode = true;
            dataGrid.IsReadOnly = true;

            foreach (PropertyInfo propertyInfo in
                item.GetType()
                    .GetProperties()
                    .Where(n => n.GetCustomAttributes(true).Any(p => p.GetType() == typeof(GridAttribute)))
                    .OrderBy(n => n.GetCustomAttributes(true).OfType<GridAttribute>().First().Index))
            {
                DataGridBoundColumn column = null;
                FormInputAttribute attribute =
                    propertyInfo.GetCustomAttributes(true).OfType<FormInputAttribute>().First();
                if (attribute.Type == FormType.TextBox)
                {
                    column = new DataGridTextColumn();
                }
                else
                {
                    column = new DataGridCheckBoxColumn();
                    column.IsReadOnly = true;
                }
                column.Header = attribute.Title;
                var binding = new Binding(propertyInfo.Name);
                column.Binding = binding;
                if (attribute.Converter != null)
                {
                    var converter = Activator.CreateInstance(attribute.Converter) as IValueConverter;
                    binding.Converter = converter;
                }
                dataGrid.Columns.Add(column);
            }

            dataGrid.VerticalAlignment = VerticalAlignment.Stretch;
            dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            dataGrid.Margin = new Thickness(0);
            grid.Children.Add(dataGrid);

            #endregion Grid Panel

            #region Form Panel

            var panelForm = new PanelMetro();
            panelForm.Title = listHeaderAtribute.Any(n => n.GetType() == typeof(DisplayFormAttribute))
                                  ? ((DisplayFormAttribute)
                                     listHeaderAtribute.First(n => n.GetType() == typeof(DisplayFormAttribute)))
                                        .Title
                                  : "Form Panel";
            var wrapPanelForm = new WrapPanel();
            grid.SizeChanged += (sender, args) =>
                                    {
                                        foreach (FrameworkElement element in
                                            wrapPanelForm.Children.OfType<FrameworkElement>())
                                        {
                                            element.Width = args.NewSize.Width / 2 - 8;
                                        }
                                    };

            foreach (PropertyInfo propertyInfo in
                item.GetType()
                    .GetProperties()
                    .Where(n => n.GetCustomAttributes(true).Any(p => p.GetType() == typeof(FormInputAttribute)))
                    .OrderBy(n => n.GetCustomAttributes(true).OfType<FormInputAttribute>().First().Index))
            {
                FormInputAttribute attribute =
                    propertyInfo.GetCustomAttributes(true).OfType<FormInputAttribute>().First();
                FieldAttribute fieldAttribute = propertyInfo.GetCustomAttributes(true).OfType<FieldAttribute>().First();
                if (attribute.Type == FormType.TextBox)
                {
                    var displayTextbox = new DisplayWithTextBox();
                    displayTextbox.Margin = new Thickness(4);
                    displayTextbox.Width = wrapPanelForm.Width / 2;
                    displayTextbox.WitdhDisplayText = new GridLength(150, GridUnitType.Pixel);
                    displayTextbox.DisplayText = attribute.Title;
                    displayTextbox.Name = propertyInfo.Name;
                    displayTextbox.Tag = attribute.Converter;
                    if (fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull)
                    {
                        displayTextbox.IsRequired = true;
                    }

                    wrapPanelForm.Children.Add(displayTextbox);
                }
                else if (attribute.Type == FormType.Checkbox)
                {
                    var displayTextbox = new DisplayWithCheckBox();
                    displayTextbox.Margin = new Thickness(4);
                    displayTextbox.Width = wrapPanelForm.Width / 2;
                    displayTextbox.WitdhDisplayText = new GridLength(150, GridUnitType.Pixel);
                    displayTextbox.DisplayText = attribute.Title;
                    displayTextbox.Name = propertyInfo.Name;
                    displayTextbox.Tag = attribute.Converter;
                    if (fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull)
                    {
                        displayTextbox.IsRequired = true;
                    }
                    wrapPanelForm.Children.Add(displayTextbox);
                }
            }

            panelForm.Content = wrapPanelForm;
            grid.Children.Add(panelForm);

            #endregion Form Panel

            #region Grid panel

            dataGrid.SelectionChanged += (sender, args) =>
                                             {
                                                 if (dataGrid.SelectedItem == null)
                                                 {
                                                     return;
                                                 }
                                                 panelForm.Selecteditem = dataGrid.SelectedItem;
                                                 foreach (FrameworkElement result in
                                                     wrapPanelForm.Children.OfType<FrameworkElement>())
                                                 {
                                                     if (result is DisplayWithTextBox)
                                                     {
                                                         var displayTextBoxTemp = result as DisplayWithTextBox;
                                                         displayTextBoxTemp.Text =
                                                             dataGrid.SelectedItem.GetType()
                                                                 .GetProperty(
                                                                     displayTextBoxTemp.Name.ToString(
                                                                         CultureInfo.InvariantCulture))
                                                                 .GetValue(dataGrid.SelectedItem, null)
                                                                 .ToString();
                                                     }
                                                     else if (result is DisplayWithCheckBox)
                                                     {
                                                         var displayTextBoxTemp = result as DisplayWithCheckBox;
                                                         if (displayTextBoxTemp.Tag is Type)
                                                         {
                                                             var converter =
                                                                 Activator.CreateInstance(displayTextBoxTemp.Tag as Type)
                                                                 as IValueConverter;
                                                             if (converter != null)
                                                             {
                                                                 displayTextBoxTemp.IsChecked =
                                                                     converter.Convert(
                                                                         dataGrid.SelectedItem.GetType()
                                                                             .GetProperty(
                                                                                 displayTextBoxTemp.Name.ToString(
                                                                                     CultureInfo.InvariantCulture))
                                                                             .GetValue(dataGrid.SelectedItem, null),
                                                                         null,
                                                                         null,
                                                                         CultureInfo.InvariantCulture) as bool?;
                                                             }
                                                         }
                                                     }
                                                 }
                                             };

            #endregion Grid panel

            #region Panel Button

            var panelButtonTemplate = new Grid();
            var stackPanelButtonTemplate = new StackPanel();
            stackPanelButtonTemplate.HorizontalAlignment = HorizontalAlignment.Right;
            stackPanelButtonTemplate.Orientation = Orientation.Horizontal;
            panelButtonTemplate.Margin = new Thickness(0);

            var buttonSave = new CoreButton();
            buttonSave.Content = "Save";
            buttonSave.Height = 25;
            buttonSave.Width = 100;
            buttonSave.FormGrid = panelForm;
            buttonSave.Click += (sender, args) =>
                                    {
                                        string strConnectionString = "";
                                        if (connectionString != null)
                                        {
                                            strConnectionString = connectionString.Invoke();
                                        }
                                        using (var manager = new ContextManager(strConnectionString))
                                        {
                                            TableItem data = null;
                                            if (panelForm.Selecteditem is TableItem)
                                            {
                                                data = panelForm.Selecteditem as TableItem;
                                                data.InitializeManager(manager);
                                                data.OnInit(panelForm.DataInForm);
                                                data.IsNew = false;
                                            }
                                            else
                                            {
                                                data = Activator.CreateInstance(item.GetType()) as TableItem;
                                                if (data != null)
                                                {
                                                    data.OnInit(panelForm.DataInForm);
                                                    manager.InsertObject(data);
                                                }
                                            }

                                            manager.Commit();
                                        }
                                    };
            stackPanelButtonTemplate.Children.Add(buttonSave);

            var buttonDelete = new CoreButton();
            buttonDelete.Content = "Delete";
            buttonDelete.Height = 25;
            buttonDelete.Width = 100;
            stackPanelButtonTemplate.Children.Add(buttonDelete);

            var buttonClose = new CoreButton();
            buttonClose.Content = "Close";
            buttonClose.Height = 25;
            buttonClose.Width = 100;
            buttonClose.IsClose = true;
            stackPanelButtonTemplate.Children.Add(buttonClose);

            panelButtonTemplate.Children.Add(stackPanelButtonTemplate);

            grid.Children.Add(panelButtonTemplate);

            wrapPanelForm.SizeChanged += (sender, args) =>
                                             {
                                                 panelButtonTemplate.Height = 28;
                                                 panelButtonTemplate.VerticalAlignment = VerticalAlignment.Bottom;
                                                 panelButtonTemplate.HorizontalAlignment = HorizontalAlignment.Stretch;
                                                 panelButtonTemplate.Margin = new Thickness(0, 0, 0, 0);
                                                 dataGrid.Margin = new Thickness(2, args.NewSize.Height + 40, 2,
                                                                                 2 + panelButtonTemplate.Height);
                                             };

            #endregion Panel Button

            ThreadPool.QueueUserWorkItem(CallBack, new object[] { grid, dataGrid, item, connectionString });
            return grid;
        }
        private static readonly Dictionary<string, MethodInfo> DictionaryComonExecute = new Dictionary<string, MethodInfo>();
        private static List<KeyValue> DictionaryLoadExecute = new List<KeyValue>();
        public static void ExecuteGenericModule(string controlNameSpace, params object[] parameters)
        {
            try
            {
                Type type;
                if (!string.IsNullOrEmpty(controlNameSpace))
                {
                    string[] arr = controlNameSpace.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    controlNameSpace = arr[0];
                    if (ListGenericModule.TryGetValue(controlNameSpace, out type))
                    {
                        if (type != null && typeof(IGenericCalling).IsAssignableFrom(type))
                        {
                            if (!type.IsClass)
                            {
                                return;
                            }
                            var instanace = Activator.CreateInstance(type) as IGenericCalling;


                            if (instanace != null)
                            {
                                if (arr.Length == 1)
                                {
                                    instanace.InitView();
                                }
                                else
                                {
                                    MethodInfo method = instanace.GetType().GetMethod(arr[1]);
                                    if (method != null)
                                    {
                                        method.Invoke(instanace, parameters);
                                    }
                                }
                            }
                        }
                    }
                    else if (HelperManager.GetListAttachPresenter().TryGetValue(controlNameSpace, out type))
                    {
                        if (type != null && typeof(IBasePresenter).IsAssignableFrom(type))
                        {
                            if (!type.IsClass)
                            {
                                return;
                            }
                            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                            var instanace = Activator.CreateInstance(type) as IBasePresenter;
                            if (instanace != null)
                            {
                                if (arr.Length == 1)
                                {
                                    instanace.Initialize(parameters);
                                }
                                else
                                {
                                    MethodInfo method = instanace.GetType().GetMethod(arr[1]);
                                    if (method != null)
                                    {
                                        method.Invoke(instanace, parameters);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                HandleException(exception, "Execute Generic Module : " + controlNameSpace);
            }
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    var childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }
        //private static readonly Dictionary<DependencyObject, List<DependencyObject>> DictionaryChild = new Dictionary<DependencyObject, List<DependencyObject>>();
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            //var tempDict = new Dictionary<DependencyObject, DependencyObject>();
            //DependencyObject tempObj;
            if (depObj != null)
            {
                //List<DependencyObject> result;
                //if (DictionaryChild.TryGetValue(depObj, out result))
                //{
                //    foreach (var dependencyObject in result)
                //    {
                //        yield return dependencyObject as T;
                //    }
                //}
                //else
                //{
                //result = new List<DependencyObject>();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is TabControl)
                    {
                        foreach (T item in (child as TabControl).Items.OfType<T>())
                        {
                            //if (!tempDict.TryGetValue(item, out tempObj))
                            //{
                            //    tempDict.Add(item, item);
                            //    result.Add(item);
                            yield return item;
                            if (item is TabItem)
                            {
                                if ((item as TabItem).Content is DependencyObject)
                                {
                                    foreach (
                                        T childOfChild in
                                            FindVisualChildren<T>((item as TabItem).Content as DependencyObject)
                                        )
                                    {
                                        //if (!tempDict.TryGetValue(childOfChild, out tempObj))
                                        //{
                                        //    tempDict.Add(childOfChild, childOfChild);
                                        //    result.Add(childOfChild);
                                        yield return childOfChild;
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                foreach (T childOfChild in FindVisualChildren<T>(item))
                                {
                                    //if (!tempDict.TryGetValue(childOfChild, out tempObj))
                                    //{
                                    //    tempDict.Add(childOfChild, childOfChild);
                                    //    result.Add(childOfChild);
                                    yield return childOfChild;
                                    //}
                                }
                            }
                        }
                        //}
                    }
                    if (child != null && child is T)
                    {
                        //if (!tempDict.TryGetValue(child, out tempObj))
                        //{
                        //    tempDict.Add(child, child);
                        //    result.Add(child);
                        yield return (T)child;
                        //}
                    }
                    //'caun
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        //if (!tempDict.TryGetValue(childOfChild, out tempObj))
                        //{
                        //    tempDict.Add(childOfChild, childOfChild);
                        //    result.Add(childOfChild);
                        yield return childOfChild;
                        //foreach (var findVisualChild in FindVisualChildren<T>(childOfChild))
                        //{
                        //    if (!tempDict.TryGetValue(findVisualChild, out tempObj))
                        //    {
                        //        tempDict.Add(findVisualChild, findVisualChild);
                        //        result.Add(findVisualChild);
                        //        yield return findVisualChild;
                        //    }
                        //}

                        // }
                    }
                }
                //  DictionaryChild.Add(depObj, result);
                //}
            }
        }

        public static T FindVisualParent<T>(FrameworkElement element) where T : FrameworkElement
        {
            FrameworkElement parent = element;
            while (parent != null)
            {
                FrameworkElement parentTemp = parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement; //parent = parent.Parent as FrameworkElement;
                if (parent == null)
                {
                    parent = parentTemp.Parent as FrameworkElement;
                }
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

            }
            return null;
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
            }
            return null;
        }

        public static T FindVisualParentContract<T>(FrameworkElement element) where T : class
        {
            FrameworkElement parent = element;
            while (parent != null)
            {
                if (parent is TabItem)
                {
                    PropertyInfo property = parent.GetType()
                        .GetProperty(
                            "TabControlParent",
                            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public
                            | BindingFlags.CreateInstance | BindingFlags.NonPublic);
                    if (property != null)
                    {
                        parent = property.GetValue(parent, null) as FrameworkElement;
                    }
                }
                else
                {
                    if (parent.Parent == null)
                    {
                        var parentTemp = VisualTreeHelper.GetParent(parent);
                        if (parent.Tag is FrameworkElement)
                        {
                            parent = parent.Tag as FrameworkElement;
                        }
                        else if (parentTemp != null)
                        {
                            parent = parentTemp as FrameworkElement;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        parent = parent.Parent as FrameworkElement;
                    }
                }

                var correctlyTyped = parent as T;

                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
            }
            return null;
        }

        public static Dictionary<string, Type> GetGenericModule()
        {
            return ListGenericModule;
        }

        public static IEnumerable<Type> GetGenericView(Type model)
        {
            foreach (var type in ListGenericView)
            {
                bool valid = false;
                try
                {
                    ModelAttribute firstModel =
                        Activator.CreateInstance(type.Value)
                            .GetType()
                            .GetCustomAttributes(true)
                            .OfType<ModelAttribute>()
                            .FirstOrDefault(n => n.Type == model);
                    if (firstModel != null)
                    {
                        valid = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                }
                if (valid)
                {
                    yield return type.Value;
                }
            }
        }

        public static Size GetSize(FrameworkElement contentControl)
        {
            var size = new Size();
            size.Width = double.IsNaN(contentControl.Width) ? contentControl.ActualWidth : contentControl.Width;
            size.Height = double.IsNaN(contentControl.Height) ? contentControl.ActualHeight : contentControl.Height;
            return size;
        }

        public static void ThrowException(Exception exception, string module = "")
        {
            Log.Error(exception.GetBaseException());
            var messageWindow = BaseDependency.Get<IMessageWindow>();
            if (messageWindow != null)
            {
                if (string.IsNullOrEmpty(module))
                {
                    module = "Error";
                }
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                {
                    log.Info(exception.GetBaseException().ToString());
                }
                messageWindow.Info(module, exception.Message);
                return;
            }
            Timeout(
                Application.Current.MainWindow.Dispatcher,
                () =>
                {
                    var window = new ChildWindow();

                    window.Title = "E r r o r ";
                    if (!string.IsNullOrEmpty(module))
                    {
                        window.Title += " : " + module;
                    }
                    window.Owner = Application.Current.MainWindow;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    var contentPlace = new ContentError();
                    var log = BaseDependency.Get<ILogRepository>();
                    if (log != null)
                    {
                        log.Info(exception.GetBaseException().ToString());
                    }
                    contentPlace.TbTitleMessage.Text = exception.Message;
                    window.Content = contentPlace;
                    window.BorderBrush = contentPlace.Background;
                    contentPlace.YesResult += (sender, args) => window.DialogResult = true;
                    bool? resultDialog = window.ShowDialog();
                });
        }        

        public static void HandleException(Exception exception, string module = "")
        {
            Log.Error(exception.GetBaseException());
            var messageWindow = BaseDependency.Get<IMessageWindow>();
            if (messageWindow != null)
            {
                if (string.IsNullOrEmpty(module))
                {
                    module = "Error";
                }
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                {
                    log.Info(exception.GetBaseException().ToString());
                }
                messageWindow.Info(module, string.IsNullOrEmpty(exception.Message) ? "Terjadi Kesalahan" : exception.Message);
                //messageWindow.Info("200", GetDocumentError("200"));
                return;
            }

            Timeout(
                Application.Current.MainWindow.Dispatcher,
                () =>
                {
                    var window = new ChildWindow();

                    window.Title = "E r r o r ";
                    if (!string.IsNullOrEmpty(module))
                    {
                        window.Title += " : " + module;
                    }
                    window.Owner = Application.Current.MainWindow;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    var contentPlace = new ContentError();
                    var log = BaseDependency.Get<ILogRepository>();
                    if (log != null)
                    {
                        log.Info(exception.GetBaseException().ToString());
                    }
                    contentPlace.TbTitleMessage.Text = "Terjadi Kesalahan";
                    window.Content = contentPlace;
                    window.BorderBrush = contentPlace.Background;
                    contentPlace.YesResult += (sender, args) => window.DialogResult = true;
                    bool? resultDialog = window.ShowDialog();
                });
        }

        public static bool Info(string title, string content)
        {
            var messageWindow = BaseDependency.Get<IMessageWindow>();
            if (messageWindow != null)
            {
                messageWindow.Info(title, content);
                return true;
            }
            var window = new ChildWindow();
            window.Title = title;
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var contentPlace = new ContentMessage();
            contentPlace.TbTitleMessage.Text = content;
            window.Content = contentPlace;
            window.BorderBrush = contentPlace.Background;
            contentPlace.YesResult += (sender, args) => window.DialogResult = true;
            bool? resultDialog = window.ShowDialog();
            return resultDialog != null && resultDialog.Value;
        }



        public static void SyncrodGenericModuleToContext(ContextMenu context)
        {
            try
            {
                foreach (var type in ListGenericModule)
                {
                    if (type.Value.IsInterface)
                    {
                        continue;
                    }

                    object instance = Activator.CreateInstance(type.Value);
                    IEnumerable<MenuAttribute> menuAttributes =
                        instance.GetType().GetCustomAttributes(true).OfType<MenuAttribute>();
                    foreach (MenuAttribute menuAttribute in menuAttributes.Where(n => n.ShowInRightClickMenu))
                    {
                        var menu = new CoreMenuItem();
                        menu.Header =
                            menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[
                                menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Length - 1].ConvertToStatementText();
                        CreateMenuGeneric(
                            menu,
                            menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
                                .Length,
                            type.Key,
                            menuAttribute, true);
                        if (string.IsNullOrEmpty(menuAttribute.Module))
                        {
                            context.Items.Add(menu);
                        }
                        else
                        {
                            var validMenu = false;
                            var arrModule = menuAttribute.Module.Split(new[] { ',' });
                            foreach (var module in arrModule)
                            {
                                var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                if (repositoryLocalSotrage != null)
                                {
                                    foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                    {
                                        if (s.Equals(module))
                                        {
                                            context.Items.Add(menu);
                                            validMenu = true;
                                            break;
                                        }
                                    }
                                }
                                if (validMenu) break;
                            }
                        }
                    }
                }

                foreach (var type in HelperManager.GetListAttachPresenter())
                {
                    if (type.Value.IsInterface)
                    {
                        continue;
                    }

                    try
                    {
                        ConstructorInfo constructor = type.Value.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                            object instance = Activator.CreateInstance(type.Value);
                            IEnumerable<MenuAttribute> menuAttributes =
                                instance.GetType().GetCustomAttributes(true).OfType<MenuAttribute>();
                            foreach (MenuAttribute menuAttribute in menuAttributes.Where(n => n.ShowInRightClickMenu))
                            {
                                var menu = new CoreMenuItem();
                                menu.Header =
                                    menuAttribute.MenuLocation.Split(
                                        new[] { '\\' },
                                        StringSplitOptions.RemoveEmptyEntries)[
                                            menuAttribute.MenuLocation.Split(
                                                new[] { '\\' },
                                                StringSplitOptions.RemoveEmptyEntries).Length - 1]
                                        .ConvertToStatementText();
                                CreateMenuGeneric(
                                    menu,
                                    menuAttribute.MenuLocation.Split(
                                        new[] { '\\' },
                                        StringSplitOptions.RemoveEmptyEntries).Length,
                                    type.Key,
                                    menuAttribute, true);
                                context.Items.Add(menu);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }
                foreach (CoreMenuItem result in context.Items.OfType<CoreMenuItem>())
                {
                    SortByName(result);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception, "Binding Menu");
            }
        }

        public static void BindingKeyValueSource(CoreMenu menu)
        {
            var source = menu.GetChildObjects().OfType<CoreMenuItem>().Where(n => n.Items.Count == 0 && !(n.Header is TextBlock)).Select(n => new KeyValue()
            {
                Key = n.Header.ToString().Replace("  ", " ").Trim(),
                Value = n,
            });

            //foreach (var keyValue in source)
            //{
            //    Debug.Print(keyValue.Key.ToString());
            //    Debug.Print(keyValue.Value.ToString());
            //}
        }

        public static void SyncrodGenericModuleToMenu(CoreMenu menu)
        {
            try
            {
                InputGeastureHasAdd = new List<string>();
                foreach (var type in ListGenericModule)
                {
                    if (type.Value.IsInterface)
                    {
                        continue;
                    }
                    object instance = Activator.CreateInstance(type.Value);
                    IEnumerable<MenuAttribute> menuAttributes =
                        instance.GetType().GetCustomAttributes(true).OfType<MenuAttribute>();
                    foreach (MenuAttribute menuAttribute in menuAttributes)
                    {
                        if (string.IsNullOrEmpty(menuAttribute.Module)) BindMenu(menu, menuAttribute, type);
                        else
                        {
                            var validMenu = false;
                            var arrModule = menuAttribute.Module.Split(new[] { ',' });
                            foreach (var module in arrModule)
                            {
                                var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                if (repositoryLocalSotrage != null)
                                {
                                    foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                    {
                                        if (s.Equals(module))
                                        {
                                            BindMenu(menu, menuAttribute, type);
                                            validMenu = true;
                                            break;
                                        }
                                    }
                                }
                                if (validMenu) break;
                            }
                        }

                    }
                }

                foreach (var type in HelperManager.GetListAttachPresenter())
                {
                    if (type.Value.IsInterface)
                    {
                        continue;
                    }

                    try
                    {
                        ConstructorInfo constructor = type.Value.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                            object instance = Activator.CreateInstance(type.Value);
                            IEnumerable<MenuAttribute> menuAttributes =
                                instance.GetType().GetCustomAttributes(true).OfType<MenuAttribute>();
                            foreach (MenuAttribute menuAttribute in menuAttributes)
                            {
                                if (string.IsNullOrEmpty(menuAttribute.Module)) BindMenu(menu, menuAttribute, type);
                                else
                                {
                                    var validMenu = false;
                                    var arrModule = menuAttribute.Module.Split(new[] { ',' });
                                    foreach (var module in arrModule)
                                    {
                                        var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                        if (repositoryLocalSotrage != null)
                                        {
                                            foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                            {
                                                if (s.Equals(module))
                                                {
                                                    BindMenu(menu, menuAttribute, type);
                                                    validMenu = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (validMenu) break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }
                foreach (CoreMenuItem result in menu.Items.OfType<CoreMenuItem>())
                {
                    SortByName(result);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception, "Binding Menu");
            }
        }

        public static void SyncronGenericModule()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (location != null)
            {
                foreach (string directory in Directory.GetDirectories(location))
                {
                    SyncronDetail(directory);
                }
                SyncronDetail(location);


            }
        }

        public static void Timeout(Dispatcher dispatcher, Func<bool> func)
        {
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => func.Invoke()));
        }

        public static void Timeout(Dispatcher dispatcher, Action func, DispatcherPriority dispatcherPriority = DispatcherPriority.ContextIdle)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(dispatcherPriority, func);
            }
            else
                func.Invoke();

        }

        #endregion

        #region Methods

        private static void BindMenu(CoreMenu menu, MenuAttribute menuAttribute, KeyValuePair<string, Type> type, bool isRightClick = false)
        {
            string[] arrPath = menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            bool repeat = true;
            int index = 0;
            ItemCollection items = menu.Items;
            while (repeat)
            {
                if (index == arrPath.Length)
                {
                    break;
                }
                CoreMenuItem controlMenu =
                    items.OfType<CoreMenuItem>()
                        .FirstOrDefault(n => n.Header.ToString().Equals(arrPath[index].ConvertToStatementText()));
                if (controlMenu == null)
                {
                    var objMenu = new CoreMenuItem();
                    //objMenu.Background = menu.Background;
                    //objMenu.Foreground = new SolidColorBrush(Colors.White);

                    if (string.IsNullOrEmpty(menuAttribute.Module))
                    {

                        CreateMenuGeneric(objMenu, index, type.Key, menuAttribute, isRightClick);
                        objMenu.Header = arrPath[index].ConvertToStatementText();
                        objMenu.Tag = menuAttribute;

                        items.Add(objMenu);
                    }
                    else
                    {
                        var validMenu = false;
                        var arrModule = menuAttribute.Module.Split(new[] { ',' });
                        foreach (var module in arrModule)
                        {
                            var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                            if (repositoryLocalSotrage != null)
                            {
                                foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                {
                                    if (s.Equals(module))
                                    {
                                        CreateMenuGeneric(objMenu, index, type.Key, menuAttribute, isRightClick);
                                        objMenu.Header = arrPath[index].ConvertToStatementText();
                                        items.Add(objMenu);
                                        validMenu = true;
                                        break;
                                    }
                                }
                            }
                            if (validMenu) break;
                        }
                    }

                    if (menuAttribute.DomainModel != null)
                    {
                        if (menuAttribute.DomainModel is Type[])
                        {
                            foreach (var domain in menuAttribute.DomainModel as Type[])
                            {
                                GetDomain(domain, objMenu);
                            }
                        }
                        else
                        {
                            var domain = menuAttribute.DomainModel as Type;
                            if (domain != null) GetDomain(domain, objMenu);
                            else
                            {
                                var domains = menuAttribute.DomainModel as IEnumerable;
                                if (domains != null)
                                    foreach (Type model in domains)
                                    {
                                        GetDomain(model, objMenu);
                                    }
                            }
                        }
                    }

                    repeat = false;
                }
                else
                {
                    index++;
                    items = controlMenu.Items;
                }
            }
        }

        private static void CallBack(object state)
        {
            var arrObject = state as object[];
            if (arrObject != null)
            {
                var grid = arrObject[0] as Grid;
                var dataGrid = arrObject[1] as CoreDataGrid;
                var item = arrObject[2] as TableItem;
                var connectionString = arrObject[3] as Func<string>;
                if (grid != null)
                {
                    Timeout(
                        grid.Dispatcher,
                        () =>
                        {
                            string strConnectionString = "";
                            if (connectionString != null)
                            {
                                strConnectionString = connectionString.Invoke();
                            }
                            using (var manager = new ContextManager(strConnectionString))
                            {
                                if (dataGrid != null)
                                {
                                    dataGrid.DataSource = manager.GetRow(item);
                                }
                            }
                        });
                }
            }
        }
        private static List<string> InputGeastureHasAdd = new List<string>();
        private static void CreateMenuGeneric(
            CoreMenuItem objMenu,
            int index,
            string genericModule,
            MenuAttribute menuAttribute, bool rightClick = false)
        {
            int i = 0;
            string[] arrPath = menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            CoreMenuItem menu = objMenu;
            foreach (string s in arrPath)
            {
                if (i <= index)
                {
                    i++;
                    continue;
                }
                menu = new CoreMenuItem();
                menu.Header = s.ConvertToStatementText();
                objMenu.Items.Add(menu);
                objMenu = menu;
            }
            if (menu != null)
            {
                objMenu.SortAutomatic = true;
                //if (menu.Header != null)
                //    menu.Header = menu.Header.ToString().ConvertToStatementText();
                menu.ControlNameSpace = genericModule;
                menu.Tag = menuAttribute.DomainModel;


                if (!string.IsNullOrEmpty(menuAttribute.HotKeys))
                {
                    if (!InputGeastureHasAdd.Any(n => n.Equals(menuAttribute.HotKeys)) || rightClick)
                    {
                        menu.InputGestureText = menuAttribute.HotKeys;
                        InputGeastureHasAdd.Add(menuAttribute.HotKeys);
                    }
                }
                else
                {
                    //if (menuAttribute is MenuHotkeyAutomaticAttribute)
                    //{
                    //    string hotKeys = "Ctrl+";
                    //    foreach (string key in
                    //        menuAttribute.MenuLocation.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
                    //            .Select(n => n[0].ToString().ToUpper()))
                    //    {
                    //        hotKeys += key + "+";
                    //        Console.WriteLine(hotKeys);
                    //    }
                    //    int lastIndex = menuAttribute.MenuLocation.LastIndexOf('\\');
                    //    string subString =
                    //        menuAttribute.MenuLocation.Substring(
                    //            lastIndex + 1,
                    //            menuAttribute.MenuLocation.Length - 1 - lastIndex).ConvertToStatementText();
                    //    if (subString.Contains(" "))
                    //    {
                    //        foreach (string key in
                    //            subString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    //                .Select(n => n[0].ToString().ToUpper())
                    //                .Skip(1))
                    //        {
                    //            hotKeys += key + "+";
                    //        }
                    //    }
                    //    menu.InputGestureText = hotKeys.Substring(0, hotKeys.Length - 1);
                    //}
                }
            }
        }

        public static void GetDomainMenuAttribute()
        {
            var listModule = ListGenericModule;
            foreach (var type in listModule)
            {
                if (typeof(IGenericCalling).IsAssignableFrom(type.Value))
                {
                    var keyValue = new KeyValue();
                    var typeDomain = type.Value;
                    if (typeDomain != null)
                    {
                        var instance = Activator.CreateInstance(typeDomain);
                        keyValue.Key = instance.GetType().Name;
                        keyValue.Value = instance.GetType();
                    }

                    if (SourceKeyValues.KeyValues == null)
                        SourceKeyValues.KeyValues = new List<KeyValue>();

                    SourceKeyValues.KeyValues.Add(keyValue);
                }
            }
        }

        private static void GetDomain(Type domain, CoreMenuItem menu)
        {
            var keyValue = new KeyValue();
            var typeDomain = domain;
            if (typeDomain != null)
            {
                var instance = Activator.CreateInstance(typeDomain);
                keyValue.Key = instance.GetType().Name;
                keyValue.Value = menu;
            }

            if (SourceKeyValues.KeyValues == null)
                SourceKeyValues.KeyValues = new List<KeyValue>();

            SourceKeyValues.KeyValues.Add(keyValue);
        }

        /// <summary>
        /// untuk mengenerate menu dari file xml
        /// </summary>
        /// <param name="menu">parameter CoreMenu</param>
        /// <param name="contextMenu"> </param>
        public static void GetMenuItemXml(CoreMenu menu, ContextMenu contextMenu = null)
        {
            try
            {
                var pathFile = Path.GetDirectoryName(Application.ResourceAssembly.Location) + "\\";
                const string fileName = "Core.Master.MenuItem.xml";

                var doc = XDocument.Load(pathFile + fileName);
                var menuItems = doc.Descendants("MenuItem");
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                foreach (var menuItem in menuItems)
                {
                    var menuAttribute = new MenuHotkeyAutomaticAttribute();

                    var title = ReadElement("Title", menuItem);
                    var menuLocation = ReadElement("MenuLocation", menuItem);
                    if (menuLocation == @"SistemAdministrator\CRM\SettingDataFixed")
                    {
                        Debug.Print("");
                    }
                    var hotkey = ReadElement("HotKeys", menuItem);
                    var showInRightClickMenu = ReadElement("ShowInRightClickMenu", menuItem);
                    var module = ReadElement("Module", menuItem);
                    var icon = ReadElement("Icon", menuItem);
                    var IsGenericCalling = ReadElement("IsGenericCalling", menuItem);

                    var listDomainItems = new List<Type>();
                    var domainModels = menuItem.Element("DomainModel");
                    if (domainModels != null)
                    {
                        foreach (var item in domainModels.Elements("Item"))
                        {
                            var itemDomainModel = item;
                            if (location != null)
                            {
                                if (itemDomainModel.Value.Contains("Domain"))
                                {
                                    var directories =
                                        Directory.GetFiles(location).Where(
                                            n => n.EndsWith(".dll") && n.Contains("Domain"));
                                    foreach (var directory in directories)
                                    {
                                        var assembly = Assembly.LoadFile(directory);
                                        var typeDomain = assembly.GetType(itemDomainModel.Value);
                                        if (typeDomain != null)
                                        {
                                            listDomainItems.Add(typeDomain);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var isFound = false;
                                    var locationGeneric = location + "\\Package\\";
                                    foreach (var directories in Directory.GetDirectories(locationGeneric))
                                    {
                                        var directorieCommon =
                                        Directory.GetFiles(directories).Where(
                                            n => n.EndsWith(".dll") && n.Contains("Generic"));
                                        foreach (var directory in directorieCommon)
                                        {
                                            var assembly = Assembly.LoadFile(directory);
                                            var typeDomain = assembly.GetType(itemDomainModel.Value);
                                            if (typeDomain != null)
                                            {
                                                listDomainItems.Add(typeDomain);
                                                isFound = true;
                                                break;
                                            }
                                        }

                                        if (isFound)
                                            break;
                                    }

                                    if (!listDomainItems.Any())
                                    {
                                        var typeModel = HelperManager.GetType(itemDomainModel.Value);
                                        if (typeModel != null)
                                        {
                                            listDomainItems.Add(typeModel);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    menuAttribute.DomainModel = listDomainItems;
                    menuAttribute.HotKeys = hotkey;
                    menuAttribute.Icon = icon;
                    menuAttribute.MenuLocation = menuLocation;
                    if (menuLocation.ToLower().Contains("konsul"))
                    {
                        Debug.Print(menuLocation);
                    }
                    menuAttribute.Module = module;
                    menuAttribute.ShowInRightClickMenu = showInRightClickMenu != null ? Convert.ToBoolean(showInRightClickMenu) : false;
                    menuAttribute.Title = title;

                    if (IsGenericCalling != null)
                    {
                        var transaction = Convert.ToBoolean(IsGenericCalling);
                        if (transaction)
                        {
                            if (menuAttribute.ShowInRightClickMenu)
                            {
                                #region Klik Kanan Menu

                                if (contextMenu != null)
                                {
                                    var firstDomain = menuAttribute.DomainModel as List<Type>;
                                    foreach (var generic in ListGenericModule.Where(n => n.Value == firstDomain.FirstOrDefault()))
                                    {
                                        if (generic.Value.IsInterface)
                                            continue;

                                        if (string.IsNullOrEmpty(menuAttribute.Module))
                                            BindMenu(menu, menuAttribute, generic, true);
                                        else
                                        {
                                            var validMenu = false;
                                            var arrModules = menuAttribute.Module.Split(new[] { ',' });
                                            foreach (var arrModule in arrModules)
                                            {
                                                var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                                if (repositoryLocalSotrage != null)
                                                {
                                                    foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                                    {
                                                        if (s.Equals(arrModule))
                                                        {
                                                            BindMenu(menu, menuAttribute, generic, true);
                                                            validMenu = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (validMenu) break;
                                            }
                                        }

                                        //

                                        var coreMenu = new CoreMenuItem();
                                        coreMenu.Header =
                                            menuAttribute.MenuLocation.Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries)[
                                                                                 menuAttribute.MenuLocation.Split(
                                                                                     new[] { '\\' },
                                                                                     StringSplitOptions.RemoveEmptyEntries)
                                                                                     .Length - 1].ConvertToStatementText();
                                        CreateMenuGeneric(
                                            coreMenu,
                                            menuAttribute.MenuLocation.Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries)
                                                .Length,
                                            generic.Key,
                                            menuAttribute, true);
                                        if (string.IsNullOrEmpty(menuAttribute.Module))
                                        {
                                            contextMenu.Items.Add(coreMenu);
                                        }
                                        else
                                        {
                                            var validMenu = false;
                                            var arrModule = menuAttribute.Module.Split(new[] { ',' });
                                            foreach (var amodule in arrModule)
                                            {
                                                var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                                if (repositoryLocalSotrage != null)
                                                {
                                                    foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                                    {
                                                        if (s.Equals(amodule))
                                                        {
                                                            contextMenu.Items.Add(coreMenu);
                                                            validMenu = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (validMenu) break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Menu Transaction

                                var firstDomain = menuAttribute.DomainModel as List<Type>;
                                foreach (var generic in ListGenericModule.Where(n => n.Value == firstDomain.FirstOrDefault()))
                                {
                                    if (generic.Value.IsInterface)
                                        continue;

                                    if (string.IsNullOrEmpty(menuAttribute.Module))
                                        BindMenu(menu, menuAttribute, generic);
                                    else
                                    {
                                        var validMenu = false;
                                        var arrModules = menuAttribute.Module.Split(new[] { ',' });
                                        foreach (var arrModule in arrModules)
                                        {
                                            var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                                            if (repositoryLocalSotrage != null)
                                            {
                                                foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                                                {
                                                    if (s.Equals(arrModule))
                                                    {
                                                        BindMenu(menu, menuAttribute, generic);
                                                        validMenu = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (validMenu) break;
                                        }
                                    }
                                }

                                #endregion

                            }
                        }
                        else
                        {
                            CreateMenuMaster(menu, menuAttribute);
                        }
                    }
                    else
                    {
                        CreateMenuMaster(menu, menuAttribute);
                    }
                }

                foreach (var result in menu.Items.OfType<CoreMenuItem>())
                {
                    SortByName(result);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static void CreateMenuMaster(CoreMenu menu, MenuHotkeyAutomaticAttribute menuAttribute)
        {
            #region Menu Master

            foreach (var type in HelperManager.GetListAttachPresenter().Where(n => n.Key == "CreateUpdateDeleteMedicalReocrdPresenter"))
            {
                if (type.Value.IsInterface)
                    continue;

                if (string.IsNullOrEmpty(menuAttribute.Module))
                    BindMenu(menu, menuAttribute, type);
                else
                {
                    var validMenu = false;
                    var arrModules = menuAttribute.Module.Split(new[] { ',' });
                    foreach (var arrModule in arrModules)
                    {
                        var repositoryLocalSotrage = BaseDependency.Get<IHistoryLoginRepository>();
                        if (repositoryLocalSotrage != null)
                        {
                            foreach (var s in repositoryLocalSotrage.Module.Split(new[] { ',' }))
                            {
                                if (s.Equals(arrModule))
                                {
                                    BindMenu(menu, menuAttribute, type);
                                    validMenu = true;
                                    break;
                                }
                            }
                        }
                        if (validMenu) break;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// untuk mendapatkan value dari element xml
        /// </summary>
        /// <param name="objectRead">parameter string nama tag element</param>
        /// <param name="xElement">parameter xelement</param>
        /// <returns>string value</returns>
        private static string ReadElement(string objectRead, XElement xElement)
        {
            try
            {
                var element = xElement.Element(objectRead);
                if (element != null)
                {
                    var elementValue = element.Value;
                    return elementValue;
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                //Console.WriteLine(args);
                if (args.RequestingAssembly == null || string.IsNullOrEmpty(args.RequestingAssembly.Location))
                {
                    return null;
                }

                string directory = Path.GetDirectoryName(args.RequestingAssembly.Location);
                if (Directory.Exists(directory))
                {
                    //foreach (string varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                    //{
                    //    if (varible.Contains("Core.Framework.Windows"))
                    //    {
                    //        continue;
                    //    }
                    //    if (Path.GetFileNameWithoutExtension(varible).Contains("Core")
                    //        || Path.GetFileNameWithoutExtension(varible).Contains("Jasamedika")
                    //        || Path.GetFileNameWithoutExtension(varible).Contains("Medifirst"))
                    //    {
                    //        Assembly assembly = Assembly.LoadFile(varible);
                    //        if (assembly.FullName.Equals(args.Name))
                    //        {
                    //            return assembly;
                    //        }
                    //    }
                    //}

                    if (directory != null)// GetDirectories(directory))
                    {
                        foreach (var dir in Directory.GetDirectories(directory))
                        {
                            foreach (var source in Directory.GetFiles(dir).Where(n => n.EndsWith(".dll")))
                            {
                                if (source != null &&
                               (source.Contains("Core")
                               || source.Contains("Jasamedika")
                               || source.Contains("Medifirst")))
                                {
                                    Assembly assembly = Assembly.LoadFile(source);
                                    if (assembly.FullName.Equals(args.Name))
                                    {
                                        return assembly;
                                    }
                                    if (args.Name.Contains("resources") &&
                                assembly.FullName.Equals(args.RequestingAssembly.FullName))
                                    {
                                        pathLoadAssembly = source;
                                        var asm = Assembly.LoadFile(source);
                                        string resourceName = asm.GetName().Name + ".resources";
                                        if (args.Name.Contains(resourceName))
                                            foreach (var fileStream in asm.GetFiles(true))
                                            {
                                                var bytes = new List<byte>();
                                                while (fileStream.Position != fileStream.Length)
                                                {
                                                    var b = fileStream.ReadByte();
                                                    try
                                                    {
                                                        if (b == -1)
                                                            b = 0;
                                                        bytes.Add(Convert.ToByte(b));
                                                    }
                                                    catch (Exception)
                                                    {
                                                    }

                                                }
                                                var y = bytes.Where(n => n != 0);
                                                return Assembly.Load(bytes.ToArray());

                                            }

                                    }
                                }
                            }
                        }
                    }

                    return FindAssembly(Path.GetDirectoryName(directory), args.Name);
                }
                return null;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }
        }

        private static Assembly FindAssembly(string directory, string name)
        {
            if (Directory.Exists(directory))
            {
                //foreach (string varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                //{
                //    if (varible.Contains("Core.Framework.Windows"))
                //    {
                //        continue;
                //    }
                //    if (Path.GetFileNameWithoutExtension(varible).Contains("Core")
                //        || Path.GetFileNameWithoutExtension(varible).Contains("Jasamedika")
                //        || Path.GetFileNameWithoutExtension(varible).Contains("Medifirst"))
                //    {
                //        Assembly assembly = Assembly.LoadFile(varible);
                //        if (assembly.FullName.Equals(name))
                //        {
                //            return assembly;
                //        }
                //    }
                //}
                try
                {


                    if (directory != null) // GetDirectories(directory))
                    {
                        foreach (var dir in Directory.GetDirectories(directory))
                        {
                            foreach (var source in Directory.GetFiles(dir).Where(n => n.EndsWith(".dll")))
                            {
                                if (source != null &&
                                    (source.Contains("Core")
                                     || source.Contains("Jasamedika")
                                     || source.Contains("Medifirst")))
                                {
                                    Assembly assembly = Assembly.LoadFile(source);
                                    if (assembly.FullName.Equals(name))
                                    {
                                        return assembly;
                                    }
                                }
                            }
                        }
                    }

                    return FindAssembly(Path.GetDirectoryName(directory), name);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                    return null;
                }

            }
            return null;
        }

        private static void SortByName(CoreMenuItem menu)
        {
            foreach (CoreMenuItem menuItem in menu.Items.OfType<CoreMenuItem>())
            {
                if (menuItem.Items.OfType<CoreMenuItem>().Any(n => n.SortAutomatic))
                {
                    var listMenuTemp = new List<CoreMenuItem>();
                    listMenuTemp.AddRange(menuItem.Items.OfType<CoreMenuItem>());
                    menuItem.Items.Clear();
                    foreach (var coreMenuItem in
                        listMenuTemp.OrderBy(n => n.Header.ToString()).GroupBy(n => n.Header.ToString()[0]))
                    {
                        menuItem.Items.Add(
                            new CoreMenuItem
                            {
                                Header =
                                        new TextBlock
                                        {
                                            Text =
                                                    coreMenuItem.Key.ToString(
                                                        CultureInfo.InvariantCulture).ToUpper(),
                                            FontWeight = FontWeights.Bold
                                        }
                            });
                        menuItem.Items.Add(new Separator());
                        foreach (CoreMenuItem fillMenu in coreMenuItem)
                        {
                            fillMenu.Header = fillMenu.Header.ToString().ConvertToStatementText();
                            if (!fillMenu.Header.ToString().Contains("System. Windows. Controls. Text Block"))
                                menuItem.Items.Add(fillMenu);
                        }
                    }
                }
                SortByName(menuItem);
            }
        }
        private static readonly List<string> fileHasLoad = new List<string>();
        private static readonly List<string> listClassHasLoad = new List<string>();

        public static bool CheckModule(string name, string module)
        {
            MethodInfo type;
            var result = DictionaryComonExecute.TryGetValue(name + "+" + module, out type);
            if (!result)
            {
                var loadModule = DictionaryLoadExecute.FirstOrDefault(n => n.Key.Equals(module));
                if (loadModule != null)
                    return true;
            }
            return result;
        }
        public static void CallModule(string name, string module, params object[] objects)
        {
            MethodInfo type;
            if (DictionaryComonExecute.TryGetValue(name + "+" + module, out type))
            {
                var instance = Activator.CreateInstance(type.DeclaringType);
                if (objects != null) type.Invoke(instance, objects);
                return;
            }
            if (DictionaryLoadExecute.FirstOrDefault(n => n.Key.Equals(module)) != null)
            {
                Manager.Invoke(module, objects);
                return;
            }
            throw new ArgumentNullException("Object Not Found");
        }
        private static void SyncronDetail(string location)
        {
            foreach (string directory in Directory.GetDirectories(location))
            {
                SyncronDetail(directory);
            }
            foreach (string file in Directory.GetFiles(location).Where(n => n.EndsWith(".dll")))
            {
                //MessageListener.Instance.ReceiveMessage("Load " + Path.GetFileNameWithoutExtension(file) + " ");
                try
                {
                    if (file.Contains("Core.Framework.Windows"))
                    {
                        continue;
                    }
                    AppDomain.CurrentDomain.ResourceResolve += CurrentDomainOnResourceResolve;
                    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                    if (Path.GetFileNameWithoutExtension(file).Contains("Core")
                        || Path.GetFileNameWithoutExtension(file).Contains("Jasamedika")
                        || Path.GetFileNameWithoutExtension(file).Contains("Medifirst"))
                    {
                        if (fileHasLoad.Any(n => n.Equals(Path.GetFileName(file)))) continue;
                        fileHasLoad.Add(Path.GetFileName(file));
                        foreach (Type type in Assembly.LoadFile(file).GetExportedTypes())
                        {
                            if (!type.IsClass)
                            {
                                continue;
                            }
                            if (typeof(IGenericCalling).IsAssignableFrom(type))
                            {
                                Type item;
                                foreach (var result in type.GetCustomAttributes(true).OfType<CommonExecuteAttribute>())
                                {
                                    MethodInfo typeLocal;
                                    foreach (var source in type.GetMethods().Where(n => n.GetCustomAttributes(true).Any(x => x is CommonModuleExecuteAttribute)))
                                    {
                                        foreach (var result1 in source.GetCustomAttributes(true).OfType<CommonModuleExecuteAttribute>())
                                        {
                                            if (!DictionaryComonExecute.TryGetValue(result.Name + "+" + result1.Name, out typeLocal))
                                                DictionaryComonExecute.Add(result.Name + "+" + result1.Name, source);
                                            else
                                                DictionaryComonExecute[result.Name + "+" + result1.Name] = source;
                                        }
                                    }

                                }
                                foreach (var result in type.GetCustomAttributes(true).OfType<LoadAttribute>())
                                {
                                    var instance = Activator.CreateInstance(type);
                                    foreach (var source in type.GetMethods().Where(n => n.GetCustomAttributes(true).Any(x => x is LoadAttribute)))
                                    {
                                        foreach (var result1 in source.GetCustomAttributes(true).OfType<LoadAttribute>())
                                        {
                                            DictionaryLoadExecute.Add(new KeyValue()
                                            {
                                                Key = result1.Name,
                                                Value = source
                                            });
                                        }
                                    }

                                }
                                if (!ListGenericModule.TryGetValue(type.Name, out item))
                                {
                                    ListGenericModule.Add(type.Name, type);
                                }
                                else
                                {
                                    ListGenericModule[type.Name] = type;
                                }
                            }

                            if (type.IsClass)
                            {
                                try
                                {
                                    Type item;
                                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                                    if (constructor != null)
                                    {
                                        if (!constructor.ContainsGenericParameters)
                                        {
                                            if (type.FullName.Contains("Report")) continue;
                                            object instance = null;
                                            if ((type.FullName.EndsWith("View")
                                                 && (type.Module.ToString().Contains("Form")) || type.Module.ToString().Contains("Views")))
                                            {
                                                if (GenereateLanguange)
                                                {
                                                    instance = Activator.CreateInstance(type);
                                                    if (listClassHasLoad.Any(n => n.Equals(type.ToString()))) continue;
                                                    listClassHasLoad.Add(type.ToString());
                                                    var window = instance as CoreUserControl;
                                                    if (window != null)
                                                    {

                                                        if (window is IControlAuthentication)
                                                        {
                                                            OnInitializeControlAuthentication(new ItemEventArgs<IControlAuthentication>(window));
                                                        }
                                                        var pathLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Language";
                                                        if (!Directory.Exists(pathLocation))
                                                        {
                                                            Directory.CreateDirectory(pathLocation);
                                                        }
                                                        var pathFile = pathLocation + "\\" + type.Name;
                                                        ISetting setting = new SettingXml(pathFile, SettingXml.TypeFile.PathFile);
                                                        var listTextBlock = (window.Content as Panel).FindChildren<TextBlock>();
                                                        var findChildren = listTextBlock as TextBlock[] ?? listTextBlock.ToArray();
                                                        if (findChildren.Any())
                                                            foreach (var findChild in findChildren)
                                                            {

                                                                if (!string.IsNullOrEmpty(findChild.Text))
                                                                    setting.Save("Label-" + findChild.Text, findChild.Text);
                                                                else
                                                                    foreach (var inline in findChild.Inlines.OfType<Run>())
                                                                        setting.Save("Label-" + inline.Text, inline.Text);
                                                            }


                                                        var listButton = (window.Content as Panel).FindChildren<CoreButton>();
                                                        var coreButtons = listButton as CoreButton[] ?? listButton.ToArray();
                                                        if (coreButtons.Any())
                                                            foreach (var findChild in coreButtons)
                                                            {
                                                                if (findChild is IControlAuthentication)
                                                                {
                                                                    OnInitializeControlAuthentication(new ItemEventArgs<IControlAuthentication>(findChild));
                                                                }
                                                                if (findChild.Content is string)
                                                                {

                                                                    if (findChild.ToolTip != null)
                                                                    {
                                                                        var panel = (findChild.ToolTip as ToolTip).Content as StackPanel;
                                                                        if (panel != null)
                                                                        {
                                                                            foreach (
                                                                                var child in
                                                                                    panel.Children.OfType<TextBlock>())
                                                                            {
                                                                                setting.Save(
                                                                                    "Button-Tooltip-" + child.Text,
                                                                                    child.Text.ToString());
                                                                            }
                                                                        }
                                                                    }
                                                                    setting.Save("Button-" + findChild.Content, findChild.Content.ToString());
                                                                }
                                                            }
                                                        var listGroups = (window.Content as Panel).FindChildren<GroupBox>();
                                                        var groupBoxs = listGroups as GroupBox[] ?? listGroups.ToArray();
                                                        if (groupBoxs.Any())
                                                            foreach (var findChild in groupBoxs)
                                                                if (findChild.Header is string)
                                                                    setting.Save("Group-" + findChild.Header, findChild.Header.ToString());
                                                        var listCheckBoxs = (window.Content as Panel).FindChildren<CoreCheckBox>();
                                                        var coreCheckBoxs = listCheckBoxs as CoreCheckBox[] ?? listCheckBoxs.ToArray();
                                                        if (coreCheckBoxs.Any())
                                                            foreach (var findChild in coreCheckBoxs)
                                                                if (findChild.Content is string)
                                                                    setting.Save("CheckBox-" + findChild.Content, findChild.Content.ToString());

                                                        var listRadioButtons = (window.Content as Panel).FindChildren<CoreRadioButton>();
                                                        var coreRadioButtons = listRadioButtons as CoreRadioButton[] ?? listRadioButtons.ToArray();
                                                        if (coreRadioButtons.Any())
                                                            foreach (var findChild in coreRadioButtons)
                                                                if (findChild.Content is string)
                                                                    setting.Save("RadioButton-" + findChild.Content, findChild.Content.ToString());

                                                        var listListView = (window.Content as Panel).FindChildren<CoreListView>().ToList();
                                                        if (listListView.Any())
                                                            foreach (var findChild in listListView)
                                                            {
                                                                var view = findChild.View as GridView;
                                                                if (view != null)
                                                                    foreach (var coreListView in view.Columns.Cast<GridViewColumn>())
                                                                        if (coreListView.Header is string)
                                                                            setting.Save("DataGrid-" + coreListView.Header, coreListView.Header.ToString());

                                                            }

                                                        var listLDataGrid = (window.Content as Panel).FindChildren<CoreDataGrid>().ToList();
                                                        if (listLDataGrid.Any())
                                                            foreach (var findChild in listLDataGrid)
                                                            {
                                                                foreach (var coreListView in findChild.Columns.Cast<DataGridColumn>())
                                                                    if (coreListView.Header is string)
                                                                        setting.Save("Grid-" + coreListView.Header, coreListView.Header.ToString());

                                                            }

                                                        var listTextBox = (window.Content as Panel).FindChildren<CoreTextBox>().ToList();
                                                        if (listTextBox.Any())
                                                            foreach (var findChild in listTextBox)
                                                            {
                                                                if (findChild is IControlAuthentication)
                                                                {
                                                                    OnInitializeControlAuthentication(new ItemEventArgs<IControlAuthentication>(findChild));
                                                                }
                                                                if (findChild.ToolTip != null)
                                                                {
                                                                    var panel = (findChild.ToolTip as ToolTip).Content as StackPanel;
                                                                    if (panel != null)
                                                                    {
                                                                        foreach (
                                                                            var child in
                                                                                panel.Children.OfType<TextBlock>())
                                                                        {
                                                                            setting.Save(
                                                                                "TextBox-Tooltip-" + child.Text,
                                                                                child.Text.ToString());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                    }
                                                }
                                            }
                                            if (instance is IManageView)
                                            {
                                                if (!ListGenericView.TryGetValue(type.Name, out item))
                                                {
                                                    ListGenericView.Add(type.Name, type);
                                                }
                                                else
                                                {
                                                    ListGenericView[type.Name] = type;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    Thread.Sleep(2);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        private static string pathLoadAssembly = "";
        private static Assembly CurrentDomainOnResourceResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathLoadAssembly))
                    return Assembly.LoadFile(pathLoadAssembly);
                return null;
            }
            finally
            {
                pathLoadAssembly = "";
            }
        }

        #endregion

        public static void ContentMessage(string message)
        {
            var messageWindow = BaseDependency.Get<IMessageWindow>();
            if (messageWindow != null)
            {
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                {
                    log.Info(message);
                }
                messageWindow.Info("Informasi", message);
                return;
            }
            Timeout(Application.Current.Dispatcher, () =>
            {
                var window = new ChildWindow();

                window.Owner = Application.Current.MainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                var contentPlace = new ContentMessage();
                contentPlace.TbTitleMessage.Text = message;
                window.Content = contentPlace;
                window.BorderBrush = contentPlace.Background;
                contentPlace.YesResult += (sender, args) => window.DialogResult = true;
                bool? resultDialog = window.ShowDialog();
            });
        }

        public static bool GenereateLanguange { get; set; }
        public static event EventHandler<ItemEventArgs<IControlAuthentication>> InitializeControlAuthentication;

        public static void Invoke(string moduleName, params object[] metroWindow)
        {
            foreach (var loadExecute in DictionaryLoadExecute.Where(n => n.Key.Equals(moduleName)))
            {
                var methodInfo = loadExecute.Value as MethodInfo;
                if (methodInfo != null)
                {
                    var obj = Activator.CreateInstance(methodInfo.DeclaringType);
                    methodInfo.Invoke(obj, metroWindow);
                }
            }
        }

        private static void OnInitializeControlAuthentication(ItemEventArgs<IControlAuthentication> e)
        {
            if (InitializeControlAuthentication != null) InitializeControlAuthentication.Invoke(null, e);
        }

        public static void RegisterFormGrid(FrameworkElement control)
        {
            var findHead = FindVisualParent<FormGrid>(control);
            if (findHead != null)
            {
                findHead.AddChlidValidation(control);
            }
            else
            {
                control.Loaded += (sender, args) =>
                {
                    RegisterFormGrid(sender as FrameworkElement);
                };
            }
        }
    }
}