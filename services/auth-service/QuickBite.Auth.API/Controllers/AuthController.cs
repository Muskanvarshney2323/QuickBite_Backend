// Used for authentication and authorization features like [Authorize]
using Microsoft.AspNetCore.Authorization;

// Used for creating Web APIs, controllers, routing, IActionResult, etc.
using Microsoft.AspNetCore.Mvc;

// Used for reading data (claims) from JWT token like Email and Role
using System.Security.Claims;

// Imports DTO classes used for request/response data transfer
using QuickBite.Auth.Application.DTOs;

// Imports service interfaces like IAuthService
using QuickBite.Auth.Application.Interfaces;

// Namespace of this controller
namespace QuickBite.Auth.API.Controllers
{
    // Marks this class as API Controller
    [ApiController]

    // Base route becomes: api/auth
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Service object used to call business logic methods
        private readonly IAuthService _authService;

        // Constructor Dependency Injection
        // ASP.NET automatically injects AuthService here
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ========================= REGISTER API =========================

        // Endpoint: POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Register new user
            var result = await _authService.RegisterAsync(request);

            // Automatically login user after registration
            // Generates JWT token
            var token = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = request.Email,
                Password = request.Password
            });

            // Fetch user details using email
            var user = await _authService.GetUserByEmailAsync(request.Email);

            // Return success response with message, token and user data
            return Ok(new { message = result, token, user });
        }

        // ========================= LOGIN API =========================

        // Endpoint: POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Validate user and generate JWT token
            var token = await _authService.LoginAsync(request);

            // Fetch logged in user details
            var user = await _authService.GetUserByEmailAsync(request.Email);

            // Return token and user details
            return Ok(new { token, user });
        }

        // ========================= LOGOUT API =========================

        // Only authenticated users can access this API
        [Authorize]

        // Endpoint: POST api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request)
        {
            // Get logged in user's email from JWT token
            var email = GetCurrentUserEmail();

            // Logout user
            await _authService.LogoutAsync(email);

            // Return logout success message
            return Ok(new
            {
                message = "Logout successful. Please remove the token from client side storage.",

                // Returning token if provided
                token = request?.Token
            });
        }

        // ========================= REFRESH TOKEN API =========================

        // User must be logged in
        [Authorize]

        // Endpoint: POST api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            // Get email from token
            var email = GetCurrentUserEmail();

            // Generate new JWT token
            var token = await _authService.RefreshTokenAsync(email);

            // Return new token
            return Ok(new { token });
        }

        // ========================= CURRENT USER API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: GET api/auth/me
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Get current user's email from JWT token
            var email = GetCurrentUserEmail();

            // Fetch current logged in user data
            var user = await _authService.GetCurrentUserAsync(email);

            // If user not found
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Return user details
            return Ok(user);
        }

        // ========================= GET PROFILE API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: GET api/auth/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Get email from token
            var email = GetCurrentUserEmail();

            // Fetch profile data
            var user = await _authService.GetUserByEmailAsync(email);

            // If user not found
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Return profile data
            return Ok(user);
        }

        // ========================= UPDATE PROFILE API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: PUT api/auth/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            // Get logged in user's email
            var email = GetCurrentUserEmail();

            // Update user profile
            var updatedUser = await _authService.UpdateProfileAsync(email, request);

            // Return updated profile
            return Ok(new
            {
                message = "Profile updated successfully.",
                user = updatedUser
            });
        }

        // ========================= CHANGE PASSWORD API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: PUT api/auth/password
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            // Get logged in user's email
            var email = GetCurrentUserEmail();

            // Change password
            await _authService.ChangePasswordAsync(email, request);

            // Return success message
            return Ok(new { message = "Password changed successfully." });
        }

        // ========================= DEACTIVATE ACCOUNT API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: DELETE api/auth/deactivate
        [HttpDelete("deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            // Get current user's email
            var email = GetCurrentUserEmail();

            // Deactivate account
            await _authService.DeactivateAccountAsync(email);

            // Return success message
            return Ok(new { message = "Account deactivated successfully." });
        }

        // ========================= ADMIN GET USER BY ID =========================

        // Only Admin can access this API
        [Authorize(Roles = "Admin")]

        // Endpoint: GET api/auth/users/{id}
        [HttpGet("users/{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            // Fetch user by ID
            var user = await _authService.GetUserByIdAsync(userId);

            // If user not found
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Return user data
            return Ok(user);
        }

        // ========================= ADMIN GET USER BY EMAIL =========================

        // Only Admin can access
        [Authorize(Roles = "Admin")]

        // Endpoint: GET api/auth/users/by-email
        [HttpGet("users/by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            // Fetch user using email
            var user = await _authService.GetUserByEmailAsync(email);

            // If user not found
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Return user details
            return Ok(user);
        }

        // ========================= VALIDATE TOKEN API =========================

        // User must be authenticated
        [Authorize]

        // Endpoint: GET api/auth/validate-token
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            // Get email from JWT token
            var email = GetCurrentUserEmail();

            // Check if user exists and account is active
            var isValidUser = await _authService.ValidateTokenUserAsync(email);

            // If user invalid
            if (!isValidUser)
            {
                return Unauthorized(new
                {
                    message = "User does not exist or account is inactive."
                });
            }

            // Token is valid
            return Ok(new { message = "Token is valid." });
        }

        // ========================= ADMIN ONLY API =========================

        // Only Admin role can access
        [Authorize(Roles = "Admin")]

        // Endpoint: GET api/auth/admin-only
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Welcome Admin" });
        }

        // ========================= CUSTOMER ONLY API =========================

        // Only Customer role can access
        [Authorize(Roles = "Customer")]

        // Endpoint: GET api/auth/customer-only
        [HttpGet("customer-only")]
        public IActionResult CustomerOnly()
        {
            return Ok(new { message = "Welcome Customer" });
        }

        // ========================= ADMIN OR OWNER API =========================

        // Both Admin and RestaurantOwner can access
        [Authorize(Roles = "Admin,RestaurantOwner")]

        // Endpoint: GET api/auth/admin-or-owner
        [HttpGet("admin-or-owner")]
        public IActionResult AdminOrOwner()
        {
            return Ok(new { message = "Welcome Admin or Owner" });
        }

        // ========================= HELPER METHOD =========================

        // Private helper method to extract email from JWT token
        private string GetCurrentUserEmail()
        {
            // Read Email claim from JWT token
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // If email not found in token
            if (string.IsNullOrWhiteSpace(email))
            {
                // Throw unauthorized exception
                throw new UnauthorizedAccessException("Invalid token.");
            }

            // Return extracted email
            return email;
        }
    }
}
