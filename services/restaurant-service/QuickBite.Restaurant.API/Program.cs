using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuickBite.Restaurant.Application.Interfaces;
using QuickBite.Restaurant.Application.Services;
using QuickBite.Restaurant.Infrastructure.Data;
using QuickBite.Restaurant.Infrastructure.Repositories;
using System.Text;
using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS is required because React runs on a different localhost port.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuickBite Restaurant Service",
        Version = "v1"
    });

    options.EnableAnnotations();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token like this: Bearer your_token_here",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var jwtKey = jwtSettings["Key"] ?? throw new Exception("JwtSettings:Key is missing.");
var jwtIssuer = jwtSettings["Issuer"] ?? throw new Exception("JwtSettings:Issuer is missing.");
var jwtAudience = jwtSettings["Audience"] ?? throw new Exception("JwtSettings:Audience is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { service = "restaurant-service", status = "running" }));

app.MapControllers();

// For project demo: keep DB ready and add sample restaurants if table is empty.
// This prevents the frontend from showing an empty restaurant page on first run.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Restaurants.AnyAsync())
    {
        var demoOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        db.Restaurants.AddRange(
            new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Palace",
                Description = "Fresh pizza, pasta, and cheesy snacks.",
                Cuisine = "Italian",
                Address = "Central Market, Meerut",
                City = "Meerut",
                Phone = "9999999999",
                OwnerId = demoOwnerId,
                IsApproved = true,
                IsOpen = true,
                MinimumOrderAmount = 100,
                EstimatedDeliveryTimeInMinutes = 30,
                AverageRating = 4.5
            },
            new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = "Biryani Hub",
                Description = "Hyderabadi biryani and North Indian meals.",
                Cuisine = "Indian",
                Address = "Begum Bridge Road, Meerut",
                City = "Meerut",
                Phone = "8888888888",
                OwnerId = demoOwnerId,
                IsApproved = true,
                IsOpen = true,
                MinimumOrderAmount = 150,
                EstimatedDeliveryTimeInMinutes = 35,
                AverageRating = 4.7
            },
            new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = "Momo Street",
                Description = "Steamed, fried, and spicy momos.",
                Cuisine = "Chinese",
                Address = "Shastri Nagar, Meerut",
                City = "Meerut",
                Phone = "7777777777",
                OwnerId = demoOwnerId,
                IsApproved = true,
                IsOpen = true,
                MinimumOrderAmount = 80,
                EstimatedDeliveryTimeInMinutes = 25,
                AverageRating = 4.3
            });

        await db.SaveChangesAsync();
    }
}

app.Run();
