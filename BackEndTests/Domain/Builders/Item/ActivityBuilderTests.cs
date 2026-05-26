using Domain.Builders.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item
{
    public class ActivityBuilderTests
    {
        private readonly string _validName = "Code Review";
        private readonly string _validDescription = "Reviewing pull requests for the team.";

        //stubbing a basic Company since it is required by the builder
        private readonly Company _validCompany = new Company { Id = Guid.NewGuid() };

        //fluent Method Guards

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var builder = new ActivityBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithName(invalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithDescription_WhenDescriptionIsNullOrEmpty_ShouldThrowException(string invalidDescription)
        {
            // Arrange
            var builder = new ActivityBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithDescription(invalidDescription));
        }

        [Fact]
        public void WithCompany_WhenCompanyIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new ActivityBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithCompany(null));
        }


        //build Method Guards

        [Fact]
        public void Build_WhenNameIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ActivityBuilder()
                .WithDescription(_validDescription)
                .WithCompany(_validCompany);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenDescriptionIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ActivityBuilder()
                .WithName(_validName)
                .WithCompany(_validCompany);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenCompanyIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ActivityBuilder()
                .WithName(_validName)
                .WithDescription(_validDescription);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        //success Tests

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredActivity()
        {
            // Arrange
            var builder = new ActivityBuilder()
                .WithName(_validName)
                .WithDescription(_validDescription)
                .WithCompany(_validCompany);

            // Act
            Activity activity = builder.Build();

            // Assert
            Assert.NotNull(activity);
            Assert.Equal(_validName, activity.Name);
            Assert.Equal(_validDescription, activity.Description);
            Assert.Equal(_validCompany.Id, activity.CompanyId);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new ActivityBuilder();

            // Act
            var nameResult = builder.WithName(_validName);
            var descriptionResult = builder.WithDescription(_validDescription);
            var companyResult = builder.WithCompany(_validCompany);

            // Assert
            Assert.Same(builder, nameResult);
            Assert.Same(builder, descriptionResult);
            Assert.Same(builder, companyResult);
        }
    }
}
