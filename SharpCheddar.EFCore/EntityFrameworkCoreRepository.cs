using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharpCheddar.Core;

namespace SharpCheddar.EFCore
{
    public class EntityFrameworkCoreRepository<T, TKey> : IRepository<T, TKey> where T : IDataModel<TKey>
    {
        /// <summary>
        /// Gets the test database context.
        /// This has been exposed for convenience, but be warned: using this in your code violates IoC and breaks the encapsulation of your repositories.
        /// Use this at your own risk. 
        /// </summary>
        /// <value>
        /// The test database context.
        /// </value>
        internal DbContext DbContext { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkCoreRepository{T, TKey}"/> class.
        /// Requires a EF Database Context to be instantiated.
        /// This should be injected by your DI container in a real project,
        /// or a fake/mock context can be injected for a test project.
        /// </summary>
        /// <param name="dbContext">The test database context.</param>
        public EntityFrameworkCoreRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// Some repos may need to be initialized first.
        /// The EF Core repo is always Initialized. 
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; } = true;

        public Task DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public Task InsertOrUpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public IDbContextTransaction BeginTransaction(Action action)
        {
            var transaction = DbContext.Database.BeginTransaction();
            action.Invoke();
            return transaction;
        }
    }
}