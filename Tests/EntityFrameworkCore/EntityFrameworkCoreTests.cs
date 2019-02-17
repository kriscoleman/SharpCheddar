using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharpCheddar.Core;
using SharpCheddar.EntityFrameworkCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests.EntityFrameworkCore
{
    /// <summary>
    /// Tests for the EntityFrameworkCore Repository
    /// </summary>
    [TestFixture]
    public class EntityFrameworkCoreTests
    {
        private IRepository<MyModel, int> _myRepository;

        [SetUp]
        public async Task Setup()
        {
            // todo: set up DI when we have more concretes. 
            // the setup method of the tests should double as an example of how to set up the repository for new projects. 

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

        [TearDown]
        public async Task TearDown() => await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>).DbContext.Database.EnsureDeletedAsync();

        [Test]
        public async Task ICanAddARecord() => await Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);

            // the id should be set by the DB and not be default now
            Assert.That(entity.Id != default(int));
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
            Assert.That(entithReturnedAgain.CreatedOn, Is.EqualTo(updatedDateTime));
        });

        // todo: get, getbyid, delete, getall.

        /// <summary>
        /// Tests the specified unit of work inside a transaction which get's rolled back, ensuring the work is Idempotent
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        private async Task Test(Func<Task> unitOfWork)
        {
            using (var transaction = await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.BeginTransactionAsync(unitOfWork))
                transaction?.Rollback();
        }

        public class MyModel : IDataModel<int>
        {
            [Key]
            public int Id { get; set; }
            public DateTime CreatedOn { get; set; }
        }
    }
}