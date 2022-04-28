using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Core.Framework.Helper;
using Core.Framework.LogManagement.Contract;
using Core.Framework.LogManagement.ViewModel;

namespace Core.Framework.LogManagement.Command
{
    public class LogManagementCommand : ILogManagementCommand
    {
        public List<FileLog> GetData()
        {
            return BaseDependency.Get<ILogManagementRepository>().GetData();
        }

        public bool SendData(List<FileLog> fileLogs, string userName, string password)
        {
            return BaseDependency.Get<ILogManagementRepository>().SendData(fileLogs, userName, password);
        }

        public string[] GetProfile()
        {
            return BaseDependency.Get<ILogManagementRepository>().LoadConfig();
        }
    }
}
