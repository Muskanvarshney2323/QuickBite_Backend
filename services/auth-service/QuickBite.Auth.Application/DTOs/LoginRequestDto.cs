// Namespace for Data Transfer Objects
namespace QuickBite.Auth.Application.DTOs
{
    // LoginRequestDto: Receives login credentials from client
    // Used in POST /api/auth/login endpoint
    public class LoginRequestDto
    {
        // Email: User's email address for login
        // Example: "john@example.com"
        public string Email { get; set; } = string.Empty;

        // Password: User's password (plain text from client, hashed on server)
        // Example: "MyPassword123"
        public string Password { get; set; } = string.Empty;
    }
}