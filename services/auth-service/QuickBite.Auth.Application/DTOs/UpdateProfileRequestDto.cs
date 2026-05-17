// Namespace for Data Transfer Objects
namespace QuickBite.Auth.Application.DTOs
{
    // UpdateProfileRequestDto: Receives profile update request from client
    // Used in PUT /api/auth/profile endpoint
    public class UpdateProfileRequestDto
    {
        // FullName: User's updated full name
        // Optional - if empty, current name is kept
        // Example: "Jane Doe"
        public string FullName { get; set; } = string.Empty;

        // Phone: User's updated phone number
        // Optional - if empty, current phone is kept
        // Example: "+1-555-5678"
        public string Phone { get; set; } = string.Empty;
    }
}
