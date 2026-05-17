// JWT Bearer authentication scheme
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Entity Framework Core for database operations
using Microsoft.EntityFrameworkCore;

// JWT token validation and security
using Microsoft.IdentityModel.Tokens;

// OpenAPI/Swagger documentation models
using Microsoft.OpenApi.Models;

// Imports service interfaces
using QuickBite.Auth.Application.Interfaces;

// Imports service implementations
using QuickBite.Auth.Application.Services;

// Imports database context
using QuickBite.Auth.Infrastructure.Context;

// Imports repository implementations
using QuickBite.Auth.Infrastructure.Repositories;

// Imports security implementations
using QuickBite.Auth.Infrastructure.Security;

// Text encoding for UTF-8
using System.Text;

// Namespace for extension methods
namespace QuickBite.Auth.API.Extensions
{
    // ServiceExtensions: Static class containing extension methods for IServiceCollection
    // Used in Program.cs to register and configure services (Dependency Injection)
    public static class ServiceExtensions
    {
        // ========================= ADD APPLICATION SERVICES =========================

        // Method: AddApplicationServices - Extension method to register all application services
        // Called in Program.cs: services.AddApplicationServices(configuration)
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ========================= ADD DATABASE CONTEXT =========================

            // Register AuthDbContext for database operations
            services.AddDbContext<AuthDbContext>(options =>
                // Configure Entity Framework to use PostgreSQL database
                // "DefaultConnection" read from appsettings.json
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            // ========================= REGISTER REPOSITORIES =========================

            // Register UserRepository as implementation of IUserRepository
            // Scoped: New instance for each HTTP request
            // Allows AuthService to inject IUserRepository
            services.AddScoped<IUserRepository, UserRepository>();

            // ========================= REGISTER SERVICES =========================

            // Register JwtTokenGenerator as implementation of IJwtTokenGenerator
            // Scoped: New instance for each HTTP request
            // Allows AuthService and AuthController to inject IJwtTokenGenerator
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Register AuthService as implementation of IAuthService
            // Scoped: New instance for each HTTP request
            // Allows AuthController to inject IAuthService
            services.AddScoped<IAuthService, AuthService>();

            // Return services collection for chaining
            return services;
        }

        // ========================= ADD JWT AUTHENTICATION =========================

        // Method: AddJwtAuthentication - Extension method to configure JWT authentication
        // Called in Program.cs: services.AddJwtAuthentication(configuration)
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Read JWT settings from appsettings.json
            var jwtSettings = configuration.GetSection("JwtSettings");

            // Read secret key used to sign and validate tokens
            var secretKey = jwtSettings["Key"];

            // ========================= CONFIGURE JWT BEARER =========================

            // Register JWT Bearer as default authentication scheme
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // Configure JWT Bearer options
                .AddJwtBearer(options =>
                {
                    // Set token validation parameters
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            // Validate that issuer (who created token) is trusted
                            ValidateIssuer = true,

                            // Validate that audience (who can use token) matches
                            ValidateAudience = true,

                            // Validate token hasn't expired
                            ValidateLifetime = true,

                            // Validate token is signed with expected secret key
                            ValidateIssuerSigningKey = true,

                            // Trusted issuer from appsettings.json
                            ValidIssuer = jwtSettings["Issuer"],

                            // Trusted audience from appsettings.json
                            ValidAudience = jwtSettings["Audience"],

                            // Secret key for validating token signature
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(secretKey!))
                        };
                });

            // Return services collection for chaining
            return services;
        }

        // ========================= ADD SWAGGER DOCUMENTATION =========================

        // Method: AddSwaggerDocumentation - Extension method to configure Swagger UI
        // Called in Program.cs: services.AddSwaggerDocumentation()
        // Swagger provides interactive API documentation at /swagger
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            // Enable API explorer to discover endpoints and models
            services.AddEndpointsApiExplorer();

            // ========================= CONFIGURE SWAGGER GENERATION =========================

            // Register Swagger generator
            services.AddSwaggerGen(options =>
            {
                // Define API documentation version and metadata
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    // API title shown in Swagger UI
                    Title = "QuickBite Auth API",

                    // API version
                    Version = "v1",

                    // API description
                    Description = "Authentication APIs for QuickBite"
                });

                // ========================= CONFIGURE JWT SECURITY =========================

                // Add JWT Bearer security definition
                // This allows users to enter JWT token in Swagger UI
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        // Security scheme name
                        Name = "Authorization",

                        // Type of security scheme (HTTP)
                        Type = SecuritySchemeType.Http,

                        // HTTP authentication scheme (bearer for JWT)
                        Scheme = "bearer",

                        // Format of bearer token (JWT)
                        BearerFormat = "JWT",

                        // Where to put token (HTTP Authorization header)
                        In = ParameterLocation.Header,

                        // Help text for users
                        Description = "Enter JWT Token"
                    });

                // ========================= REQUIRE JWT FOR PROTECTED ENDPOINTS =========================

                // Add security requirement to all endpoints
                // Swagger will show lock icon on protected endpoints
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            // Reference to Bearer security scheme defined above
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    // Reference type is SecurityScheme
                                    Type = ReferenceType.SecurityScheme,

                                    // Reference ID matches the definition above
                                    Id = "Bearer"
                                }
                            },
                            // Empty array means no specific scopes required
                            Array.Empty<string>()
                        }
                    });
            });

            // Return services collection for chaining
            return services;
        }
    }
}