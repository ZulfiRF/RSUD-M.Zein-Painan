using Core.Framework.Helper.Collections;

namespace Core.Framework.Helper.Contracts
{
    public interface IHierarchyItem<T> : IHierarchyItem where T : class, ILoadModel
    {
        ICoreQueryable<T> GetChildren { get; }
    }
    public interface IFirstLoad
    {
        #region Public Methods and Operators

        void Initialize();

        #endregion
    }
}