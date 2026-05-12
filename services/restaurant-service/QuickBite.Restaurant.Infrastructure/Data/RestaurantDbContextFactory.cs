using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QuickBite.Restaurant.Infrastructure.Data
{
    public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
    {
        public RestaurantDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../QuickBite.Restaurant.API"
            );

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder =
                new DbContextOptionsBuilder<RestaurantDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new RestaurantDbContext(optionsBuilder.Options);
        }
    }
}