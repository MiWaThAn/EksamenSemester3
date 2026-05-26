using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item.Registrations.ExpenseRegistrationsTests
{
    public class ExpenseRegistrationTests
    {
        private readonly Guid _validProjectId = Guid.NewGuid();
        private readonly Guid _validExpenseId = Guid.NewGuid();
        private readonly string _validDescription = "Client Dinner Expense";
        private readonly RegistrationStatus _initialStatus = RegistrationStatus.Pending;
        private readonly WorkLog _stubWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };

        //constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectlyAndSetDateToUtcNow()
        {
            // Arrange & Act
            var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
            var registration = new ExpenseRegistration(
                _validProjectId,
                _stubWorkLog,
                null,
                _validExpenseId,
                _validDescription,
                _initialStatus);
            var afterCreation = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.Equal(_validExpenseId, registration.ExpenseId);
            Assert.Equal(_validProjectId, registration.ProjectId);
            Assert.Equal(_stubWorkLog.EmployeeId, registration.EmployeeId);
            Assert.Equal(_validDescription, registration.Description);
            Assert.Equal(_initialStatus, registration.Status);

            // Verify Date fallback defaults roughly to UtcNow during creation
            Assert.True(registration.Date >= beforeCreation && registration.Date <= afterCreation);
        }

        [Fact]
        public void Constructor_WhenExpenseIdIsEmpty_ShouldThrowException()
        {
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => new ExpenseRegistration(
                _validProjectId,
                _stubWorkLog,
                null,
                Guid.Empty,
                _validDescription,
                _initialStatus));
        }

        //validateAgainst Tests

        [Fact]
        public void ValidateAgainst_WhenRegistrationIdAlreadyExists_ShouldThrowArgumentException()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, _initialStatus);


            typeof(Registration).GetProperty("Id")?.SetValue(registration, registrationId);

            var duplicateRegistration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, "Another text", _initialStatus);
            typeof(Registration).GetProperty("Id")?.SetValue(duplicateRegistration, registrationId);

            var existingRegistrations = new List<Registration> { duplicateRegistration };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => registration.ValidateAgainst(existingRegistrations));
            Assert.Contains("Denne registrering er allerede tilføjet til medarbejderen.", exception.Message);
        }



        //mutation & Guard Tests

        [Fact]
        public void UpdateExpense_WithValidGuid_ShouldUpdateExpenseIdAndMarkAsPending()
        {
            // Arrange
            // Set initial state to Afvist so we can track if it transitions back to Pending
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, RegistrationStatus.Afvist);
            var newExpenseId = Guid.NewGuid();

            // Act
            registration.UpdateExpense(newExpenseId);

            // Assert
            Assert.Equal(newExpenseId, registration.ExpenseId);
            Assert.Equal(RegistrationStatus.Pending, registration.Status);
            Assert.NotNull(registration.UpdatedAt);
        }

        [Fact]
        public void UpdateExpense_WithEmptyGuid_ShouldThrowException()
        {
            // Arrange
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, _initialStatus);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => registration.UpdateExpense(Guid.Empty));
        }

        [Fact]
        public void UpdateDate_WithValidDate_ShouldUpdateDateAndMarkAsPending()
        {
            // Arrange
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, RegistrationStatus.Afvist);
            var testDate = new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc);

            // Act
            registration.UpdateDate(testDate);

            // Assert
            Assert.Equal(testDate, registration.Date);
            Assert.Equal(RegistrationStatus.Pending, registration.Status);
        }

        [Fact]
        public void UpdateAmount_WithPositiveValue_ShouldUpdateAmountAndMarkAsPending()
        {
            // Arrange
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, RegistrationStatus.Afvist);
            decimal validAmount = 450.50m;

            // Act
            registration.UpdateAmount(validAmount);

            // Assert
            Assert.Equal(validAmount, registration.Amount);
            Assert.Equal(RegistrationStatus.Pending, registration.Status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100.50)]
        public void UpdateAmount_WhenValueIsNegativeOrZero_ShouldThrowException(decimal invalidAmount)
        {
            // Arrange
            var registration = new ExpenseRegistration(_validProjectId, _stubWorkLog, null, _validExpenseId, _validDescription, _initialStatus);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => registration.UpdateAmount(invalidAmount));
        }
    }
}
