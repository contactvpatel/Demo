using Demo.Core.Entities;
using Demo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Demo.Infrastructure.Data
{
    public class DemoContext : DbContext
    {
        public DemoContext()
        {

        }

        public DemoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>(ConfigureProduct);
            builder.Entity<Category>(ConfigureCategory);
        }

        private void ConfigureProduct(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(ci => ci.ProductId);

            builder.Property(cb => cb.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cb => cb.UnitPrice)
                .HasColumnType("decimal(18,0)");
        }

        private void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(ci => ci.CategoryId);

            builder.Property(cb => cb.Name)
                .IsRequired()
                .HasMaxLength(100);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var databaseConnectionSettings = new DbConnectionModel();
                configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

                optionsBuilder.UseSqlServer(
                    databaseConnectionSettings.CreateConnectionString(databaseConnectionSettings.Write),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: new int[] { }
                    )).EnableDetailedErrors();
            }
        }
    }

    public class DemoReadContext : DemoContext
    {
        public DemoReadContext()
        {

        }

        public DemoReadContext(DbContextOptions<DemoContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only. Please use write database context.");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This context is read-only. Please use write database context.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var databaseConnectionSettings = new DbConnectionModel();
                configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

                optionsBuilder.UseSqlServer(
                    databaseConnectionSettings.CreateConnectionString(databaseConnectionSettings.Read),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: new int[] { }
                    )).EnableDetailedErrors();
            }
        }
    }

    public class DemoWriteContext : DemoContext
    {
        public DemoWriteContext()
        {

        }
        public DemoWriteContext(DbContextOptions<DemoContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var databaseConnectionSettings = new DbConnectionModel();
                configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

                optionsBuilder.UseSqlServer(
                    databaseConnectionSettings.CreateConnectionString(databaseConnectionSettings.Write),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: new int[] { }
                    )).EnableDetailedErrors();
            }
        }
    }
}
