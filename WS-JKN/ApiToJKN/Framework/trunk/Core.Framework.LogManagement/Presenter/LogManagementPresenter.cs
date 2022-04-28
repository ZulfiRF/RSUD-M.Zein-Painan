using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Core.Framework.Helper.Presenters;
using Core.Framework.LogManagement.Contract;
using Core.Framework.LogManagement.ViewModel;

namespace Core.Framework.LogManagement.Presenter
{
    public class LogManagementPresenter : BasePresenterViewCommand<ILogManagementView, ILogManagementCommand>
    {
        public LogManagementPresenter(ILogManagementView view, ILogManagementCommand command) : base(view, command)
        {

        }

        public override void Initialize(params object[] parameters)
        {
            base.Initialize(parameters);
        }

        public void LoadFile()
        {
            ThreadPool.QueueUserWorkItem(GetDataCallBack);
        }

        private void GetDataCallBack(object state)
        {
            var result = Command.GetData();
            View.SetLogSource = result;
        }

        public void KirimDokumen()
        {
            var files = View.CurrentFileSend;
            var username = View.CurrentUsername;
            var password = View.CurrentPassword;
            ThreadPool.QueueUserWorkItem(SendCallBack, new object[] { files, username, password });
        }

        private void SendCallBack(object state)
        {
            try
            {
                var param = state as object[];
                if (param != null)
                {
                    var files = param[0] as List<FileLog>;
                    var usename = param[1].ToString();
                    var pass = param[2].ToString();
                    var result = Command.SendData(files, usename, pass);
                    View.SetResult = result;
                }
            }
            catch (Exception e)
            {
                View.SetError = e;
            }
        }

        public void LoadConfig()
        {
            ThreadPool.QueueUserWorkItem(LoadConfigCallBack);
        }

        private void LoadConfigCallBack(object state)
        {
            var config = Command.GetProfile();
            View.SetConfig = config;
        }
    }
}
