namespace Core.Framework.Helper.Commands
{
    using System;
    using System.Collections.Generic;

    public interface ICreateUpdateDeleteCommand
    {
    }

    public interface ICreateUpdateDeleteCommand<in TDomain> : ICreateUpdateDeleteCommand
        where TDomain : class
    {
        #region Public Methods and Operators

        /// <summary>
        ///     digunakan untuk melakukan penghapusan sebuah domain
        /// </summary>
        /// <param name="domains"></param>
        /// <returns></returns>
        Exception DeleteDomain(IEnumerable<TDomain> domains);

        /// <summary>
        ///     digunakan untuk melakukan penyimpanan kedalam repository
        /// </summary>
        /// <param name="domain">domain yang akan di simpan</param>
        /// <returns></returns>
        Exception SaveDomain(TDomain domain);

        /// <summary>
        ///     digunakan untuk melakukan perubahan
        /// </summary>
        /// <param name="domainOld">domain sebelum ada perubahan</param>
        /// <returns></returns>
        Exception UpdateDomain(TDomain domainOld);

        #endregion
    }
}