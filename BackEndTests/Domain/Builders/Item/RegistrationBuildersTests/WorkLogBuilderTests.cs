using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item.RegistrationBuildersTests
{
    public class WorkLogBuilderTests
    {
        private readonly Employee _validEmployee = new Employee { Id = Guid.NewGuid() };

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithValidEmployee_ShouldReturnConfiguredWorkLog()
        {
            // Arrange
            var builder = new WorkLogBuilder()
                .WithEmployee(_validEmployee);

            // Act
            WorkLog workLog = builder.Build();

            // Assert
            Assert.NotNull(workLog);
            Assert.Equal(_validEmployee.Id, workLog.EmployeeId);

            // Verify default aggregate base states are set correctly on construction
            Assert.False(workLog.IsClosed);
            Assert.False(workLog.HasActiveRegistration);
            Assert.Equal(ApprovalStatus.Draft, workLog.Status);
        }

        [Fact]
        public void Build_WhenEmployeeIsNull_ShouldThrowExceptionViaEntityConstructor()
        {
            // Arrange
            var builder = new WorkLogBuilder()
                .WithEmployee(null!);

            // Act & Assert
            // The builder doesn't guard internally, but WorkLog's internal constructor 
            // calls Guard.AgainstNull(employee), meaning Build() will safely bubble up that exception.
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new WorkLogBuilder();

            // Act
            var result = builder.WithEmployee(_validEmployee);

            // Assert
            Assert.Same(builder, result);
        }

        #endregion
    }
}
