using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharpCheddar.Core;
using SharpCheddar.EntityFrameworkCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests.EntityFrameworkCore
{
    /// <summary>
    ///     Tests for the EntityFrameworkCore Repository
    /// </summary>
    [TestFixture]
    public class EntityFrameworkCoreTests
    {
        private IRepository<MyModel, int> _myRepository;

        /// <summary>
        /// Set to false to disable the initialization of the repository (for fail-testing for instance)
        /// </summary>
        private bool _initialize; 

        /// <summary>
        /// Runs before every test.
        /// Creates the database and ensures its migrated and seeded.
        /// </summary>
        /// <returns></returns>
        [SetUp]
        public async Task Setup()
        {
            // todo: set up DI when we have more concretes. 
            // the setup method of the tests should double as an example of how to set up the repository for new projects. 

            _initialize = true;
            var entityFrameworkCoreRepository = new EntityFrameworkCoreRepository<MyModel, int>(new TestDbContext());
            _myRepository = entityFrameworkCoreRepository;

            // Alternatively, you can use context.Database.EnsureCreated() to create a new database containing the seed data,
            // for example for a test database or when using the in-memory provider or any non-relation database.
            // Note that if the database already exists, EnsureCreated() will neither update the schema nor seed data in the database.
            // For relational databases you shouldn't call EnsureCreated() if you plan to use Migrations.
            // https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding

            await entityFrameworkCoreRepository.DbContext.Database.MigrateAsync();
            await entityFrameworkCoreRepository.DbContext.Database.EnsureCreatedAsync();
        }

        /// <summary>
         /// Ensures that the database is deleted after testing, leaving no trace. 
         /// </summary>
         /// <returns></returns>
        [TearDown]
        public async Task TearDown()
        {
            await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>).DbContext.Database
                .EnsureDeletedAsync();
        }

        [Test]
        public Task ANullDbContextThrowAnException()
        {
            Assert.Throws<ArgumentNullException>(() => new EntityFrameworkCoreRepository<MyModel, int>(null));
            return Task.CompletedTask;
        }

        [Test]
        public Task CallsMadeToAnUninitializedRepositoryWillThrowAnException()
        {
            _initialize = false;

            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanAddARecord(); });
            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanDeleteARecord(); });
            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanGetAllRecords(); });
            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanGetById(); });
            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanGetRecords(); });
            Assert.ThrowsAsync<SharpCheddarInitializationException>(async () => { await ICanUpdateRecord(); });
            return Task.CompletedTask;
        }

        [Test]
        public async Task ICanAddARecord() => await Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);

            // the id should be set by the DB and not be default now
            Assert.That(entity.Id != default(int));
        });

        [Test]
        public async Task ICanDeleteARecord() => await Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);

            // the id should be set by the DB and not be default now
            Assert.That(entity.Id != default(int));
            var id = entity.Id;

            await _myRepository.DeleteAsync(id);

            var results = (await _myRepository.GetAsync(x => x.Id == id)).ToList();
            Assert.That(results, Is.Empty);
        });

        [Test]
        public async Task ICanGetAllRecords() => await Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);

            // the id should be set by the DB and not be default now
            Assert.That(entity.Id != default(int));

            var results = (await _myRepository.GetAllAsync()).ToList();
            Assert.That(results, Has.Count.EqualTo(2),
                "We expected to only have two entities saved in db, one seed, and one we just created, but we found more or less than these 2.");
        });

        [Test]
        public async Task ICanGetById()
        {
            await InitializeIfNeededAsync();
            var result = await _myRepository.GetByIdAsync(MyModel.SeedEntityId);
            Assert.That(result.CreatedOn, Is.EqualTo(DateTime.MinValue), "We couldn't find the seed entity in our db.");
        }

        [Test]
        public async Task ICanGetRecords() => await Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);

            // the id should be set by the DB and not be default now
            Assert.That(entity.Id != default(int));

            var results = (await _myRepository.GetAsync(x => x.CreatedOn > DateTime.MinValue)).ToList();
            Assert.That(results, Has.Count.EqualTo(1),
                "We expected to only have one entities saved in db, the one we just created, but we found more or less than these 1. It should have been one because we filtered out the seed.");
        });

        [Test]
        public async Task ICanUpdateRecord() => await Test(async () =>
        {
            var createdOn = DateTime.UtcNow;
            var entity = new MyModel {CreatedOn = createdOn};
            await _myRepository.InsertOrUpdateAsync(entity);

            var entityReturned = await _myRepository.GetByIdAsync(entity.Id);
            Assert.That(entityReturned.CreatedOn, Is.EqualTo(createdOn));

            var updatedDateTime = createdOn.AddYears(7);
            entityReturned.CreatedOn = updatedDateTime;

            await _myRepository.InsertOrUpdateAsync(entityReturned);

            var entithReturnedAgain = await _myRepository.GetByIdAsync(entity.Id);
            Assert.That(entithReturnedAgain.CreatedOn, Is.EqualTo(updatedDateTime),
                "We expected the CreatedOn date to update but it did not.");
        });

        /// <summary>
        /// Initializes the context if initialize is true
        /// </summary>
        /// <param name="initialize"></param>
        /// <returns></returns>
        private async Task InitializeIfNeededAsync()
        {
            if (_initialize)
                await _myRepository.InitializeAsync();
        }

        /// <summary>
        ///     Tests the specified unit of work inside a transaction which get's rolled back, ensuring the work is Idempotent
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        private async Task Test(Func<Task> unitOfWork)
        {
            await InitializeIfNeededAsync();

            using (var transaction =
                await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.BeginTransactionAsync(unitOfWork))
            {
                transaction?.Rollback();
            }
        }

        public class MyModel : IDataModel<int>
        {
            public const int SeedEntityId = 1;
            public DateTime CreatedOn { get; set; }

            [Key] public int Id { get; set; }
        }
    }
}