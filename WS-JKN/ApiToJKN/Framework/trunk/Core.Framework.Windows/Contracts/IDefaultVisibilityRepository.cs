using System.Collections.Generic;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Contracts
{
    public interface IDefaultVisibilityRepository
    {
        IEnumerable<VisibilityControl> GetVisibilityControlsByName(string name);
    }
}
