using QuickBite.Auth.Application.DTOs;
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
            var exists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (exists)
            {
                throw new Exception("Email already registered");
            }

            var role = string.IsNullOrWhiteSpace(request.Role)
                ? UserRole.Customer
                : request.Role.Trim();

            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email.Trim().ToLower(),
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

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

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLower());

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                throw new Exception("Invalid email or password");
            }

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
    }
}