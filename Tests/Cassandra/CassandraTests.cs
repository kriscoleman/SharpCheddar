using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharpCheddar.Core;
using SharpCheddar.EntityFrameworkCore;
using Tests.EntityFrameworkCore;
using Tests.EntityFrameworkCore.Common;

namespace Tests.Cassandra
{
    /// <summary>
    ///     Tests for the Cassandra Repository
    /// </summary>
    [TestFixture]
    public class CassandraTests : SimpleCRUDTestsBase
    {
        public override Task Setup()
        {
            throw new NotImplementedException();
        }

        public override Task TearDown()
        {
            throw new NotImplementedException();
        }
    }
}