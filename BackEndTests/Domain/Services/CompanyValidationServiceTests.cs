using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Domain.Services.Person;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Services
{
    public class CompanyValidationServiceTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly CompanyValidationService _validationService;
        private readonly CvrNumber _testCvr = new CvrNumber("12345678");

        public CompanyValidationServiceTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _validationService = new CompanyValidationService(_mockCompanyRepository.Object);
        }

        #region Operational Scenario Tests

        [Fact]
        public async Task CvrExistsAsync_WhenRepositoryReturnsCompany_ShouldReturnTrue()
        {
            // Arrange
            var matchedCompany = new Company { CVRNumber = _testCvr };

            _mockCompanyRepository
                .Setup(repo => repo.GetByCVRAsync(_testCvr, It.IsAny<CancellationToken>()))
                .ReturnsAsync(matchedCompany);

            // Act
            bool result = await _validationService.CvrExistsAsync(_testCvr, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockCompanyRepository.Verify(repo => repo.GetByCVRAsync(_testCvr, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CvrExistsAsync_WhenRepositoryReturnsNull_ShouldReturnFalse()
        {
            // Arrange
            _mockCompanyRepository
                .Setup(repo => repo.GetByCVRAsync(_testCvr, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company?)null);

            // Act
            bool result = await _validationService.CvrExistsAsync(_testCvr, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockCompanyRepository.Verify(repo => repo.GetByCVRAsync(_testCvr, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CvrExistsAsync_ShouldForwardCancellationTokenToRepository()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            _mockCompanyRepository
                .Setup(repo => repo.GetByCVRAsync(_testCvr, cts.Token))
                .ReturnsAsync((Company?)null);

            // Act
            await _validationService.CvrExistsAsync(_testCvr, cts.Token);

            // Assert
            _mockCompanyRepository.Verify(repo => repo.GetByCVRAsync(_testCvr, cts.Token), Times.Once);
        }

        #endregion
    }
}
