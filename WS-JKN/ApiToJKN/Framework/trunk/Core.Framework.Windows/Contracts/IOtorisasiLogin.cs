namespace Core.Framework.Windows.Contracts
{
    public interface IOtorisasiLogin
    { 
        bool isHavingAccess(string namaUserIntervace, bool newLogin = false);  
    }
}
