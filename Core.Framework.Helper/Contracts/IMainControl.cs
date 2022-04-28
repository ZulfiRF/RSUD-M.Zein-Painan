namespace Core.Framework.Helper.Contracts
{
    public interface IMainControl
    {
        /// <summary>
        /// untuk menampilkan view di dasboard menu windows
        /// </summary>
        /// <param name="sender">framework element</param>
        /// <param name="padding">kiri / kanan</param>
        void SetContent(object sender, Padding padding);        
    }
}