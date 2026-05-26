using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item
{
    public class ExpenseTests
    {
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly string _validName = "Software License Fee";
        private readonly Company _owningCompany;
        private readonly Company _wrongCompany;

        public ExpenseTests()
        {
            // Set up standardized companies for tenant isolation testing
            _owningCompany = new Company { Id = _validCompanyId };
            _wrongCompany = new Company { Id = Guid.NewGuid() };
        }

        //constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Act
            var expense = new Expense(_validName, _validCompanyId);

            // Assert
            Assert.Equal(_validName, expense.Name);
            Assert.Equal(_validCompanyId, expense.CompanyId);
            Assert.Equal(ApprovalStatus.Draft, expense.Status);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => new Expense(invalidName, _validCompanyId));
        }

        [Fact]
        public void Constructor_WhenCompanyIdIsEmpty_ShouldThrowException()
        {
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => new Expense(_validName, Guid.Empty));
        }
        
        //updateExpenseName Tests

        [Fact]
        public void UpdateExpenseName_WithValidName_ShouldUpdateNameAndResetStatusToDraft()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);
            expense.SubmitForApproval(_owningCompany); // Move it out of Draft state first
            var newName = "Updated Software License Fee";
            var beforeUpdate = DateTime.UtcNow;

            // Act
            expense.UpdateExpenseName(newName);

            // Assert
            Assert.Equal(newName, expense.Name);
            Assert.Equal(ApprovalStatus.Draft, expense.Status);
            Assert.True(expense.UpdatedAt >= beforeUpdate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpdateExpenseName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => expense.UpdateExpenseName(invalidName));
        }



        //SubmitForApproval Tests

        [Fact]
        public void SubmitForApproval_WithOwningCompany_ShouldSetStatusToPending()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            expense.SubmitForApproval(_owningCompany);

            // Assert
            Assert.Equal(ApprovalStatus.Pending, expense.Status);
            Assert.True(expense.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void SubmitForApproval_WithWrongCompany_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => expense.SubmitForApproval(_wrongCompany));
            Assert.Equal("Company does not own this expense.", exception.Message);
        }

        [Fact]
        public void SubmitForApproval_WithNullCompany_ShouldThrowException()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => expense.SubmitForApproval(null));
        }

        //approve Tests

        [Fact]
        public void Approve_WithOwningCompany_ShouldSetStatusToApproved()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            expense.Approve(_owningCompany);

            // Assert
            Assert.Equal(ApprovalStatus.Approved, expense.Status);
            Assert.True(expense.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void Approve_WithWrongCompany_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => expense.Approve(_wrongCompany));
            Assert.Equal("Company does not own this expense.", exception.Message);
        }

        //reject tests
        [Fact]
        public void Reject_WithAnyNonNullCompany_ShouldSetStatusToRejected()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            expense.Reject(_owningCompany);

            // Assert
            Assert.Equal(ApprovalStatus.Rejected, expense.Status);
            Assert.True(expense.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void Reject_WithNullCompany_ShouldThrowException()
        {
            // Arrange
            var expense = new Expense(_validName, _validCompanyId);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => expense.Reject(null));
        }
    }
}
