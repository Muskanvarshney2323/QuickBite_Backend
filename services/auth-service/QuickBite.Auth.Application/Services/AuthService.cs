using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Application.Interfaces;
using QuickBite.Auth.Domain.Entities;

namespace QuickBite.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            var email = request.Email.Trim().ToLower();
            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                throw new Exception("User already exists with this email.");
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone.Trim(),
                Role = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<string> LoginAsync(LoginRequestDto request)
        {
            var email = request.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                throw new Exception("Invalid email or password.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password.");
            }

            return _jwtTokenGenerator.GenerateToken(user);
        }

        public async Task<UserResponseDto?> GetCurrentUserAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());
            return user == null ? null : MapToUserResponse(user);
        }

        public async Task<bool> ValidateTokenUserAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());
            return user != null && user.IsActive;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : MapToUserResponse(user);
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());
            return user == null ? null : MapToUserResponse(user);
        }

        public async Task<UserResponseDto> UpdateProfileAsync(string email, UpdateProfileRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? user.Phone : request.Phone.Trim();

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToUserResponse(user);
        }

        public async Task ChangePasswordAsync(string email, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                throw new Exception("Current password is incorrect.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            {
                throw new Exception("New password must be at least 6 characters long.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeactivateAccountAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            if (!user.IsActive)
            {
                throw new Exception("Account is already deactivated.");
            }

            user.IsActive = false;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<string> RefreshTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower())
                       ?? throw new Exception("User not found.");

            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated.");
            }

            return _jwtTokenGenerator.GenerateToken(user);
        }

        public Task LogoutAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Invalid user email.");
            }

            return Task.CompletedTask;
        }

        private static UserResponseDto MapToUserResponse(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };
        }
    }
}
