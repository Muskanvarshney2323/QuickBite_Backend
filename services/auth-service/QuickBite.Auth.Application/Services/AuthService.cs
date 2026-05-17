// Imports DTOs (data transfer objects) for request/response
using QuickBite.Auth.Application.DTOs;

// Imports service interfaces like IAuthService
using QuickBite.Auth.Application.Interfaces;

// Imports User entity from Domain layer
using QuickBite.Auth.Domain.Entities;

// Namespace for service classes
namespace QuickBite.Auth.Application.Services
{
    // AuthService implements IAuthService interface
    // Contains all business logic for authentication operations
    public class AuthService : IAuthService
    {
        // Repository for accessing User data from database
        private readonly IUserRepository _userRepository;

        // Service for generating JWT tokens
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        // ========================= CONSTRUCTOR =========================

        // Constructor with Dependency Injection
        // Receives dependencies from ASP.NET DI container
        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            // Store repository reference
            _userRepository = userRepository;

            // Store token generator reference
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        // ========================= REGISTER METHOD =========================

        // Method: RegisterAsync - Creates a new user in the system
        // Parameter: request - Contains FullName, Email, Password, Phone, Role from user input
        // Returns: String message indicating registration success
        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            // Trim whitespace and convert email to lowercase for consistency
            var email = request.Email.Trim().ToLower();

            // Check if user with this email already exists
            var existingUser = await _userRepository.GetByEmailAsync(email);

            // If user exists, throw error
            if (existingUser != null)
            {
                throw new Exception("User already exists with this email.");
            }

            // Create new User object with provided data
            var user = new User
            {
                // Store full name after trimming whitespace
                FullName = request.FullName.Trim(),

                // Store email (lowercase)
                Email = email,

                // Hash password using BCrypt for security (not plain text)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

                // Store phone after trimming whitespace
                Phone = request.Phone.Trim(),

                // Normalize role to standard format (Admin, Customer, RestaurantOwner, DeliveryPartner)
                Role = NormalizeRole(request.Role),

                // User is active by default when registering
                IsActive = true,

                // Store current UTC time as registration timestamp
                CreatedAt = DateTime.UtcNow
            };

            // Save new user to database
            await _userRepository.AddAsync(user);

            // Commit changes to database
            await _userRepository.SaveChangesAsync();

            // Return success message
            return "User registered successfully.";
        }

        // ========================= LOGIN METHOD =========================

        // Method: LoginAsync - Verifies user credentials and generates JWT token
        // Parameter: request - Contains Email and Password from login form
        // Returns: JWT token string (used for subsequent API calls)
        public async Task<string> LoginAsync(LoginRequestDto request)
        {
            // Trim and lowercase email for consistency
            var email = request.Email.Trim().ToLower();

            // Fetch user from database using email
            var user = await _userRepository.GetByEmailAsync(email);

            // If user not found OR account is deactivated, deny login
            if (user == null || !user.IsActive)
            {
                throw new Exception("Invalid email or password.");
            }

            // Verify provided password matches stored hashed password
            // BCrypt.Verify checks password against hash safely
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            // If password doesn't match, deny login
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password.");
            }

