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
    public class CompanyFactoryTests
    {
        private readonly Mock<ICompanyValidationService> _mockValidationService;
        private readonly CompanyFactory _factory;

        private readonly string _validName = "Danish Cyber Security ApS";
        private readonly CvrNumber _stubCvr = new CvrNumber("88888888");
        private readonly EmailAddress _stubEmail = new EmailAddress("info@cybersec.dk");
        private readonly Account _stubAccount = new Account { Id = Guid.NewGuid() };

        public CompanyFactoryTests()
        {
            _mockValidationService = new Mock<ICompanyValidationService>();
            _factory = new CompanyFactory(_mockValidationService.Object);
        }

        #region Constructor Dependency Rules

        [Fact]
        public void Constructor_WhenValidationServiceIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyFactory(null!));
        }

        #endregion

        #region Factory Core Logic Processing Tests

        [Fact]
        public async Task CreateAsync_WithUniqueCvr_ShouldReturnSuccessResultWithLinkedCompany()
        {
            // Arrange
            var builder = new CompanyBuilder()
                .WithName(_validName)
                .WithCVRNumber(_stubCvr)
                .WithEmail(_stubEmail);

            _mockValidationService
                .Setup(s => s.CvrExistsAsync(_stubCvr, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // CVR is completely available

            // Act
            Result<Company> result = await _factory.CreateAsync(builder, _stubAccount, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validName, result.Value.Name);
            Assert.Equal(_stubCvr, result.Value.CVRNumber);
            Assert.Equal(_stubAccount.Id, result.Value.AccountId); // Ensures WithAccount() was called in-flight

            _mockValidationService.Verify(s => s.CvrExistsAsync(_stubCvr, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCvrAlreadyExists_ShouldReturnFailureResult()
        {
            // Arrange
            var builder = new CompanyBuilder()
                .WithName(_validName)
                .WithCVRNumber(_stubCvr)
                .WithEmail(_stubEmail);

            _mockValidationService
                .Setup(s => s.CvrExistsAsync(_stubCvr, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // CVR clash detected in validation layer

            // Act
            Result<Company> result = await _factory.CreateAsync(builder, _stubAccount, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains($"Et firma med dette CVR nummer {_stubCvr} findes alerede.", result.Error);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task CreateAsync_WhenBuilderIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _factory.CreateAsync(null!, _stubAccount, CancellationToken.None));
        }

        #endregion
    }
}
