using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.Payment.Application.Interfaces;
using QuickBite.Payment.Application.Services;
using QuickBite.Payment.Infrastructure.Data;
using QuickBite.Payment.Infrastructure.ExternalServices;
using QuickBite.Payment.Infrastructure.Repositories;

namespace QuickBite.Payment.Infrastructure.DependencyInjection;

/// <summary>
/// Registers Payment-Service infrastructure (DbContext, repositories, gateway stub)
/// and application services.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL via Npgsql, matching the rest of the QuickBite services.
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IPaymentService, PaymentService>();

        // Stub gateway client. Swap for a Razorpay/Stripe-backed adapter
        // when the gateway is wired in.
        services.AddScoped<IPaymentGatewayClient, StubPaymentGatewayClient>();

        return services;
    }
}
