using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Item;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Services
{
    public class RegistrationDomainServiceTests
    {
        private readonly Mock<IHashingService> _mockPasswordHasher;
        private readonly Mock<ICompanyValidationService> _mockCompanyValidation;
        private readonly Mock<IAccountValidationService> _mockAccountValidation;
        private readonly Mock<IAccountFactory> _mockAccountFactory;
        private readonly Mock<ICompanyFactory> _mockCompanyFactory;

        private readonly RegistrationDomainService _service;

        // Stub test data
        private readonly string _companyName = "Viking Tech Solutions";
        private readonly CvrNumber _cvrNumber = new CvrNumber("12345678");
        private readonly EmailAddress _emailAddress = new EmailAddress("admin@vikingtech.dk");
        private readonly PhoneNumber _phoneNumber = new PhoneNumber("+4512345678");
        private readonly string _username = "viking.admin";
        private readonly string _plainPassword = "SuperSecretPassword123!";
        private readonly string _hashedPassword = "SystemHashedPassword_XYZ_123";

        public RegistrationDomainServiceTests()
        {
            _mockPasswordHasher = new Mock<IHashingService>();
            _mockCompanyValidation = new Mock<ICompanyValidationService>();
            _mockAccountValidation = new Mock<IAccountValidationService>();
            _mockAccountFactory = new Mock<IAccountFactory>();
            _mockCompanyFactory = new Mock<ICompanyFactory>();

            _service = new RegistrationDomainService(
                _mockPasswordHasher.Object,
                _mockAccountValidation.Object,
                _mockCompanyValidation.Object,
                _mockAccountFactory.Object,
                _mockCompanyFactory.Object
            );

            // Default mock setups
            _mockPasswordHasher.Setup(h => h.Hash(_plainPassword)).Returns(_hashedPassword);
        }

        #region RegisterCompanyAccountAsync Tests

        [Fact]
        public async Task RegisterCompanyAccountAsync_WithValidUniqueData_ShouldReturnSuccessTuple()
        {
            // Arrange
            var expectedAccount = new Account { Username = _username };
            var expectedCompany = new Company { Name = _companyName, CVRNumber = _cvrNumber };

            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockCompanyValidation.Setup(v => v.CvrExistsAsync(_cvrNumber, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            _mockAccountFactory.Setup(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Account>.Success(expectedAccount));

            _mockCompanyFactory.Setup(f => f.CreateAsync(It.IsAny<CompanyBuilder>(), expectedAccount, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Company>.Success(expectedCompany));

            // Act
            var result = await _service.RegisterCompanyAccountAsync(_companyName, _cvrNumber, _emailAddress, _phoneNumber, _username, _plainPassword, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Same(expectedCompany, result.Value.Item1);
            Assert.Same(expectedAccount, result.Value.Item2);
            _mockPasswordHasher.Verify(h => h.Hash(_plainPassword), Times.Once);
        }

        [Fact]
        public async Task RegisterCompanyAccountAsync_WhenUsernameExists_ShouldReturnFailureResult()
        {
            // Arrange
            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _service.RegisterCompanyAccountAsync(_companyName, _cvrNumber, _emailAddress, _phoneNumber, _username, _plainPassword, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Brugernavn er allerede i brug", result.Error);
            _mockAccountFactory.Verify(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterCompanyAccountAsync_WhenCvrExists_ShouldReturnFailureResult()
        {
            // Arrange
            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockCompanyValidation.Setup(v => v.CvrExistsAsync(_cvrNumber, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _service.RegisterCompanyAccountAsync(_companyName, _cvrNumber, _emailAddress, _phoneNumber, _username, _plainPassword, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Cvr er allerede i brug.", result.Error);
        }

        [Fact]
        public async Task RegisterCompanyAccountAsync_WhenAccountFactoryFails_ShouldShortCircuit()
        {
            // Arrange
            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockCompanyValidation.Setup(v => v.CvrExistsAsync(_cvrNumber, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            _mockAccountFactory.Setup(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Account>.Failure("Factory Account Error"));

            // Act
            var result = await _service.RegisterCompanyAccountAsync(_companyName, _cvrNumber, _emailAddress, _phoneNumber, _username, _plainPassword, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Factory Account Error", result.Error);
            _mockCompanyFactory.Verify(f => f.CreateAsync(It.IsAny<CompanyBuilder>(), It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region RegisterEmployeeAccountAsync Tests

        [Fact]
        public async Task RegisterEmployeeAccountAsync_WithValidUniqueData_ShouldReturnSuccessAndLinkEmployee()
        {
            // Arrange
            var employee = new Employee { Id = Guid.NewGuid() };
            var expectedAccount = new Account { Id = Guid.NewGuid(), Username = _username };

            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockAccountFactory.Setup(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Account>.Success(expectedAccount));

            // Act
            var result = await _service.RegisterEmployeeAccountAsync(_phoneNumber, _username, _plainPassword, employee, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedAccount, result.Value);
            Assert.Equal(expectedAccount.Id, employee.AccountId); // Asserts employee.LinkToAccount(account) was triggered
        }

        [Fact]
        public async Task RegisterEmployeeAccountAsync_WhenUsernameExists_ShouldReturnFailureResult()
        {
            // Arrange
            var employee = new Employee { Id = Guid.NewGuid() };
            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _service.RegisterEmployeeAccountAsync(_phoneNumber, _username, _plainPassword, employee, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Brugernavn er allerede i brug", result.Error);
            _mockAccountFactory.Verify(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterEmployeeAccountAsync_WhenAccountFactoryFails_ShouldReturnFailureResult()
        {
            // Arrange
            var employee = new Employee { Id = Guid.NewGuid() };
            _mockAccountValidation.Setup(v => v.UsernameExistsAsync(_username, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockAccountFactory.Setup(f => f.CreateAsync(It.IsAny<AccountBuilder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Account>.Failure("Factory Employee-Account Error"));

            // Act
            var result = await _service.RegisterEmployeeAccountAsync(_phoneNumber, _username, _plainPassword, employee, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Factory Employee-Account Error", result.Error);
            Assert.Null(employee.AccountId); // Verification that it was never linked
        }

        #endregion
    }
}
