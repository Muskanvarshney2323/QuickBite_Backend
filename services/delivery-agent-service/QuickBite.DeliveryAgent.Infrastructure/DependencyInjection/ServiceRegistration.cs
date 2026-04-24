using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.DeliveryAgent.Application.Interfaces;
using QuickBite.DeliveryAgent.Application.Services;
using QuickBite.DeliveryAgent.Infrastructure.Data;
using QuickBite.DeliveryAgent.Infrastructure.Repositories;

namespace QuickBite.DeliveryAgent.Infrastructure.DependencyInjection;

/// <summary>
/// Registers the DbContext, repository, and application service with DI.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connect to PostgreSQL using the connection string from appsettings.json.
        services.AddDbContext<DeliveryAgentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register the repository and service.
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IDeliveryService, DeliveryService>();

        return services;
    }
}
