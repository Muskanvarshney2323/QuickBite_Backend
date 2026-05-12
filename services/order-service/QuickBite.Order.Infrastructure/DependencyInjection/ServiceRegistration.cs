using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Application.Services;
using QuickBite.Order.Infrastructure.Data;
using QuickBite.Order.Infrastructure.ExternalServices;
using QuickBite.Order.Infrastructure.Repositories;

namespace QuickBite.Order.Infrastructure.DependencyInjection;

/// <summary>
/// Registers Order-Service infrastructure (DbContext, repositories, stub adapters)
/// and application services.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL via Npgsql, matching the rest of the QuickBite services.
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();

        // Stubs for cross-service integrations. Swap these for HTTP-backed
        // adapters when Payment-Service and Delivery-Service are wired in.
        services.AddHttpClient<IPaymentGateway, HttpPaymentGateway>();
        services.AddScoped<IDeliveryDispatcher, StubDeliveryDispatcher>();

        return services;
    }
}
