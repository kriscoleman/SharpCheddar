using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpCheddar.Core
{
    /// <summary>
    /// An implementation of the Repository Pattern 
    /// Follows the Async/Await pattern 
    /// </summary>
    /// <typeparam name="T">The type of Model you are storing.</typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<T, in TKey> where T : IDataModel<TKey>
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
        Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets all. . 
        /// Caching and Paging should probably be implemented on large repositories.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> GetAllAsync();

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