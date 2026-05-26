using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item
{
    public class ProjectBuilderTests
    {
        private readonly string _validName = "Infrastructure Modernization Project";
        private readonly string _validDescription = "Migrating legacy data pipelines to a cloud architecture.";
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly Status _validStatus = Status.Åben;

        private readonly Company _stubCompany;
        private readonly Customer _stubCustomer;
        private readonly Employee _stubEmployee;
        private readonly Address _stubAddress;

        public ProjectBuilderTests()
        {
            // Initialize associated stub entities with valid IDs
            _stubCompany = new Company { Id = _validCompanyId };
            _stubCustomer = new Customer { Id = Guid.NewGuid() };
            _stubEmployee = new Employee { Id = Guid.NewGuid() };
            _stubAddress = new Address();
        }

        #region Fluent Guard Validation Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var builder = new ProjectBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithName(invalidName!));
        }

        [Fact]
        public void WithDescription_WhenDescriptionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var builder = new ProjectBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithDescription(null!));
        }

        [Fact]
        public void WithCompany_WhenCompanyIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var builder = new ProjectBuilder();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => builder.WithCompany(null!));
        }

        [Fact]
        public void WithCompanyId_WhenGuidIsEmpty_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithCompanyId(Guid.Empty));
        }

        #endregion

        #region Build Guard Validation Tests

        [Fact]
        public void Build_WhenNameIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectBuilder()
                .WithCompany(_stubCompany); // Name left unconfigured

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenCompanyIsMissing_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectBuilder()
                .WithName(_validName); // Company details left unconfigured

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredProject()
        {
            // Arrange
            var builder = new ProjectBuilder()
                .WithName(_validName)
                .WithDescription(_validDescription)
                .WithCompany(_stubCompany)
                .WithCustomer(_stubCustomer)
                .WithResponsibleEmployee(_stubEmployee)
                .WithAddress(_stubAddress)
                .WithIsStatus(_validStatus);

            // Act
            Project project = builder.Build();

            // Assert
            Assert.NotNull(project);
            Assert.Equal(_validName, project.Name);
            Assert.Equal(_validDescription, project.Description);
            Assert.Equal(_stubCompany.Id, project.CompanyId);
            Assert.Equal(_stubCustomer.Id, project.CustomerId);
            Assert.Equal(_stubEmployee.Id, project.ResponsibleEmployeeId);
            Assert.Same(_stubAddress, project.Address);
            Assert.Equal(_validStatus, project.Status);
        }

        [Fact]
        public void Build_WithAlternativeCompanyIdOverload_ShouldReturnConfiguredProject()
        {
            // Arrange
            var builder = new ProjectBuilder()
                .WithName(_validName)
                .WithCompanyId(_validCompanyId);

            // Act
            Project project = builder.Build();

            // Assert
            Assert.NotNull(project);
            Assert.Equal(_validCompanyId, project.CompanyId);
            Assert.Equal(string.Empty, project.Description); // Verifies class default string allocation
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new ProjectBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithName(_validName));
            Assert.Same(builder, builder.WithAddress(_stubAddress));
            Assert.Same(builder, builder.WithCustomer(_stubCustomer));
            Assert.Same(builder, builder.WithResponsibleEmployee(_stubEmployee));
            Assert.Same(builder, builder.WithIsStatus(_validStatus));
            Assert.Same(builder, builder.WithDescription(_validDescription));
            Assert.Same(builder, builder.WithCompany(_stubCompany));
            Assert.Same(builder, builder.WithCompanyId(_validCompanyId));
        }

        #endregion
    }
}
