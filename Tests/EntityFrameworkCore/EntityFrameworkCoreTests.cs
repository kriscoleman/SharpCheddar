using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using SharpCheddar.Core;
using SharpCheddar.EntityFrameworkCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests.EntityFrameworkCore
{
    /// <summary>
    /// Tests for the EntityFrameworkCore Repository
    /// </summary>
    public class EntityFrameworkCoreTests
    {
        private IRepository<MyModel, int> _myRepository;

        [SetUp]
        public void Setup()
        {
            // todo: set up DI when we have more concretes. 
            // the setup method of the tests should double as an example of how to set up the repository for new projects. 

            _myRepository = new EntityFrameworkCoreRepository<MyModel, int>(new TestDbContext());
        }

        [Test]
        public void ICanAddARecord() => Test(async () =>
        {
            var entity = new MyModel {CreatedOn = DateTime.UtcNow};
            await _myRepository.InsertOrUpdateAsync(entity);
           
            // we're kinda breaking IoC here, but its for the purpose of this test. normally you dont want to leak your abstraction. 
            await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.DbContext.SaveChangesAsync();

            // the id should be set by the DB and not be default now

            Assert.That(entity.Id != default(int));
        });

        [Test]
        public void ICanUpdateRecord() => Test(async () =>
        {
            var createdOn = DateTime.UtcNow;
            var entity = new MyModel {CreatedOn = createdOn};
            await _myRepository.InsertOrUpdateAsync(entity);
           
            // we're kinda breaking IoC here, but its for the purpose of this test. normally you dont want to leak your abstraction. 
            await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.DbContext.SaveChangesAsync();

            var entityReturned = await _myRepository.GetByIdAsync(entity.Id);
            Assert.That(entityReturned.CreatedOn, Is.EqualTo(createdOn));
           
            // we're kinda breaking IoC here, but its for the purpose of this test. normally you dont want to leak your abstraction. 
            await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.DbContext.SaveChangesAsync();

            var updatedDateTime = createdOn.AddYears(7);
            entityReturned.CreatedOn = updatedDateTime;
            Assert.That(entityReturned.CreatedOn, Is.EqualTo(updatedDateTime));
        });

        /// <summary>
        /// Tests the specified unit of work inside a transaction which get's rolled back, ensuring the work is Idempotent
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        private void Test(Action unitOfWork)
        {
            using (var transaction = (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.BeginTransaction(unitOfWork))
            {
                transaction?.Rollback();
            }
        }

        public class MyModel : IDataModel<int>
        {
            [Key]
            public int Id { get; set; }
            public DateTime CreatedOn { get; set; }
        }
    }
}