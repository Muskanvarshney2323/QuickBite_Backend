using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.Notification.Application.Interfaces;
using QuickBite.Notification.Application.Services;
using QuickBite.Notification.Infrastructure.Data;
using QuickBite.Notification.Infrastructure.Repositories;

namespace QuickBite.Notification.Infrastructure.DependencyInjection;

/// <summary>
/// Registers the DbContext, repository, and application service with DI.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connect to PostgreSQL using the connection string from appsettings.json.
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register the repository and service.
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
