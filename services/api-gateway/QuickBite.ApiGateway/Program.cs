using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----- CORS: let Flutter / React frontends call the gateway -----
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----- YARP: load routes and clusters from appsettings.json -----
// This is the actual reverse proxy. It reads the "ReverseProxy" section
// and forwards incoming requests to the right microservice based on the URL path.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ----- JWT Authentication (same settings as all microservices) -----
// The gateway validates the token once, then forwards the request.
// Each microservice will also validate again for safety.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JwtSettings:Key is missing in appsettings.json.");
if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new Exception("JwtSettings:Issuer is missing in appsettings.json.");
if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new Exception("JwtSettings:Audience is missing in appsettings.json.");
if (jwtKey.Length < 32)
    throw new Exception("JwtSettings:Key must be at least 32 characters long.");

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

// ----- Swagger (gateway-level landing page) -----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuickBite API Gateway",
        Version = "v1",
        Description = "Single entry point for all QuickBite microservices. " +
                      "Forwards /api/v1/auth -> Auth-Service, /api/v1/cart -> Cart-Service, etc."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Friendly root page so people know the gateway is alive.
app.MapGet("/", () => Results.Ok(new
{
    service = "QuickBite API Gateway",
    status = "running",
    docs = "/swagger",
    routes = new[]
    {
        "/api/v1/auth/**          -> Auth-Service (5234)",
        "/api/v1/cart/**          -> Cart-Service (5111)",
        "/api/v1/menu/**          -> Menu-Service (5281)",
        "/api/v1/orders/**        -> Order-Service (5113)",
        "/api/v1/payments/**      -> Payment-Service (5114)",
        "/api/v1/restaurants/**   -> Restaurant-Service (5167)",
        "/api/v1/agents/**        -> Delivery-Agent-Service (5115)",
        "/api/v1/reviews/**       -> Review-Service (5116)",
        "/api/v1/notifications/** -> Notification-Service (5117)"
    }
}));

// Tell YARP to handle everything else.
app.MapReverseProxy();

app.Run();
