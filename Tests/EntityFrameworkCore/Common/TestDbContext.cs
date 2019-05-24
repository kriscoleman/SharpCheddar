using System;
using Microsoft.EntityFrameworkCore;

namespace Tests.EntityFrameworkCore.Common
{
    class TestDbContext : DbContext
    {
        public DbSet<MyModel> MyModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(ConfigHelper.Configuration.GetSection("efCore")["connectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // we'll seed this data for use in queries in our testing
            modelBuilder.Entity<MyModel>()
                .HasData(new MyModel {CreatedOn = DateTime.MinValue, Id = MyModel.SeedEntityId});
        }
    }
}
