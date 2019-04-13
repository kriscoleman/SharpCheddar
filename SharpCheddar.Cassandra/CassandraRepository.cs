using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharpCheddar.Core;

namespace SharpCheddar.Cassandra
{
    public class CassandraRepository<T, TKey> : IRepository<T, TKey> where T : IDataModel<TKey>
    {
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

        public bool IsInitialized { get; }
    }
}