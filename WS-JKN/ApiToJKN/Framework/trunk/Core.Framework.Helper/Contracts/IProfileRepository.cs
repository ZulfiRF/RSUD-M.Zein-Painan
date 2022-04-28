namespace Core.Framework.Helper.Contracts
{
    public interface IProfileRepository
    {
        #region Public Methods and Operators

        /// <summary>
        ///     digunkan untuk mengambil Profile yang digunakan
        /// </summary>
        /// <returns></returns>
        ProfileItem CurrentProfile();

        #endregion
    }
}