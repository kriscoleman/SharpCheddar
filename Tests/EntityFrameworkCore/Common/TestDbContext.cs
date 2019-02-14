using Microsoft.EntityFrameworkCore;

namespace Tests.EntityFrameworkCore.Common
{
    class TestDbContext : DbContext
    {
        public DbSet<EntityFrameworkCoreTests.MyModel> MyModels { get; set; }
    }
}
