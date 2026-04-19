using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Application.Exceptions;
using QuickBite.Auth.Application.Interfaces;
using QuickBite.Auth.Domain.Entities;
using QuickBite.Auth.Domain.Enums;

namespace QuickBite.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var exists = await _userRepository.ExistsByEmailAsync(normalizedEmail);

            if (exists)
            {
                throw new ConflictException("Email is already registered");
            }

            var role = NormalizeRole(request.Role);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                Phone = request.Phone.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            return BuildAuthResponse(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role
                }
            };
        }

        private static string NormalizeRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return UserRole.Customer;
            }

            var normalizedRole = role.Trim().ToLower();

            return normalizedRole switch
            {
                "admin" => UserRole.Admin,
                "customer" => UserRole.Customer,
                "restaurantowner" => UserRole.RestaurantOwner,
                "restaurant_owner" => UserRole.RestaurantOwner,
                "restaurant owner" => UserRole.RestaurantOwner,
                "owner" => UserRole.RestaurantOwner,
                "deliverypartner" => UserRole.DeliveryPartner,
                "delivery_partner" => UserRole.DeliveryPartner,
                "delivery partner" => UserRole.DeliveryPartner,
                "agent" => UserRole.DeliveryPartner,
                _ => throw new ArgumentException("Invalid role provided")
            };
        }
    }
}
