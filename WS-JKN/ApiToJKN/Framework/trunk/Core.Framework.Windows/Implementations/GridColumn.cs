using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class GridColumn : ContentControl
    {
        private static object _objLock = new object();
        public GridColumn()
        {
            Loaded += OnLoaded;
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var textBlock = new TextBlock();
            if (DataContext is INotifyPropertyChanged)
            {
                (DataContext as INotifyPropertyChanged).PropertyChanged+=
                    delegate(object o, PropertyChangedEventArgs args)
                    {
                        if(args.PropertyName.Equals(Display))
                            textBlock.Text = HelperManager.BindPengambilanObjekDariSource(o, Display).ToString();
                    };
            }
            var result = HelperManager.BindPengambilanObjekDariSource(DataContext, Display.ToString());
            textBlock.Text = result != null ? result.ToString() : null;
            Content = textBlock;
            var contextMenu = new ContextMenu();
            var menu = new CoreMenuItem();
            menu.Header = "Copy";
            menu.Click += MenuOnClick;
            contextMenu.Items.Add(menu);
            ContextMenu = contextMenu;
        }

        private void MenuOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var textBlock = Content as TextBlock;
            if (textBlock != null) Clipboard.SetText(textBlock.Text);
        }

        public string Display
        {
            get { return (string)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Display.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayProperty =
            DependencyProperty.Register("Display", typeof(string), typeof(GridColumn), new UIPropertyMetadata(null));


    }
}