            // Generate JWT token containing user's data (UserId, Email, Role)
            // Token includes expiry time and is signed with secret key
            return _jwtTokenGenerator.GenerateToken(user);
        }

        // ========================= GET CURRENT USER METHOD =========================

        // Method: GetCurrentUserAsync - Retrieves logged-in user's information
        // Parameter: email - Email of the authenticated user (extracted from JWT token)
        // Returns: UserResponseDto with user details or null if not found
        public async Task<UserResponseDto?> GetCurrentUserAsync(string email)
        {
            // Fetch user from database using email
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());

            // If user not found, return null
            // If found, convert User entity to UserResponseDto for API response
            return user == null ? null : MapToUserResponse(user);
        }

        // ========================= VALIDATE TOKEN METHOD =========================

        // Method: ValidateTokenUserAsync - Checks if user exists and is active
        // Parameter: email - User's email from JWT token
        // Returns: True if user exists and is active, False otherwise
        public async Task<bool> ValidateTokenUserAsync(string email)
        {
            // Fetch user from database
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());

            // Return true only if user exists AND account is active
            return user != null && user.IsActive;
        }

        // ========================= GET USER BY ID METHOD =========================

        // Method: GetUserByIdAsync - Retrieves user information by ID (Admin only)
        // Parameter: userId - Unique identifier (GUID) of the user
        // Returns: UserResponseDto with user details or null if not found
        public async Task<UserResponseDto?> GetUserByIdAsync(Guid userId)
        {
            // Fetch user from database using unique ID
            var user = await _userRepository.GetByIdAsync(userId);

            // If user not found, return null
            // If found, convert to response DTO
            return user == null ? null : MapToUserResponse(user);
        }

        // ========================= GET USER BY EMAIL METHOD =========================

        // Method: GetUserByEmailAsync - Retrieves user information by email (Admin only)
        // Parameter: email - Email address to search for
        // Returns: UserResponseDto with user details or null if not found
        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            // Fetch user from database using email
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());

            // If user not found, return null
            // If found, convert to response DTO
            return user == null ? null : MapToUserResponse(user);
        }

        // ========================= UPDATE PROFILE METHOD =========================

        // Method: UpdateProfileAsync - Updates user's name and phone number
        // Parameter: email - Current user's email
        // Parameter: request - Contains updated FullName and Phone
        // Returns: Updated UserResponseDto
        public async Task<UserResponseDto> UpdateProfileAsync(string email, UpdateProfileRequestDto request)
        {
            // Fetch user from database
            // ?? throws exception if user not found
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            // If account is deactivated, don't allow profile updates
            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            // Update FullName only if new value provided, otherwise keep existing value
            user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName.Trim();

            // Update Phone only if new value provided, otherwise keep existing value
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? user.Phone : request.Phone.Trim();

            // Save updated user to database
            await _userRepository.UpdateAsync(user);

            // Commit changes to database
            await _userRepository.SaveChangesAsync();

            // Convert updated user to response DTO and return
            return MapToUserResponse(user);
        }

        // ========================= CHANGE PASSWORD METHOD =========================

        // Method: ChangePasswordAsync - Changes user's password
        // Parameter: email - User's email
        // Parameter: request - Contains CurrentPassword and NewPassword
        public async Task ChangePasswordAsync(string email, ChangePasswordRequestDto request)
        {
            // Fetch user from database
            // ?? throws exception if user not found
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            // If account is deactivated, don't allow password changes
            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            // Verify current password matches what's stored in database
            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);

            // If current password is incorrect, throw error
            if (!isCurrentPasswordValid)
            {
                throw new Exception("Current password is incorrect.");
            }

            // Validate new password meets minimum requirements (at least 6 characters)
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            {
                throw new Exception("New password must be at least 6 characters long.");
            }

            // Hash the new password using BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Save updated user to database
            await _userRepository.UpdateAsync(user);

            // Commit changes to database
            await _userRepository.SaveChangesAsync();
        }

        // ========================= DEACTIVATE ACCOUNT METHOD =========================

        // Method: DeactivateAccountAsync - Disables user account (soft delete)
        // Parameter: email - User's email
        // Note: Account data is preserved but marked as inactive
        public async Task DeactivateAccountAsync(string email)
        {
            // Fetch user from database
            // ?? throws exception if user not found
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            // If account already deactivated, throw error
            if (!user.IsActive)
            {
                throw new Exception("Account is already deactivated.");
            }

            // Mark account as inactive
            user.IsActive = false;

            // Save updated user to database
            await _userRepository.UpdateAsync(user);

            // Commit changes to database
            await _userRepository.SaveChangesAsync();
        }

        // ========================= REFRESH TOKEN METHOD =========================

        // Method: RefreshTokenAsync - Generates new JWT token for existing user
        // Parameter: email - User's email (from expired token)
        // Returns: New JWT token string
        public async Task<string> RefreshTokenAsync(string email)
        {
            // Fetch user from database
            // ?? throws exception if user not found
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            // If account is deactivated, don't issue new token
            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            // Generate new JWT token with user's current data
            return _jwtTokenGenerator.GenerateToken(user);
        }

        // ========================= LOGOUT METHOD =========================

        // Method: LogoutAsync - Handles user logout
        // Parameter: email - User's email
        // Note: Currently just validates email, token removal happens on client side
        public Task LogoutAsync(string email)
        {
            // Validate email is provided
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Invalid user email.");
            }

            // Return completed task (no database operations needed)
            // Token is invalidated on client side by removing it from storage
            return Task.CompletedTask;
        }

        // ========================= NORMALIZE ROLE HELPER METHOD =========================

        // Private helper method: NormalizeRole - Converts various role formats to standard names
        // Parameter: role - Role string from client (could be uppercase, lowercase, with spaces, etc.)
        // Returns: Standardized role name (Admin, Customer, RestaurantOwner, DeliveryPartner)
        private static string NormalizeRole(string? role)
        {
            // Trim whitespace and convert to uppercase for comparison
            var normalized = (role ?? string.Empty).Trim().ToUpperInvariant();

            // Switch statement converts various input formats to standard role names
            return normalized switch
            {
                // Admin variations
                "ADMIN" => "Admin",

                // RestaurantOwner variations
                "RESTAURANT" => "RestaurantOwner",
                "RESTAURANTOWNER" => "RestaurantOwner",
                "RESTAURANT_OWNER" => "RestaurantOwner",
                "OWNER" => "RestaurantOwner",

                // DeliveryPartner variations (supports multiple naming conventions)
                "AGENT" => "DeliveryPartner",
                "DELIVERY" => "DeliveryPartner",
                "DELIVERYPARTNER" => "DeliveryPartner",
                "DELIVERY_PARTNER" => "DeliveryPartner",
                "DELIVERY PARTNER" => "DeliveryPartner",
                "DELIVERYAGENT" => "DeliveryPartner",
                "DELIVERY_AGENT" => "DeliveryPartner",
                "DELIVERY AGENT" => "DeliveryPartner",

                // Customer variations
                "CUSTOMER" => "Customer",
                "USER" => "Customer",

                // Default to Customer if role not recognized
                _ => "Customer"
            };
        }

        // ========================= MAP TO RESPONSE HELPER METHOD =========================

        // Private helper method: MapToUserResponse - Converts User entity to UserResponseDto
        // Parameter: user - User entity from database
        // Returns: UserResponseDto for API response (excludes sensitive data like password)
        private static UserResponseDto MapToUserResponse(User user)
        {
            // Create new response DTO with non-sensitive user data
            return new UserResponseDto
            {
                // User's unique identifier
                UserId = user.UserId,

                // User's full name
                FullName = user.FullName,

                // User's email
                Email = user.Email,

                // User's phone number
                Phone = user.Phone,

                // User's role (Admin, Customer, RestaurantOwner, DeliveryPartner)
                Role = user.Role

                // Note: Password hash is NOT included in response for security
            };
        }
    }
}
