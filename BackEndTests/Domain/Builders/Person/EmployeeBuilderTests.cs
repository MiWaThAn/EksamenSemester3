using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Person
{
    public class EmployeeBuilderTests
    {
        private readonly string _validName = "Mads Mikkelsen";
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly EmailAddress _stubEmail = new EmailAddress("mads@company.dk");
        private readonly EmployeeType _validType = EmployeeType.Formand;
        private readonly Company _stubCompany;

        public EmployeeBuilderTests()
        {
            _stubCompany = new Company { Id = _validCompanyId };
        }

        #region Fluent Method Guard Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithName_WhenNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var builder = new EmployeeBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithName(invalidName!));
        }

        [Fact]
        public void WithCompany_WhenCompanyIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var builder = new EmployeeBuilder();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => builder.WithCompany(null!));
        }

        [Fact]
        public void WithCompanyId_WhenGuidIsEmpty_ShouldThrowException()
        {
            // Arrange
            var builder = new EmployeeBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithCompanyId(Guid.Empty));
        }

        #endregion

        #region Build Guard Tests

        [Fact]
        public void Build_WhenNameIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new EmployeeBuilder()
                .WithCompany(_stubCompany); // Name left unconfigured

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenCompanyInformationIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new EmployeeBuilder()
                .WithName(_validName); // Company configuration left unconfigured

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredEmployee()
        {
            // Arrange
            var builder = new EmployeeBuilder()
                .WithName(_validName)
                .WithCompany(_stubCompany)
                .WithEmployeeType(_validType)
                .WithEmail(_stubEmail)
                .WithAutonomy(true);

            // Act
            Employee employee = builder.Build();

            // Assert
            Assert.NotNull(employee);
            Assert.Equal(_validName, employee.Name);
            Assert.Equal(_validCompanyId, employee.CompanyId);
            Assert.Equal(_validType, employee.EmployeeType);
            Assert.Equal(_stubEmail, employee.Email);
            Assert.True(employee.IsAutonomous);
        }

        [Fact]
        public void Build_UsingCompanyIdOverload_ShouldReturnConfiguredEmployee()
        {
            // Arrange
            var builder = new EmployeeBuilder()
                .WithName(_validName)
                .WithCompanyId(_validCompanyId);

            // Act
            Employee employee = builder.Build();

            // Assert
            Assert.NotNull(employee);
            Assert.Equal(_validCompanyId, employee.CompanyId);
            Assert.False(employee.IsAutonomous); // Verifies default bool behavior (false)
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new EmployeeBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithName(_validName));
            Assert.Same(builder, builder.WithCompany(_stubCompany));
            Assert.Same(builder, builder.WithCompanyId(_validCompanyId));
            Assert.Same(builder, builder.WithEmployeeType(_validType));
            Assert.Same(builder, builder.WithEmail(_stubEmail));
            Assert.Same(builder, builder.WithAutonomy(true));
        }

        #endregion
    }
}
