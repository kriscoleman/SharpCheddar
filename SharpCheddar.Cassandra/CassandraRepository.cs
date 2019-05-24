using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using SharpCheddar.Core;

namespace SharpCheddar.Cassandra
{
    public class CassandraRepository<T, TKey> : IRepository<T, TKey> where T : IDataModel<TKey>
    {
        private readonly string _clusterAddress;
        private readonly string _keySpace;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CassandraRepository{T, TKey}" /> class.
        /// </summary>
        /// <param name="clusterAddress">The cluster address.</param>
        /// <param name="keySpace">The key space.</param>
        public CassandraRepository(string clusterAddress, string keySpace)
        {
            _clusterAddress = clusterAddress;
            _keySpace = keySpace;
        }

        /// <summary>
        ///     Gets or sets the cluster.
        /// </summary>
        /// <value>
        ///     The cluster.
        /// </value>
        public Cluster Cluster { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Gets or sets the session.
        /// </summary>
        /// <value>
        ///     The session.
        /// </value>
        public ISession Session { get; private set; }

        /// <summary>
        ///     Gets or sets the table.
        /// </summary>
        /// <value>
        ///     The table.
        /// </value>
        public Table<T> Table { get; set; }

        /// <summary>
        ///     Initializes the cluster asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            Cluster = Cluster.Builder().AddContactPoint(_clusterAddress).Build();
            Session = await Cluster.ConnectAsync(_keySpace);
            Table = new Table<T>(Session);

            IsInitialized = true;
        }

        /// <summary>
        ///     Deletes the entity asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task DeleteAsync(TKey id)
        {
            await this.CheckIfInitializedAsync();
            await Table.Where(x => x.Id.Equals(id)).Delete().ExecuteAsync();
        }

        /// <summary>
        ///     Gets all entities asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<T>> GetAllAsync()
        {
            await this.CheckIfInitializedAsync();
            return Table.AsQueryable();
        }

        /// <summary>
        ///     Gets the entities asynchronously.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public async Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            await this.CheckIfInitializedAsync();
            return (await Table.Where(predicate).ExecuteAsync()).AsQueryable();
        }

        /// <summary>
        ///     Gets the entity by identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(TKey id)
        {
            await this.CheckIfInitializedAsync();
            return (await Table.Where(x => x.Id.Equals(id)).ExecuteAsync()).SingleOrDefault();
        }

        /// <summary>
        /// Inserts or updates the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task InsertOrUpdateAsync(T entity)
        {
            await this.CheckIfInitializedAsync();
            if (entity.Id.Equals(default(TKey))) await Table.Insert(entity).ExecuteAsync();
            else await Table.Where(x => x.Id.Equals(entity.Id)).Select(x => entity).Update().ExecuteAsync();
        }
    }
}