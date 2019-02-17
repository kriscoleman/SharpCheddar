using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharpCheddar.Core;

namespace SharpCheddar.EntityFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    ///     A repository for EntityFrameworkCore
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="T:SharpCheddar.Core.IRepository`2" />
    public class EntityFrameworkCoreRepository<T, TKey> : IRepository<T, TKey> where T : class, IDataModel<TKey>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityFrameworkCoreRepository{T, TKey}" /> class.
        ///     Requires a EF Database Context to be instantiated.
        ///     This should be injected by your DI container in a real project,
        ///     or a fake/mock context can be injected for a test project.
        /// </summary>
        /// <param name="dbContext">The test database context.</param>
        public EntityFrameworkCoreRepository(DbContext dbContext)
        {   
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        ///     Gets the test database context.
        ///     This has been exposed for convenience, but be warned: using this in your code violates IoC and breaks the
        ///     encapsulation of your repositories.
        ///     Use this at your own risk.
        /// </summary>
        /// <value>
        ///     The test database context.
        /// </value>
        public DbContext DbContext { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        ///     Some repos may need to be initialized first.
        ///     The EF Core repo is always Initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Starts a transacation and returns it so you can control the transaction.
        ///     Please note that this method is not part of the core interface, so using it will require you to break IoC.
        ///     But it can be useful when you know your type is a efcore repo, and you want to control the transaction, like during
        ///     testing.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(Func<Task> action)
        {
            await CheckIfInitializedAsync();

            var transaction = await DbContext.Database.BeginTransactionAsync();
            await action.Invoke();
            return transaction;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteAsync(TKey id)
        {
            await CheckIfInitializedAsync();
            var entity = await GetByIdAsync(id);
            DbContext.Set<T>().Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all. 
        /// Caching and Paging should probably be implemented on large repositories.
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<T>> GetAllAsync()
        {
            await CheckIfInitializedAsync();
            return DbContext.Set<T>();
        }

        /// <summary>
        /// Gets results with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// A collection of T
        /// </returns>
        public async Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            await CheckIfInitializedAsync();
            return DbContext.Set<T>().Where(predicate);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///     The entity
        /// </returns>
        public async Task<T> GetByIdAsync(TKey id)
        {
            await CheckIfInitializedAsync();
            return await DbContext.FindAsync<T>(id);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            IsInitialized = await DbContext.Database.CanConnectAsync();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Inserts or updates the Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task InsertOrUpdateAsync(T entity)
        {
            await CheckIfInitializedAsync();

            if (entity.Id.Equals(default(TKey)))
            {
                if (DbContext != null) await DbContext?.AddAsync(entity);
            }
            else
            {
                DbContext?.Update(entity);
            }

            if (DbContext != null) await DbContext?.SaveChangesAsync();
        }

        /// <summary>
        ///     Checks if initialized.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SharpCheddarInitializationException"></exception>
        private Task CheckIfInitializedAsync()
        {
            if (!IsInitialized) throw new SharpCheddarInitializationException();
            return Task.CompletedTask;
        }
    }
}