using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item
{
    public class ExpenseBuilderTests
    {
        private readonly string _validName = "Office Supplies";
        private readonly Company _validCompany = new Company { Id = Guid.NewGuid() };

        #region Fluent Method Guard Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var builder = new ExpenseBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithName(invalidName));
        }

        [Fact]
        public void WithCompany_WhenCompanyIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new ExpenseBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithCompany(null));
        }

        #endregion

        #region Build Method Guard Tests

        [Fact]
        public void Build_WhenNameIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ExpenseBuilder()
                .WithCompany(_validCompany);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenCompanyIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ExpenseBuilder()
                .WithName(_validName);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredExpense()
        {
            // Arrange
            var builder = new ExpenseBuilder()
                .WithName(_validName)
                .WithCompany(_validCompany);

            // Act
            Expense expense = builder.Build();

            // Assert
            Assert.NotNull(expense);
            Assert.Equal(_validName, expense.Name);
            Assert.Equal(_validCompany.Id, expense.CompanyId);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new ExpenseBuilder();

            // Act
            var nameResult = builder.WithName(_validName);
            var companyResult = builder.WithCompany(_validCompany);

            // Assert
            Assert.Same(builder, nameResult);
            Assert.Same(builder, companyResult);
        }

        #endregion
    }
}
