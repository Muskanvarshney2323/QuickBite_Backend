using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.Cart.Application.Interfaces;
using QuickBite.Cart.Application.Services;
using QuickBite.Cart.Infrastructure.Data;
using QuickBite.Cart.Infrastructure.Repositories;

namespace QuickBite.Cart.Infrastructure.DependencyInjection;

/// <summary>
/// Registers infrastructure and application services.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}