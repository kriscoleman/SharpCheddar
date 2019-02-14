using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using SharpCheddar.Core;
using SharpCheddar.EFCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests
{
    /// <summary>
    /// Tests for the EntityFrameworkCore Repository
    /// </summary>
    public class EntityFrameworkCoreTests
    {
        private EntityFrameworkCoreRepository<MyModel, int> _myRepository;

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

            // the id should be set by the DB and not be default now

            Assert.That(entity.Id != default(int));
        });

        /// <summary>
        /// Tests the specified unit of work inside a transaction which get's rolled back, ensuring the work is Idempotent
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        private void Test(Action unitOfWork)
        {
            using (var transaction = _myRepository.BeginTransaction(unitOfWork))
            {
                transaction.Rollback();
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