using System.Data.Common;

namespace DbContext
{
    public interface IConnection
    {
        string CommandText { get; set; }
        string CommandReader { get; set; }
        void ExecuteNonQuery();
        void BeginTransaction();
        void Commit();
        void RollBack();
        void Close();
        void CloseReader();
        void Open();
        string FormateDate { get; }
        DbDataReader ExecuteReader();
    }
}

