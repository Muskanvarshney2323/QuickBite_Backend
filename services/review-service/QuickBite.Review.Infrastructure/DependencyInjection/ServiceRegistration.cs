using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.Review.Application.Interfaces;
using QuickBite.Review.Application.Services;
using QuickBite.Review.Infrastructure.Data;
using QuickBite.Review.Infrastructure.Repositories;

namespace QuickBite.Review.Infrastructure.DependencyInjection;

/// <summary>
/// Registers the DbContext, repository, and application service with DI.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connect to PostgreSQL using the connection string from appsettings.json.
        services.AddDbContext<ReviewDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register the repository and service.
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}
