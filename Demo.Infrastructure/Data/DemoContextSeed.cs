using Demo.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Infrastructure.Data
{
    public class DemoContextSeed
    {
        public static async Task SeedAsync(DemoContext demoContext, ILoggerFactory loggerFactory, int? retry = 0)
        {
            if (retry != null)
            {
                var retryForAvailability = retry.Value;

                try
                {
                    // TODO: Only run this if using a real database
                    // demoContext.Database.Migrate();
                    // demoContext.Database.EnsureCreated();

                    if (!demoContext.Categories.Any())
                    {
                        await demoContext.Categories.AddRangeAsync(GetPreconfiguredCategories());
                        await demoContext.SaveChangesAsync();
                    }

                    if (!demoContext.Products.Any())
                    {
                        await demoContext.Products.AddRangeAsync(GetPreconfiguredProducts());
                        await demoContext.SaveChangesAsync();
                    }
                }
                catch (Exception exception)
                {
                    if (retryForAvailability < 10)
                    {
                        retryForAvailability++;
                        var log = loggerFactory.CreateLogger<DemoContextSeed>();
                        log.LogError(exception.Message);
                        await SeedAsync(demoContext, loggerFactory, retryForAvailability);
                    }
                    throw;
                }
            }
        }

        private static IEnumerable<Category> GetPreconfiguredCategories()
        {
            return new List<Category>()
            {
                new Category() { Name = "Phone", Description = "Phone Category"},
                new Category() { Name = "TV", Description = "TV Category"}
            };
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>()
            {
                new Product() { Name = "IPhone", CategoryId = 1 , UnitPrice = 19.5M , UnitsInStock = 10, QuantityPerUnit = "2", UnitsOnOrder = 1, ReorderLevel = 1, Discontinued = false },
                new Product() { Name = "Samsung", CategoryId = 1 , UnitPrice = 33.5M , UnitsInStock = 10, QuantityPerUnit = "2", UnitsOnOrder = 1, ReorderLevel = 1, Discontinued = false },
                new Product() { Name = "LG TV", CategoryId = 2 , UnitPrice = 33.5M , UnitsInStock = 10, QuantityPerUnit = "2", UnitsOnOrder = 1, ReorderLevel = 1, Discontinued = false }
            };
        }
    }
}
