// Namespace for Data Transfer Objects
namespace QuickBite.Auth.Application.DTOs
{
    // RegisterRequestDto: Receives user registration details from client
    // Used in POST /api/auth/register endpoint
    public class RegisterRequestDto
    {
        // FullName: User's complete name
        // Example: "John Doe"
        public string FullName { get; set; } = string.Empty;

        // Email: User's email address (must be unique)
        // Example: "john@example.com"
        public string Email { get; set; } = string.Empty;

        // Password: User's password (plain text from client)
        // Will be hashed using BCrypt on server
        // Example: "MyPassword123"
        public string Password { get; set; } = string.Empty;

        // Phone: User's phone number
        // Example: "+1-555-1234"
        public string Phone { get; set; } = string.Empty;

        // Role: User's role in the system
        // Values: "Customer", "RestaurantOwner", "DeliveryPartner", "Admin"
        // Example: "Customer"
        public string Role { get; set; } = string.Empty;
    }
}