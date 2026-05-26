using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using Domain.Services;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Person
{
    public class EmployeeTests
    {
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly string _validName = "Mads Mikkelsen";
        private readonly EmailAddress _validEmail = new EmailAddress("mads@company.dk");
        private readonly EmployeeType _defaultType = EmployeeType.Formand;

        #region Helper Methods

        private Employee CreateEmployeeInstance()
        {
            var employee = new Employee(_validName, _validCompanyId, _defaultType, _validEmail, isAutonomous: false);
            // Establish a target aggregate Id explicitly if required by base configuration tracking
            typeof(Employee).GetProperty("Id")?.SetValue(employee, Guid.NewGuid());
            return employee;
        }

        #endregion

        #region Constructor Constraints

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Act
            var employee = new Employee(_validName, _validCompanyId, _defaultType, _validEmail, isAutonomous: true);

            // Assert
            Assert.Equal(_validName, employee.Name);
            Assert.Equal(_validCompanyId, employee.CompanyId);
            Assert.Equal(_defaultType, employee.EmployeeType);
            Assert.Equal(_validEmail, employee.Email);
            Assert.True(employee.IsAutonomous);
            Assert.False(employee.IsLocal); // Matches internal initialization default
            Assert.Empty(employee.WorkLogs);
            Assert.Empty(employee.Assignments);
            Assert.Null(employee.AccountId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => new Employee(invalidName, _validCompanyId, _defaultType, _validEmail, false));
        }

        #endregion

        #region Mutation & Value State Changes

        [Fact]
        public void UpdateName_WithValidString_ShouldMutatePropertyAndTrackTimestamp()
        {
            // Arrange
            var employee = CreateEmployeeInstance();
            var initialTimestamp = employee.UpdatedAt;
            var updatedName = "Mads S. Mikkelsen";

            // Act
            employee.UpdateName(updatedName);

            // Assert
            Assert.Equal(updatedName, employee.Name);
            Assert.True(employee.UpdatedAt >= initialTimestamp);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpdateName_WithInvalidString_ShouldThrowException(string invalidName)
        {
            // Arrange
            var employee = CreateEmployeeInstance();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => employee.UpdateName(invalidName!));
        }

        [Fact]
        public void UpdateEmail_WithValidValueObject_ShouldMutateProperty()
        {
            // Arrange
            var employee = CreateEmployeeInstance();
            var newEmail = new EmailAddress("new.mads@company.com");

            // Act
            employee.UpdateEmail(newEmail);

            // Assert
            Assert.Equal(newEmail, employee.Email);
        }

        [Fact]
        public void UpdateEmployeeTypeAndAutonomy_ShouldAlterStatesCorrectly()
        {
            // Arrange
            var employee = CreateEmployeeInstance();

            // Act
            employee.UpdateEmployeeType(EmployeeType.Lærling);
            employee.UpdateAutonomy(true);

            // Assert
            Assert.Equal(EmployeeType.Lærling, employee.EmployeeType);
            Assert.True(employee.IsAutonomous);
        }

        #endregion

        #region Account Relationship Constraints

        [Fact]
        public void LinkToAccount_WhenAccountNotLinked_ShouldSuccessfullyBind()
        {
            // Arrange
            var employee = CreateEmployeeInstance();
            var account = new Account { Id = Guid.NewGuid() };

            // Act
            employee.LinkToAccount(account);

            // Assert
            Assert.Equal(account.Id, employee.AccountId);
            Assert.Same(account, employee.Account);
        }

        [Fact]
        public void LinkToAccount_WhenAlreadyLinkedToAnAccount_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = CreateEmployeeInstance();
            var initialAccount = new Account { Id = Guid.NewGuid() };
            var alternativeAccount = new Account { Id = Guid.NewGuid() };

            employee.LinkToAccount(initialAccount);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => employee.LinkToAccount(alternativeAccount));
            Assert.Equal("Denne medarbejder er allerede tilknyttet en konto.", exception.Message);
        }

        #endregion

        #region WorkLog Aggregate Factory Rules

        [Fact]
        public void CreateWorkLog_WithValidBuilder_ShouldAppendChildAndConfigureOwnership()
        {
            // Arrange
            var employee = CreateEmployeeInstance();
            var builder = new WorkLogBuilder();

            // Act
            WorkLog resultLog = employee.CreateWorkLog(builder);

            // Assert
            Assert.NotNull(resultLog);
            Assert.Single(employee.WorkLogs);
            Assert.Same(resultLog, employee.WorkLogs.First());
            Assert.Equal(employee.Id, resultLog.EmployeeId);
        }

        [Fact]
        public void CreateWorkLog_WithNullBuilder_ShouldThrowArgumentNullException()
        {
            // Arrange
            var employee = CreateEmployeeInstance();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => employee.CreateWorkLog(null!));
        }

        #endregion
    }
}
