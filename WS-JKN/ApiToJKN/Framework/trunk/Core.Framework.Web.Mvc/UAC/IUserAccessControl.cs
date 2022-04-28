namespace Core.Framework.Web.Mvc.UAC
{
    /// <summary>
    /// Interface IUserAccessControl
    /// </summary>
    public interface IUserAccessControl
    {
        /// <summary>
        /// Validates the specified module. digunakan untuk validate module dan user id
        /// </summary>
        /// <param name="module">berisikan module id yang access oleh user</param>
        /// <param name="userId">berisikan user id user</param>
        /// <returns><c>true</c>jika user akses, <c>false</c> jika user tidak memiliki akses</returns>
        bool Validate(string module, string userId);
    }
}