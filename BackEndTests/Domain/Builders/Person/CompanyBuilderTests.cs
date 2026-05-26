using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Person
{
    public class CompanyBuilderTests
    {
        private readonly string _validCompanyName = "Nordic Tech Solutions ApS";
        private readonly CvrNumber _stubCvr = new CvrNumber("87654321");
        private readonly EmailAddress _stubEmail = new EmailAddress("contact@nordictech.dk");
        private readonly Account _stubAccount = new Account { Id = Guid.NewGuid() };

        #region Fluent Method Guard Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithName_WhenNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var builder = new CompanyBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithName(invalidName!));
        }

        #endregion

        #region Build Guard Tests

        [Fact]
        public void Build_WhenNameIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new CompanyBuilder()
                .WithCVRNumber(_stubCvr)
                .WithEmail(_stubEmail)
                .WithAccount(_stubAccount); // Name left unconfigured

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenAccountIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new CompanyBuilder()
                .WithName(_validCompanyName)
                .WithCVRNumber(_stubCvr)
                .WithEmail(_stubEmail); // Account left unconfigured (internal rule protection)

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredCompany()
        {
            // Arrange
            var builder = new CompanyBuilder()
                .WithName(_validCompanyName)
                .WithCVRNumber(_stubCvr)
                .WithEmail(_stubEmail)
                .WithAccount(_stubAccount);

            // Act
            Company company = builder.Build();

            // Assert
            Assert.NotNull(company);
            Assert.Equal(_validCompanyName, company.Name);
            Assert.Equal(_stubCvr, company.CVRNumber);
            Assert.Equal(_stubEmail, company.Email);
            Assert.Equal(_stubAccount.Id, company.AccountId);
            Assert.Same(_stubAccount, company.Account);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new CompanyBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithName(_validCompanyName));
            Assert.Same(builder, builder.WithCVRNumber(_stubCvr));
            Assert.Same(builder, builder.WithEmail(_stubEmail));
            Assert.Same(builder, builder.WithAccount(_stubAccount));
        }

        #endregion
    }
}
