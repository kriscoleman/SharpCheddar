using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Cassandra.Common;

namespace Tests.Cassandra
{
    /// <summary>
    ///     Tests for the Cassandra Repository
    /// </summary>
    [TestFixture]
    public sealed class CassandraTests : SimpleCRUDTestsBase<TestModule>
    {
        public override Task TearDown() => Task.CompletedTask;
    }
}