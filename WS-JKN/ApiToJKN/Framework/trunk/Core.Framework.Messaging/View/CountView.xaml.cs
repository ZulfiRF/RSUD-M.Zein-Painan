using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace Core.Framework.Messaging.View
{
    /// <summary>
    /// Interaction logic for CountView.xaml
    /// </summary>
    public partial class CountView : UserControl
    {
        public CountView()
        {
            InitializeComponent();
            MouseDown += OnMouseDown;
            Loaded += OnLoaded;
        }

        private XElement SetMenuEditor(XElement doc, string tagXml)
        {
            var element = doc.Element(tagXml);
            return element != null ? element : null;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var pathFile = Path.GetDirectoryName(Application.ResourceAssembly.Location) + "\\";
            const string fileName = "Tema.xml";
            var readFile = XDocument.Load(pathFile + fileName);
            var doc = readFile.Descendants("tema").FirstOrDefault();

            if (doc != null)
            {
                var temaIcon = SetMenuEditor(doc, "icon-main").Value.Trim();
                Lonceng.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaIcon));
                Bandul.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaIcon));
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!Pop.IsOpen)
            {
                Pop.IsOpen = true;
            }
            else
            {
                Pop.IsOpen = false;
            }
        }

        public void Fill(NotifyView view)
        {
            SpItem.Children.Add(view);
            RefreshCount();
        }

        public void RefreshCount()
        {
            var jumlah = SpItem.Children.Count;
            if (jumlah > 0)
            {
                TbCount.Text = jumlah.ToString();
                TbCount.Visibility = Visibility.Visible;
            }
            else
            {
                TbCount.Visibility = Visibility.Collapsed;
            }
        }

        public List<NotifyView> GetDetail()
        {
            return SpItem.Children.OfType<NotifyView>().ToList();
        }

        public void Remove(NotifyView notify)
        {
            var items = SpItem.Children.OfType<NotifyView>().ToList().Any(n => n.Message == notify.Message);
            if (items)
            {
                var removed = SpItem.Children.OfType<NotifyView>().ToList();
                removed.Remove(notify);
                SpItem.Children.Clear();
                foreach (var notifyView in removed)
                {
                    SpItem.Children.Add(notifyView);
                }
                RefreshCount();
            }
        }
    }
}
