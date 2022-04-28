namespace Core.Framework.Windows.Contracts
{
    public interface IControlAuthentication
    {
        /// <summary>
        /// Flag yang di gunakan untuk mengetahui Control itu harus di authentication atau tidak
        /// </summary>
        bool UseAuthentication { get; set; }
        /// <summary>
        /// Method yang di eksekusi ketika Control memiliki flag true    
        /// </summary>
        /// <returns></returns>
        bool ExecuteAuthentication();
        /// <summary>
        /// Identity dari control
        /// </summary>
        string IdentityAuthentication { get; }
    }
}