using System;
using Microsoft.EntityFrameworkCore;

namespace Tests.EntityFrameworkCore.Common
{
    class TestDbContext : DbContext
    {
        public DbSet<EntityFrameworkCoreTests.MyModel> MyModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // for CI testing

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(
                "Data Source=sql;Database=SharpCheddar.EfCoreTests;user=sa;password=SomeStrong!Passw0rd;Connect Timeout=30;Encrypt=False;");

            // below is for local dev

            //optionsBuilder.UseSqlServer(
            //    "Data Source=localhost, 5100;Database=SharpCheddar.EfCoreTests;user=sa;password=SomeStrong!Passw0rd;Connect Timeout=30;Encrypt=False;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // we'll seed this data for use in queries in our testing
            modelBuilder.Entity<EntityFrameworkCoreTests.MyModel>()
                .HasData(new EntityFrameworkCoreTests.MyModel {CreatedOn = DateTime.MinValue, Id = EntityFrameworkCoreTests.MyModel.SeedEntityId});
        }
    }
}
