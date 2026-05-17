// Configuration access for reading settings from appsettings.json
using Microsoft.Extensions.Configuration;

// JWT token handling and signing
using Microsoft.IdentityModel.Tokens;

// Imports IJwtTokenGenerator interface
using QuickBite.Auth.Application.Interfaces;

// Imports User entity
using QuickBite.Auth.Domain.Entities;

// JWT token creation and manipulation
using System.IdentityModel.Tokens.Jwt;

// Claims (user information) in JWT
using System.Security.Claims;

// Text encoding for UTF-8 conversion
using System.Text;

// Namespace for security-related classes
namespace QuickBite.Auth.Infrastructure.Security
{
    // JwtTokenGenerator: Creates JWT tokens for authenticated users
    // JWT (JSON Web Token) is used for stateless authentication
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // Configuration object to read settings from appsettings.json
        private readonly IConfiguration _configuration;

        // ========================= CONSTRUCTOR =========================

        // Constructor with Dependency Injection
        // Receives configuration from ASP.NET DI container
        public JwtTokenGenerator(IConfiguration configuration)
        {
            // Store configuration reference
            _configuration = configuration;
        }

        // ========================= GENERATE TOKEN METHOD =========================

        // Method: GenerateToken - Creates JWT token for user
        // Parameter: user - User object containing UserId, Email, Role, FullName
        // Returns: JWT token string (signed and encoded)
        public string GenerateToken(User user)
        {
            // Read JWT settings section from appsettings.json file
            var jwtSettings = _configuration.GetSection("JwtSettings");

            // Read secret key used to sign the token (from appsettings.json)
            var key = jwtSettings["Key"];

            // Read issuer (who created the token) from appsettings.json
            var issuer = jwtSettings["Issuer"];

            // Read audience (who can use the token) from appsettings.json
            var audience = jwtSettings["Audience"];

            // Read token expiry time in minutes from appsettings.json
            var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);

            // ========================= CREATE CLAIMS LIST =========================

            // Claims: Information about the user to include in the token
            var claims = new List<Claim>
            {
                // Subject claim: User's unique ID
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),

                // Email claim: Standard JWT email
                new Claim(JwtRegisteredClaimNames.Email, user.Email),

                // NameIdentifier claim: User ID (used by [Authorize] attribute)
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

                // Name claim: User's full name
                new Claim(ClaimTypes.Name, user.FullName),

                // Email claim: User's email address
                new Claim(ClaimTypes.Email, user.Email),

                // Role claim: User's role (Admin, Customer, RestaurantOwner, DeliveryPartner)
                new Claim(ClaimTypes.Role, user.Role)
            };

            // ========================= SIGN TOKEN =========================

            // Convert secret key string to bytes using UTF-8 encoding
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            // Create signing credentials using HMAC-SHA256 algorithm
            // This ensures token cannot be tampered with
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // ========================= BUILD JWT TOKEN =========================

            // Create JWT token with all parameters
            var token = new JwtSecurityToken(
                // Who created this token
                issuer: issuer,

                // Who can use this token
                audience: audience,

                // User information to include in token
                claims: claims,

                // When token expires (current time + X minutes)
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),

                // How to sign and validate token
                signingCredentials: credentials
            );

            // ========================= SERIALIZE TOKEN =========================

            // Convert JWT object to string (URL-safe format)
            // Returns encoded JWT token that client can send in Authorization header
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
