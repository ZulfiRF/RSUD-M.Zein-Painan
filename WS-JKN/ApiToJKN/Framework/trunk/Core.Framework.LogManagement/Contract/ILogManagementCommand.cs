using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.LogManagement.ViewModel;

namespace Core.Framework.LogManagement.Contract
{
    public interface ILogManagementCommand
    {
        List<FileLog> GetData();
        bool SendData(List<FileLog> fileLogs, string userName, string password);
        string[] GetProfile();
    }
}
