using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Application.Interfaces;
using QuickBite.Auth.Application.Services;
using QuickBite.Auth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace QuickBite.Auth.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _authService = new AuthService(_userRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldRegisterUser_WhenEmailIsUnique()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Password = "password123",
                Phone = "1234567890",
                Role = "Customer"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.AreEqual("User registered successfully.", result);
            _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.FullName == "John Doe" &&
                u.Email == "john@example.com" &&
                u.Phone == "1234567890" &&
                u.Role == "Customer" &&
                u.IsActive == true &&
                !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "existing@example.com"
            };

            var existingUser = new User { Email = "existing@example.com" };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("existing@example.com")).ReturnsAsync(existingUser);

            // Act & Assert
            try
            {
                _authService.RegisterAsync(request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("User already exists with this email.", ex.Message);
            }
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "john@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsActive = true
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.AreEqual("jwt-token", result);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "nonexistent@example.com" };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

            // Act & Assert
            try
            {
                _authService.LoginAsync(request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid email or password.", ex.Message);
            }
        }

        [TestMethod]
        public async Task LoginAsync_ShouldThrowException_WhenUserIsInactive()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "inactive@example.com" };
            var user = new User { Email = "inactive@example.com", IsActive = false };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("inactive@example.com")).ReturnsAsync(user);

            // Act & Assert
            try
            {
                _authService.LoginAsync(request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid email or password.", ex.Message);
            }
        }

        [TestMethod]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "john@example.com", Password = "wrongpassword" };
            var user = new User
            {
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsActive = true
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            // Act & Assert
            try
            {
                _authService.LoginAsync(request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid email or password.", ex.Message);
            }
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            // Act
            var result = await _authService.GetCurrentUserAsync("john@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.FullName, result.FullName);
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

            // Act
            var result = await _authService.GetCurrentUserAsync("nonexistent@example.com");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateProfileAsync_ShouldUpdateFields_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = "Old Name",
                Email = "john@example.com",
                Phone = "oldphone",
                IsActive = true
            };
            var request = new UpdateProfileRequestDto
            {
                FullName = "New Name",
                Phone = "newphone"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.UpdateProfileAsync("john@example.com", request);

            // Assert
            Assert.AreEqual("New Name", result.FullName);
            Assert.AreEqual("newphone", result.Phone);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateProfileAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

            // Act & Assert
            try
            {
                _authService.UpdateProfileAsync("nonexistent@example.com", new UpdateProfileRequestDto()).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("User not found.", ex.Message);
            }
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldChangePassword_WhenCurrentPasswordIsCorrect()
        {
            // Arrange
            var user = new User
            {
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
                IsActive = true
            };
            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword123"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _authService.ChangePasswordAsync("john@example.com", request);

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => !string.IsNullOrEmpty(u.PasswordHash))), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldThrowException_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var user = new User
            {
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
                IsActive = true
            };
            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "wrongpassword",
                NewPassword = "newpassword123"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            // Act & Assert
            try
            {
                _authService.ChangePasswordAsync("john@example.com", request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Current password is incorrect.", ex.Message);
            }
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldThrowException_WhenNewPasswordIsTooShort()
        {
            // Arrange
            var user = new User
            {
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
                IsActive = true
            };
            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "oldpassword",
                NewPassword = "123"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            // Act & Assert
            try
            {
                _authService.ChangePasswordAsync("john@example.com", request).GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("New password must be at least 6 characters long.", ex.Message);
            }
        }

        [TestMethod]
        public async Task DeactivateAccountAsync_ShouldDeactivateUser_WhenUserExistsAndActive()
        {
            // Arrange
            var user = new User { Email = "john@example.com", IsActive = true };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _authService.DeactivateAccountAsync("john@example.com");

            // Assert
            Assert.IsFalse(user.IsActive);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeactivateAccountAsync_ShouldThrowException_WhenUserIsAlreadyInactive()
        {
            // Arrange
            var user = new User { Email = "john@example.com", IsActive = false };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            // Act & Assert
            try
            {
                _authService.DeactivateAccountAsync("john@example.com").GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Account is already deactivated.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ShouldReturnNewToken_WhenUserExistsAndActive()
        {
            // Arrange
            var user = new User { Email = "john@example.com", IsActive = true };
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user)).Returns("new-jwt-token");

            // Act
            var result = await _authService.RefreshTokenAsync("john@example.com");

            // Assert
            Assert.AreEqual("new-jwt-token", result);
        }

        [TestMethod]
        public async Task LogoutAsync_ShouldComplete_WhenEmailIsValid()
        {
            // Act
            await _authService.LogoutAsync("john@example.com");

            // Assert - No exception, just completes
        }

        [TestMethod]
        public async Task LogoutAsync_ShouldThrowException_WhenEmailIsInvalid()
        {
            // Act & Assert
            try
            {
                _authService.LogoutAsync("").GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid user email.", ex.Message);
            }
        }
    }
}
