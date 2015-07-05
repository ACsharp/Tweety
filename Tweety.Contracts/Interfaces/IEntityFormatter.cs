using System;
namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of objects that can render an entity in a textual human-readable form 
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to render</typeparam>
    public interface IEntityFormatter<TEntity>
    {
        /// <summary>
        /// Renders the provided entity in a textual human-readable form
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string Format(TEntity entity);
    }
}
