using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpCheddar.Core
{
    /// <summary>
    /// Your Repositories should not be used directly and expose IQueryables. Instead, wrap them in an IRepositoryService.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IRepositoryService<T, in TKey> where T: IDataModel<TKey>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        Task DeleteAsync(TKey id);

        /// <summary>
        /// Gets results with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A collection of T</returns>
        Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets all. This can be expensive on big repositories. I suggest not depending on GetAllAsync, unless you need to. 
        /// Caching and Paging should probably be implemented on large repositories.
        /// </summary>
        /// <returns></returns>
        Task<ICollection<T>> GetAllAsync();

        /// <summary>
        /// Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The entity</returns>
        Task<T> GetByIdAsync(TKey id);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Inserts or updates the Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task InsertOrUpdateAsync(T entity);
    }
}