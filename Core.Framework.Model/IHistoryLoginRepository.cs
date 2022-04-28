namespace Core.Framework.Model
{
    public interface IHistoryLoginRepository
    {
        int KodeHistroyLogin { get; set; }
        string Module { get; set; }
        string Ruangan { get; set; }
        string Departemen { get; set; }
    }
}