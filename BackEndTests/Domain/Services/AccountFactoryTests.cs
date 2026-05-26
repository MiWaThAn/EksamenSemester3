using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Services
{
    public class AccountFactoryTests
    {
        private readonly Mock<IAccountValidationService> _mockValidationService;
        private readonly AccountFactory _factory;

        private readonly string _validUsername = "admin@enterprise.dk";
        private readonly string _validPassword = "SecretHashedPassword123";
        private readonly PhoneNumber _stubPhoneNumber = new PhoneNumber("+4512345678");

        public AccountFactoryTests()
        {
            _mockValidationService = new Mock<IAccountValidationService>();
            _factory = new AccountFactory(_mockValidationService.Object);
        }

        #region Constructor Dependency Rules

        [Fact]
        public void Constructor_WhenValidationServiceIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AccountFactory(null!));
        }

        #endregion

        #region Factory Processing Core System Tests

        [Fact]
        public async Task CreateAsync_WithValidUniqueUsername_ShouldReturnSuccessResultWithAccount()
        {
            // Arrange
            var builder = new AccountBuilder()
                .WithUsername(_validUsername)
                .WithHashedPassword(_validPassword)
                .WithPhoneNumber(_stubPhoneNumber);

            _mockValidationService
                .Setup(s => s.UsernameExistsAsync(_validUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // No duplicate user exists

            // Act
            Result<Account> result = await _factory.CreateAsync(builder, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validUsername, result.Value.Username);

            // Verify that the factory checked the validation rule exactly once
            _mockValidationService.Verify(s => s.UsernameExistsAsync(_validUsername, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenUsernameAlreadyExists_ShouldReturnFailureResult()
        {
            // Arrange
            var builder = new AccountBuilder()
                .WithUsername(_validUsername)
                .WithHashedPassword(_validPassword)
                .WithPhoneNumber(_stubPhoneNumber);

            _mockValidationService
                .Setup(s => s.UsernameExistsAsync(_validUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Duplicate username detected

            // Act
            Result<Account> result = await _factory.CreateAsync(builder, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Brugernavnet er allerede i brug.", result.Error);

            // Ensure that builder.Build() was not executed upon a failure state if necessary (value remains unassigned/null)
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task CreateAsync_WhenBuilderIsNull_ShouldThrowExceptionViaGuard()
        {
            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _factory.CreateAsync(null!, CancellationToken.None));
        }

        #endregion
    }
}
