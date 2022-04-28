using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Framework.Helper.Presenters;
using Core.Framework.LanguageEditor.Contract;
using Core.Framework.LanguageEditor.Model;
using Core.Framework.LanguageEditor.Presenter;
using Core.Framework.Windows.Helper;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Core.Framework.LanguageEditor
{
    /// <summary>
    /// Interaction logic for LanguageView.xaml
    /// </summary>
    public partial class LanguageView : IXmlView
    {
        public LanguageView()
        {
            InitializeComponent();
            TvFileXml.MouseDoubleClick += TvFileXmlOnMouseDoubleClick;
            LvDetailXml.MouseDoubleClick += LvDetailXmlOnMouseDoubleClick;
            BtnSimpan.Click += BtnSimpanOnClick;
            BtnBatal.Click += BtnBatalOnClick;
            TvFileXml.KeyDown += TvFileXmlOnKeyDown;
            LvDetailXml.KeyDown += LvDetailXmlOnKeyDown;
            TbTranslate.KeyDown += TbTranslateOnKeyDown;
            KeyDown += OnKeyDown;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Presenter.LoadData();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F12)
                Save();
        }

        private void TbTranslateOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == System.Windows.Input.Key.Return)
                BtnSimpan.Focus();
        }

        private void LvDetailXmlOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == System.Windows.Input.Key.Return)
            {
                if (LvDetailXml.SelectedItem != null)
                {
                    SetList(LvDetailXml);
                    TbTranslate.Focus();
                }
            }
        }

        private void TvFileXmlOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                if (TvFileXml.SelectedItem != null)
                {
                    LvDetailXml.ItemsSource = Presenter.XmlSource();
                    LvDetailXml.Focus();
                }
            }
        }

        private void BtnBatalOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            TbComponent.Clear();
            TbKey.Clear();
            TbTranslate.Clear();
        }

        private void TvFileXmlOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            LvDetailXml.ItemsSource = Presenter.XmlSource();
        }

        private void LvDetailXmlOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            SetList(sender);
        }

        public void SetList(object sender)
        {
            var item = LvDetailXml.SelectedItem as Item;
            if (item != null)
            {
                TbComponent.Text = item.Component;
                TbKey.Text = item.Key;
                TbTranslate.Text = item.Translate;
            }
        }

        private void BtnSimpanOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Save();
        }

        public void Save()
        {
            try
            {
                if (Presenter.SaveXml())
                {
                    Message = new MessageItem() { Content = "Data berhasil disimpan!", MessageType = MessageType.Success };
                    ReLoad();
                }
                else
                {
                    Message = new MessageItem() { Content = "Data gagal disimpan!", MessageType = MessageType.Error };
                }
            }
            catch (Exception e)
            {
                Message = new MessageItem() { Exceptions = e, MessageType = MessageType.Error };
            }
        }

        public void ReLoad()
        {
            if (TvFileXml.SelectedItem != null)
            {
                LvDetailXml.ItemsSource = Presenter.XmlSource();
            }
        }

        public override MessageItem Message
        {
            set { base.Message = value; }
        }

        public void AttachPresenter(object presenter)
        {
            Presenter = presenter as XmlPresenter;
        }

        public XmlPresenter Presenter { get; set; }

        public string Key { get { return TbComponent.Text + "-" + TbKey.Text; } }
        public DirectoryInfo Names
        {
            get
            {
                if (DirectoryName != null)
                {
                    var directoryInfo = new DirectoryInfo(DirectoryName);
                    return directoryInfo;
                }
                return null;
            }
        }
        public string DirectoryName { get; set; }
        public string Directory
        {
            get
            {
                var item = TvFileXml.SelectedItem as TreeViewItem;
                if (item != null)
                {
                    var data = item.Tag as FileView;
                    return data != null ? data.Path : null;
                }
                throw new Exception("File belum di pilih!");
            }
        }
        public string Translate { get { return TbTranslate.Text; } }

        public List<FileView> SetSource
        {
            set
            {
                var result = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    TvFileXml.Items.Clear();
                    foreach (var fileView in result)
                    {
                        var treeItem = new TreeViewItem()
                        {
                            Header = fileView.NamaFile,
                            Tag = fileView
                        };
                        TvFileXml.Items.Add(treeItem);
                    }
                });
            }
        }
    }
}
