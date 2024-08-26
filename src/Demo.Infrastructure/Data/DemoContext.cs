using Demo.Core.Entities;
using Demo.Core.Models;
using Microsoft.EntityFrameworkCore;
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

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Created)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Created)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_Category");
            });
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
                    DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Write),
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
                    DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Read),
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
                    DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Write),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: new int[] { }
                    )).EnableDetailedErrors();
            }
        }
    }
}
