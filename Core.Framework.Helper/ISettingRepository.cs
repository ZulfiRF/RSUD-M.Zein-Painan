using System.Collections.Generic;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Helper
{

    public interface ISettingRepository : IProfileRepository
    {
        /// <summary>
        /// digunakan untuk mengambil data connection string dari sebuah repository
        /// </summary>
        /// <returns></returns>
        ConnectionDomain GetIdentity(string projectName);

        /// <summary>
        /// digunakan untuk mengambil data connection string dari sebuah repository
        /// </summary>
        /// <returns></returns>
        void SetIdentity(string projectName, ConnectionDomain model);

        /// <summary>
        /// digunakan untuk mengetahui jika connection telah benar atau tidak
        /// </summary>
        /// <returns></returns>
        bool CekConnection();

        /// <summary>
        /// digunakan untuk mengetahui jika connection telah benar atau tidak
        /// </summary>
        /// <returns></returns>
        bool CekConnection(ConnectionDomain model);

        /// <summary>
        /// digunkan untuk mengambil database dari sebuah repository
        /// </summary>
        /// <returns></returns>
        IEnumerable<DatabaseItem> GetDatabase(ConnectionDomain connection=null);

        /// <summary>
        /// digunkan untuk mengambil List Module yang terdaftar
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectItem> GetProject();

        /// <summary>
        /// digunkan untuk mengambil Project yang digunakan
        /// </summary>
        /// <returns></returns>
        ProjectItem CurrentProject();



        /// <summary>
        /// digunkan untuk mengambil Modul yang digunakan
        /// </summary>
        /// <returns></returns>
        ModuleItem CurrentModule();

        /// <summary>
        /// digunkan untuk mengambil Connection String
        /// </summary>
        /// <returns></returns>
        string ConnectionString { get; }


    }
}
