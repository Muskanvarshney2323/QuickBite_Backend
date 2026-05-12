using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------- CORS ----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------------- YARP Reverse Proxy ----------------
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ---------------- JWT Authentication ----------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var jwtKey = jwtSettings["Key"] ?? throw new Exception("JwtSettings:Key is missing.");
var jwtIssuer = jwtSettings["Issuer"] ?? throw new Exception("JwtSettings:Issuer is missing.");
var jwtAudience = jwtSettings["Audience"] ?? throw new Exception("JwtSettings:Audience is missing.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// ---------------- Swagger ----------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuickBite API Gateway",
        Version = "v1",
        Description = "Single entry point for all QuickBite microservices."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter token like this: Bearer your_token_here",
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

var app = builder.Build();

// ---------------- Swagger UI ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";

        options.SwaggerEndpoint("/swagger/v1/swagger.json", "QuickBite API Gateway");

        options.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
        options.SwaggerEndpoint("/restaurant/swagger/v1/swagger.json", "Restaurant Service");
        options.SwaggerEndpoint("/menu/swagger/v1/swagger.json", "Menu Service");
        options.SwaggerEndpoint("/cart/swagger/v1/swagger.json", "Cart Service");
        options.SwaggerEndpoint("/order/swagger/v1/swagger.json", "Order Service");
        options.SwaggerEndpoint("/payment/swagger/v1/swagger.json", "Payment Service");
    });
}

// ---------------- Middleware ----------------
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Only gateway test endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "QuickBite API Gateway",
    status = "running",
    swagger = "http://localhost:5000/swagger"
}));

// Real APIs are forwarded by YARP from appsettings.json
app.MapReverseProxy();

app.Run();