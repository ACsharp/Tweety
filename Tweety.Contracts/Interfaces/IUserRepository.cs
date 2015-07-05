using System;

namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of service classes that represent a repository of type TEntity
    /// (including their navigation properties)
    /// </summary>
    /// <typeparam name="TEntity">Type of the entities managed by the repository</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Gets the entityt matching the provided key.
        /// If no entity exists with such key, a new entity is added to the repository and returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TEntity GetOrCreate(string key);
    }
}
