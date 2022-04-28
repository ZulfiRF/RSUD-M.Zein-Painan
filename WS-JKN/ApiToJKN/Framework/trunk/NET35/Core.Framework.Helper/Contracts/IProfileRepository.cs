namespace Core.Framework.Helper.Contracts
{
    public interface IProfileRepository
    {
        /// <summary>
        /// digunkan untuk mengambil Profile yang digunakan
        /// </summary>
        /// <returns></returns>
        ProfileItem CurrentProfile();

    }
}