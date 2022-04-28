using System;
using System.Windows;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Presenters;
using Core.Framework.Security.Contracts;
using Core.Framework.Security.Control;

namespace Core.Framework.Security
{
    /// <summary>
    /// Interaction logic for ForbiddenView.xaml
    /// </summary>
    public partial class ForbiddenView : IForbiddenView
    {
        public ForbiddenView()
        {
            InitializeComponent();
            BtnLogin.Click += BtnLoginOnClick;
        }


        public override MessageItem Message
        {
            set { base.Message = value; }
        }

        private void BtnLoginOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Presenter.Login(true))
            {
                var isHavingAccess = Presenter.IsHavingAccess(PassingView.GetType().Name, true);
                if (isHavingAccess)
                {
                    Presenter.ChangedView();
                }
                else
                {
                    Message = new MessageItem() { Content = "Anda Tidak Mempunyai Akses untuk Membuka Halaman ini!", MessageType = MessageType.Error };
                }
            }
            else
            {
                var pesan = Presenter.MsgException();
                if (pesan != null)
                {
                    Message = new MessageItem() { Content = pesan, MessageType = MessageType.Error };
                    #region PlayColors
                    var colorRed = new SolidColorBrush(Colors.Red);
                    var colorBlack = new SolidColorBrush(Colors.Black);
                    if (pesan.Contains("Username"))
                    {
                        TbUsername.Focus();
                        TbUsername.Foreground = colorRed;
                        TbUsername.BorderBrush = colorRed;
                        TbPassword.Foreground = colorBlack;
                        TbPassword.BorderBrush = colorBlack;
                    }
                    else if (pesan.Contains("Password"))
                    {
                        TbPassword.Focus();
                        TbPassword.Foreground = colorRed;
                        TbPassword.BorderBrush = colorRed;
                        TbUsername.BorderBrush = colorBlack;
                    }
                    #endregion
                }
            }
        }

        public void SetAtasan(string atasan)
        {
            if (!string.IsNullOrEmpty(atasan))
                TbHunbungi.Text = "Hubungi " + atasan;
            else
                TbHunbungi.Text = "Hubungi Atasan Anda";
        }

        public FrameworkElement PassingView { get; set; }
        public FrameworkElement ForbiddenVIew { get { return this; } }

        public void AttachPresenter(object presenter)
        {
            Presenter = presenter as ForbiddenPresenter;
        }

        public ForbiddenPresenter Presenter { get; set; }
        public string Username { get { return TbUsername.Text; } }

        public string Passowrd { get { return TbPassword.HiddenText; } }
        public Brush SetUsernames
        {
            set
            {
                TbUsername.Foreground = value;
                TbUsername.BorderBrush = value;
            }
        }
        public Brush SetPasswords
        {
            set
            {
                TbPassword.Foreground = value;
                TbPassword.BorderBrush = value;
            }
        }
        public Brush SetRuangans { set; private get; }
        public bool SetBtnLogin { set; private get; }
        public void ToMainWindow()
        {

        }

        public void ClearAll()
        {
            TbPassword.ClearValueControl();
            TbUsername.ClearValueControl();
        }
    }
}
