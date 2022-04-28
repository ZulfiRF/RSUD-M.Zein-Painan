using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Helper.Presenters;
using Core.Framework.LogManagement.Presenter;
using Core.Framework.LogManagement.ViewModel;

namespace Core.Framework.LogManagement.Contract
{
    public interface ILogManagementView : IAttachPresenter<LogManagementPresenter>
    {
        List<FileLog> SetLogSource { set; }
        List<FileLog> CurrentFileSend { get; }
        bool SetResult { set; }
        Exception SetError { set; }
        string CurrentUsername { get; }
        string CurrentPassword { get; }
        string[] SetConfig { set; }
    }
}
