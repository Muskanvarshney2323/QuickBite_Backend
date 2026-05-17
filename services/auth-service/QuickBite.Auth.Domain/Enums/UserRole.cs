// Namespace for enum constants
namespace QuickBite.Auth.Domain.Enums
{
    // UserRole: Static class containing all available user role constants
    // Used for authorization and [Authorize(Roles="...")] attributes
    public static class UserRole
    {
        // ========================= ROLE CONSTANTS =========================

        // Customer: Regular user who places food orders
        // Can view restaurants, menu items, place orders, track delivery
        public const string Customer = "Customer";

        // Admin: System administrator with full permissions
        // Can manage all users, view reports, configure system settings
        public const string Admin = "Admin";

        // RestaurantOwner: Restaurant business owner
        // Can manage restaurant profile, menu items, view orders
        public const string RestaurantOwner = "RestaurantOwner";

        // DeliveryPartner: Delivery personnel
        // Can accept delivery orders, update delivery status, track location
        public const string DeliveryPartner = "DeliveryPartner";
    }
}