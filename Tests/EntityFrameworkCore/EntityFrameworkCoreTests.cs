using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharpCheddar.EntityFrameworkCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests.EntityFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    ///     Tests for the EntityFrameworkCore Repository
    /// </summary>
    [TestFixture]
    public sealed class EntityFrameworkCoreTests : SimpleCRUDTestsBase
    {
        public override async Task Setup()
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

        public override async Task TearDown()
        {
            await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>).DbContext.Database
                .EnsureDeletedAsync();
        }

        protected override async Task Test(Func<Task> unitOfWork)
        {
            await base.Test(unitOfWork);

            using (var transaction =
                await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.BeginTransactionAsync(unitOfWork))
            {
                transaction?.Rollback();
            }
        }

        [Test]
        public Task ANullDbContextThrowAnException()
        {
            Assert.Throws<ArgumentNullException>(() => new EntityFrameworkCoreRepository<MyModel, int>(null));
            return Task.CompletedTask;
        }
    }
}