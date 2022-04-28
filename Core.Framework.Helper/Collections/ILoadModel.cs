using System.Data;

namespace Core.Framework.Helper.Collections
{
    public interface ILoadModel
    {
        bool IsLoad { get; set; }
        void OnInitLoad(IDataRecord read);
        bool Skip { get; set; }
    }
}