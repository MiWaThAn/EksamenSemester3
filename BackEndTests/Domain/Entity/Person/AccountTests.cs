using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Person
{
    public class AccountTests
    {
        private readonly string _validUsername = "john.doe@company.com";
        private readonly string _validPasswordHash = "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3";
        private readonly PhoneNumber _stubPhoneNumber = new PhoneNumber("+4512345678");

        #region Constructor & Initialization Tests

        [Fact]
        public void Constructor_WithValidCredentials_ShouldInitializeCorrectlyAsUnlinked()
        {
            // Act
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);

            // Assert
            Assert.Equal(_validUsername, account.Username);
            Assert.Equal(_validPasswordHash, account.HashedPassword);
            Assert.Equal(_stubPhoneNumber, account.PhoneNumber);
            Assert.Null(account.HashedPin);
            Assert.False(account.IsCompanyAccount);
            Assert.False(account.IsEmployeeAccount);
            Assert.Empty(account.Roles);
            Assert.Empty(account.DeviceTokens);
        }

        [Fact]
        public void Constructor_WhenPassingEntities_ShouldLinkAutomatically()
        {
            // Arrange
            var company = new Company { Id = Guid.NewGuid() };
            var employee = new Employee { Id = Guid.NewGuid() };

            // Act
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, "1234Hash", employee, company);

            // Assert
            Assert.True(account.IsCompanyAccount);
            Assert.True(account.IsEmployeeAccount);
            Assert.Equal(company.Id, account.CompanyId);
            Assert.Equal(employee.Id, account.EmployeeId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WithInvalidCredentials_ShouldThrowException(string invalidInput)
        {
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => new Account(invalidInput, _validPasswordHash, _stubPhoneNumber, null, null, null));
            Assert.ThrowsAny<Exception>(() => new Account(_validUsername, invalidInput, _stubPhoneNumber, null, null, null));
        }

        #endregion

        #region Value Mutation & Security Guards

        [Fact]
        public void UpdatePassword_WithValidHash_ShouldAlterPropertyAndSetUpdatedAt()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            var initialUpdate = account.UpdatedAt;
            var newHash = "NEW_SECURE_HASHED_PASSWORD_VAL";

            // Act
            account.UpdatePassword(newHash);

            // Assert
            Assert.Equal(newHash, account.HashedPassword);
            Assert.True(account.UpdatedAt >= initialUpdate);
        }

        [Fact]
        public void UpdatePassword_WithNullOrEmpty_ShouldThrowException()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => account.UpdatePassword(null!));
            Assert.ThrowsAny<Exception>(() => account.UpdatePassword(""));
        }

        #endregion

        #region Password Recovery Lifecycle Machine

        [Fact]
        public void PasswordResetWorkflow_WithValidTokenInsideExpiryWindow_ShouldSucceed()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            var token = account.GeneratePasswordResetToken();
            var newPasswordHash = "COMPLETELY_NEW_MUTATED_PASSWORD_HASH";

            // Act
            account.ResetPassword(token, newPasswordHash);

            // Assert
            Assert.Equal(newPasswordHash, account.HashedPassword);
            Assert.Null(account.RecorveryToken);
            Assert.Null(account.RecoveryExpiry);
        }

        [Fact]
        public void ResetPassword_WithInvalidToken_ShouldThrowException()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            account.GeneratePasswordResetToken();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => account.ResetPassword("IncorrectTokenString", "NewHash"));
        }

        [Fact]
        public void ResetPassword_WhenTokenIsExpired_ShouldThrowException()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            var token = account.GeneratePasswordResetToken();

            // Use reflection to artificially age the token expiry property backward past 30 mins ago
            var expiryProp = typeof(Account).GetProperty("RecoveryExpiry");
            expiryProp?.SetValue(account, DateTime.UtcNow.AddMinutes(-31));

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => account.ResetPassword(token, "NewHash"));
            Assert.Contains("Reset token has expired.", exception.Message);
        }

        #endregion

        #region Collections & Integration Factory Processing

        [Fact]
        public void AddDeviceToken_ShouldEnsureDistinctSetEntriesOnly()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            string matchingToken = "Firebase_Device_Token_String_XYZ";

            // Act
            account.AddDeviceToken(matchingToken);
            account.AddDeviceToken(matchingToken); // Duplicate entry request
            account.AddDeviceToken("");            // Invalid entry request

            // Assert
            Assert.Single(account.DeviceTokens);
            Assert.Equal(matchingToken, account.DeviceTokens.First().Value);
        }

        [Fact]
        public void AddRole_ShouldAddRoleIfNotAlreadyPresent()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            var adminRole = new Role { Id = Guid.NewGuid(), Title = "Admin" };

            // Act
            account.AddRole(adminRole);
            account.AddRole(adminRole); // Try duplicate registration entry

            // Assert
            Assert.Single(account.Roles);
        }

        [Fact]
        public async Task CreateCompany_WhenFactorySucceeds_ShouldLinkToAccount()
        {
            // Arrange
            var account = new Account(_validUsername, _validPasswordHash, _stubPhoneNumber, null, null, null);
            var createdCompany = new Company { Id = Guid.NewGuid() };

            // Use a REAL builder instead of a Mock
            var realBuilder = new CompanyBuilder();
            var mockFactory = new Mock<ICompanyFactory>();

            // Setup successful factory result returns matching the real builder
            mockFactory.Setup(f => f.CreateAsync(realBuilder, account, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<Company>.Success(createdCompany));

            // Act
            var result = await account.CreateCompany(realBuilder, mockFactory.Object);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(account.IsCompanyAccount);
            Assert.Equal(createdCompany.Id, account.CompanyId);
            Assert.Same(createdCompany, account.Company);
        }

        #endregion
    }
}
