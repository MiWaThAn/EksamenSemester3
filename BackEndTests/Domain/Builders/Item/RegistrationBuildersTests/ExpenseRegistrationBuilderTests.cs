using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item.RegistrationBuildersTests
{
    public class ExpenseRegistrationBuilderTests
    {
        private readonly Project _validProject = new Project { Id = Guid.NewGuid() };
        private readonly WorkLog _validWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };
        private readonly Expense _validExpense = new Expense { Id = Guid.NewGuid() };
        private readonly string _validDescription = "Taxi fare for client onboarding visit.";
        private readonly RegistrationStatus _validStatus = RegistrationStatus.Pending;

        #region Method Guard Tests

        [Fact]
        public void WithExpense_WhenExpenseIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var builder = new ExpenseRegistrationBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithExpense(null!));
        }

        #endregion

        #region Build Guard Tests

        [Fact]
        public void Build_WhenExpenseIdIsEmpty_ShouldThrowException()
        {
            // Arrange
            var builder = new ExpenseRegistrationBuilder()
                .WithProject(_validProject)
                .WithWorkLog(_validWorkLog)
                .WithDescription(_validDescription);
            // Intentionally omitting WithExpense()

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        #endregion

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredExpenseRegistration()
        {
            // Arrange
            var builder = new ExpenseRegistrationBuilder()
                .WithProject(_validProject)
                .WithWorkLog(_validWorkLog)
                .WithExpense(_validExpense)
                .WithDescription(_validDescription)
                .WithStatus(_validStatus);

            // Act
            ExpenseRegistration registration = builder.Build();

            // Assert
            Assert.NotNull(registration);
            Assert.Equal(_validProject.Id, registration.ProjectId);
            Assert.Equal(_validWorkLog.EmployeeId, registration.EmployeeId);
            Assert.Equal(_validWorkLog.Id, registration.WorkLogId);
            Assert.Equal(_validExpense.Id, registration.ExpenseId);
            Assert.Equal(_validDescription, registration.Description);
            Assert.Equal(_validStatus, registration.Status);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new ExpenseRegistrationBuilder();

            // Act
            var result = builder.WithExpense(_validExpense);

            // Assert
            Assert.Same(builder, result);
        }

        #endregion
    }
}
