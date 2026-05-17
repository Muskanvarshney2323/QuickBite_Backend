// Namespace for domain entities (business objects)
namespace QuickBite.Auth.Domain.Entities
{
    // User: Domain entity representing a registered user in the system
    // Stored in PostgreSQL Users table
    public class User
    {
        // ========================= PROPERTIES =========================

        // UserId: Unique identifier (GUID) for the user
        // Primary key in Users table
        public Guid UserId { get; set; }

        // FullName: User's complete name
        // Example: "John Doe"
        public string FullName { get; set; } = string.Empty;

        // Email: User's email address (unique, used for login)
        // Example: "john@example.com"
        public string Email { get; set; } = string.Empty;

        // PasswordHash: Hashed password (never store plain text passwords)
        // Uses BCrypt for secure hashing
        public string PasswordHash { get; set; } = string.Empty;

        // Phone: User's phone number
        // Example: "+1-555-1234"
        public string Phone { get; set; } = string.Empty;

        // Role: User's role in the system
        // Values: "Admin", "Customer", "RestaurantOwner", "DeliveryPartner"
        public string Role { get; set; } = string.Empty;

        // IsActive: Whether user account is active (not deactivated)
        // Default: true (active when registered)
        // False means account is soft-deleted
        public bool IsActive { get; set; } = true;

        // CreatedAt: Registration timestamp in UTC
        // Used to track when user created their account
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}