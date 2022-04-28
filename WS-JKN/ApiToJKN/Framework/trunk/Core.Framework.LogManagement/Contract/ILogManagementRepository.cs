using System.Collections.Generic;
using Core.Framework.LogManagement.ViewModel;

namespace Core.Framework.LogManagement.Contract
{
    public interface ILogManagementRepository
    {
        bool SendData(List<FileLog> fileLogs, string userName, string password);
        List<FileLog> GetData();
        string[] LoadConfig();
    }
}
