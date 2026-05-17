// Namespace for Data Transfer Objects
namespace QuickBite.Auth.Application.DTOs
{
    // ChangePasswordRequestDto: Receives password change request from client
    // Used in PUT /api/auth/password endpoint
    public class ChangePasswordRequestDto
    {
        // CurrentPassword: User's existing password (to verify identity)
        // Plain text from client, will be verified against stored hash
        // Example: "OldPassword123"
        public string CurrentPassword { get; set; } = string.Empty;

        // NewPassword: User's new password
        // Plain text from client, will be hashed using BCrypt on server
        // Must be at least 6 characters long
        // Example: "NewPassword456"
        public string NewPassword { get; set; } = string.Empty;
    }
}
