using System.Collections;
using System.Collections.Generic;

namespace Core.Framework.Model.Contract
{
    /// <summary>
    ///     Interface ILinks
    /// </summary>
    /// <typeparam name="TLink">The type of the T link.</typeparam>
    public interface ILinks<TLink> : IEnumerable<TLink>
    {
    }

    /// <summary>
    ///     Interface ILinks
    /// </summary>
    public interface ILinks : IEnumerable
    {
    }
}