// Namespace for Data Transfer Objects
namespace QuickBite.Auth.Application.DTOs
{
    // UserResponseDto: Sends user information to client
    // Used in API responses (e.g., after login, get profile, etc.)
    // Does NOT include password hash for security
    public class UserResponseDto
    {
        // UserId: User's unique identifier (GUID)
        // Used to reference user in other services
        public Guid UserId { get; set; }

        // FullName: User's complete name
        // Example: "John Doe"
        public string FullName { get; set; } = string.Empty;

        // Email: User's email address
        // Example: "john@example.com"
        public string Email { get; set; } = string.Empty;

        // Phone: User's phone number
        // Example: "+1-555-1234"
        public string Phone { get; set; } = string.Empty;

        // Role: User's role in the system
        // Values: "Customer", "RestaurantOwner", "DeliveryPartner", "Admin"
        public string Role { get; set; } = string.Empty;

        // Note: PasswordHash is NOT included in response for security
    }
}