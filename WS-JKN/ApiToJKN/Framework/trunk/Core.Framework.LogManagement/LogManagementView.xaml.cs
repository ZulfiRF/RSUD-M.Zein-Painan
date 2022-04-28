using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Framework.Helper.Presenters;
using Core.Framework.Helper.Security;
using Core.Framework.LogManagement.Contract;
using Core.Framework.LogManagement.Presenter;
using Core.Framework.LogManagement.ViewModel;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Implementations;

namespace Core.Framework.LogManagement
{
    /// <summary>
    /// Interaction logic for LogManagementViewxaml.xaml
    /// </summary>
    public partial class LogManagementView : ILogManagementView
    {
        public LogManagementView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            BtnKirim.Click += BtnKirimOnClick;
        }

        private void BtnKirimOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Presenter.KirimDokumen();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Presenter.LoadFile();
            Presenter.LoadConfig();
        }

        public override MessageItem Message
        {
            set { base.Message = value; }
        }

        public void AttachPresenter(object presenter)
        {
            Presenter = presenter as LogManagementPresenter;
        }

        public LogManagementPresenter Presenter { get; set; }

        public List<FileLog> SetLogSource
        {
            set
            {
                var logFiles = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    WpLog.Children.Clear();
                    foreach (var logFile in logFiles)
                    {
                        var checkBox = new CoreCheckBox();
                        checkBox.Click -= CheckBoxOnClick;
                        checkBox.Click += CheckBoxOnClick;
                        checkBox.MouseDoubleClick -= CheckBoxOnMouseDoubleClick;
                        checkBox.MouseDoubleClick += CheckBoxOnMouseDoubleClick;
                        checkBox.Content = logFile.NamaFile;
                        checkBox.Tag = logFile;
                        checkBox.IsChecked = true;
                        WpLog.Children.Add(checkBox);
                    }
                });
            }
        }

        public List<FileLog> CurrentFileSend
        {
            get
            {
                var items = WpLog.Children.OfType<CoreCheckBox>().Where(n => n.IsChecked == true).ToList();
                var logs = items.Select(n => n.Tag as FileLog).ToList();
                return logs;
            }
        }

        public bool SetResult
        {
            set
            {
                var result = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    if (result)
                    {
                        Message = new MessageItem() { Content = "Date berhasil dikirim!", MessageType = MessageType.Success };
                    }
                    else
                    {
                        Message = new MessageItem() { Content = "Date gagal dikirim!", MessageType = MessageType.Error };
                    }
                });
            }
        }

        public Exception SetError
        {
            set
            {
                var result = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    if (result != null)
                    {
                        Message = new MessageItem() { Exceptions = result, MessageType = MessageType.Error };
                    }
                });
            }
        }

        public string CurrentUsername { get { return TbUserName.Text; } }
        public string CurrentPassword { get { return TbPassword.Password.ToString(); } }

        public string[] SetConfig
        {
            set
            {
                var configs = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    TbNamaRs.Text = configs[0];
                    TbAlamatRs.Text = configs[1];
                    TbEmailRs.Text = configs[2];
                    TbPassword.Password = Cryptography.FuncAesDecrypt(configs[3]);
                    TbUserName.Text = configs[2];
                });
            }
        }

        private void CheckBoxOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var checkBox = sender as CoreCheckBox;
            if (checkBox != null)
            {
                var fileLog = checkBox.Tag as FileLog;
                if (fileLog != null)
                {
                    var read = File.ReadAllText(fileLog.Path);
                    var flowDoc = new FlowDocument();
                    flowDoc.Blocks.Add(new Paragraph(new Run(read)));
                    RtFile.Document = flowDoc;
                }
            }
        }

        private void CheckBoxOnClick(object sender, RoutedEventArgs routedEventArgs)
        {

        }
    }
}
