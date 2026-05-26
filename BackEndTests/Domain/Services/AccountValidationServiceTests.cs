using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Domain.Services.Person;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Services
{
    public class AccountValidationServiceTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly AccountValidationService _validationService;
        private readonly string _testUsername = "verify.user@domain.dk";

        public AccountValidationServiceTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _validationService = new AccountValidationService(_mockAccountRepository.Object);
        }

        #region Operational Scenario Tests

        [Fact]
        public async Task UsernameExistsAsync_WhenRepositoryReturnsAccount_ShouldReturnTrue()
        {
            // Arrange
            var matchedAccount = new Account { Username = _testUsername };

            _mockAccountRepository
                .Setup(repo => repo.GetByUsernameAsync(_testUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync(matchedAccount);

            // Act
            bool result = await _validationService.UsernameExistsAsync(_testUsername, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockAccountRepository.Verify(repo => repo.GetByUsernameAsync(_testUsername, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UsernameExistsAsync_WhenRepositoryReturnsNull_ShouldReturnFalse()
        {
            // Arrange
            _mockAccountRepository
                .Setup(repo => repo.GetByUsernameAsync(_testUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            bool result = await _validationService.UsernameExistsAsync(_testUsername, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockAccountRepository.Verify(repo => repo.GetByUsernameAsync(_testUsername, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Boundary Exception Guard Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task UsernameExistsAsync_WhenUsernameIsNullOrEmpty_ShouldThrowArgumentNullException(string? invalidUsername)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _validationService.UsernameExistsAsync(invalidUsername!, CancellationToken.None));

            // Verify that the database layer was completely bypassed for invalid structural inputs
            _mockAccountRepository.Verify(repo => repo.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion
    }
}
