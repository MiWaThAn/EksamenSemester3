using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Person
{
    public class AccountBuilderTests
    {
        private readonly string _validUsername = "test.user@company.com";
        private readonly string _validPasswordHash = "HashedPassword123!";
        private readonly string _validPinHash = "1234Hashed";
        private readonly PhoneNumber _stubPhoneNumber = new PhoneNumber("+4512345678");

        private readonly Company _stubCompany;
        private readonly Employee _stubEmployee;

        public AccountBuilderTests()
        {
            _stubCompany = new Company { Id = Guid.NewGuid() };
            _stubEmployee = new Employee { Id = Guid.NewGuid() };
        }

        #region Fluent Method Guard Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithHashedPassword_WhenNullOrEmpty_ShouldThrowException(string invalidPassword)
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithHashedPassword(invalidPassword!));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithHashedPin_WhenNullOrEmpty_ShouldThrowException(string invalidPin)
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithHashedPin(invalidPin!));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithUsername_WhenNullOrEmpty_ShouldThrowException(string invalidUsername)
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithUsername(invalidUsername!));
        }

        [Fact]
        public void WithCompany_WhenNull_ShouldThrowArgumentNullException()
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithCompany(null!));
        }

        [Fact]
        public void WithEmployee_WhenNull_ShouldThrowArgumentNullException()
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithEmployee(null!));
        }

        [Fact]
        public void WithPhoneNumber_WhenNull_ShouldThrowArgumentNullException()
        {
            var builder = new AccountBuilder();
            Assert.ThrowsAny<Exception>(() => builder.WithPhoneNumber(null!));
        }

        #endregion

        #region Build Guard Tests

        [Fact]
        public void Build_WhenUsernameIsMissing_ShouldThrowException()
        {
            var builder = new AccountBuilder()
                .WithHashedPassword(_validPasswordHash)
                .WithPhoneNumber(_stubPhoneNumber);

            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenPhoneNumberIsMissing_ShouldThrowException()
        {
            var builder = new AccountBuilder()
                .WithUsername(_validUsername)
                .WithHashedPassword(_validPasswordHash);

            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenHashedPasswordIsMissing_ShouldThrowException()
        {
            var builder = new AccountBuilder()
                .WithUsername(_validUsername)
                .WithPhoneNumber(_stubPhoneNumber);

            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredAccount()
        {
            // Arrange
            var builder = new AccountBuilder()
                .WithUsername(_validUsername)
                .WithHashedPassword(_validPasswordHash)
                .WithHashedPin(_validPinHash)
                .WithPhoneNumber(_stubPhoneNumber)
                .WithCompany(_stubCompany)
                .WithEmployee(_stubEmployee);

            // Act
            Account account = builder.Build();

            // Assert
            Assert.NotNull(account);
            Assert.Equal(_validUsername, account.Username);
            Assert.Equal(_validPasswordHash, account.HashedPassword);
            Assert.Equal(_validPinHash, account.HashedPin);
            Assert.Equal(_stubPhoneNumber, account.PhoneNumber);
            Assert.Equal(_stubCompany.Id, account.CompanyId);
            Assert.Equal(_stubEmployee.Id, account.EmployeeId);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new AccountBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithUsername(_validUsername));
            Assert.Same(builder, builder.WithHashedPassword(_validPasswordHash));
            Assert.Same(builder, builder.WithHashedPin(_validPinHash));
            Assert.Same(builder, builder.WithPhoneNumber(_stubPhoneNumber));
            Assert.Same(builder, builder.WithCompany(_stubCompany));
            Assert.Same(builder, builder.WithEmployee(_stubEmployee));
        }

        #endregion
    }
}
