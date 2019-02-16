using Microsoft.EntityFrameworkCore;

namespace Tests.EntityFrameworkCore.Common
{
    class TestDbContext : DbContext
    {
        public DbSet<EntityFrameworkCoreTests.MyModel> MyModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
