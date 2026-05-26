using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using System;
using Xunit;

namespace BackEndTests.Domain.Entity.Item.Activities
{
    public class ProjectActivityTests
    {
        private readonly Guid _validActivityId = Guid.NewGuid();
        private readonly Guid _validProjectId = Guid.NewGuid();
        private readonly DateTime _validStartDate = DateTime.UtcNow.AddDays(1);
        private readonly DateTime _validEndDate = DateTime.UtcNow.AddDays(5);
        private readonly Guid _validEmployeeId = Guid.NewGuid();
        private readonly Status _defaultStatus = Status.Åben;

        //constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            var projectActivity = new ProjectActivity(
                _validActivityId,
                _validProjectId,
                _validStartDate,
                _validEndDate,
                _validEmployeeId,
                _defaultStatus);

            Assert.Equal(_validActivityId, projectActivity.ActivityId);
            Assert.Equal(_validProjectId, projectActivity.ProjectId);
            Assert.Equal(_validStartDate, projectActivity.StartDate);
            Assert.Equal(_validEndDate, projectActivity.EndDate);
            Assert.Equal(_validEmployeeId, projectActivity.ResponsibleEmployeeId);
            Assert.Equal(_defaultStatus, projectActivity.Status);
        }

        [Fact]
        public void Constructor_WhenActivityIdIsEmpty_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectActivity(
                Guid.Empty,
                _validProjectId,
                _validStartDate,
                _validEndDate,
                _validEmployeeId,
                _defaultStatus));
        }

        [Fact]
        public void Constructor_WhenProjectIdIsEmpty_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectActivity(
                _validActivityId,
                Guid.Empty,
                _validStartDate,
                _validEndDate,
                _validEmployeeId,
                _defaultStatus));
        }

        [Fact]
        public void Constructor_WhenEndDateIsBeforeStartDate_ShouldThrowException()
        {
            // Arrange
            var invalidEndDate = _validStartDate.AddDays(-1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectActivity(
                _validActivityId,
                _validProjectId,
                _validStartDate,
                invalidEndDate,
                _validEmployeeId,
                _defaultStatus));
        }

        [Fact]
        public void Constructor_WithNullResponsibleEmployee_ShouldBeAllowed()
        {
            // Act
            var projectActivity = new ProjectActivity(
                _validActivityId,
                _validProjectId,
                _validStartDate,
                _validEndDate,
                null,
                _defaultStatus);

            // Assert
            Assert.Null(projectActivity.ResponsibleEmployeeId);
        }


        //status tests

        [Fact]
        public void MarkAsClosed_ShouldSetStatusToLukketAndSetTimestamp()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, Status.Åben);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            projectActivity.MarkAsClosed();

            // Assert
            Assert.Equal(Status.Lukket, projectActivity.Status);
            Assert.True(projectActivity.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void MarkAsOpen_ShouldSetStatusToÅbenAndSetTimestamp()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, Status.Lukket);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            projectActivity.MarkAsOpen();

            // Assert
            Assert.Equal(Status.Åben, projectActivity.Status);
            Assert.True(projectActivity.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void MarkAsOnHold_ShouldSetStatusToGodkendesAndSetTimestamp()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, Status.Åben);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            projectActivity.MarkAsOnHold();

            // Assert
            Assert.Equal(Status.Godkendes, projectActivity.Status);
            Assert.True(projectActivity.UpdatedAt >= beforeUpdate);
        }


        //assignment Tests

        [Fact]
        public void AssignResponsibleEmployee_WithValidId_ShouldUpdateEmployeeAndTimestamp()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, _defaultStatus);
            var newEmployeeId = Guid.NewGuid();
            var beforeUpdate = DateTime.UtcNow;

            // Act
            projectActivity.AssignResponsibleEmployee(newEmployeeId);

            // Assert
            Assert.Equal(newEmployeeId, projectActivity.ResponsibleEmployeeId);
            Assert.True(projectActivity.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void AssignResponsibleEmployee_WhenEmployeeIdIsEmpty_ShouldThrowException()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, _defaultStatus);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => projectActivity.AssignResponsibleEmployee(Guid.Empty));
        }
        
        
        //date range update tests

        [Fact]
        public void UpdateStartAndEndDates_WithValidRange_ShouldUpdateDatesAndTimestamp()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, _defaultStatus);
            var newStartDate = DateTime.UtcNow.AddDays(10);
            var newEndDate = DateTime.UtcNow.AddDays(15);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            projectActivity.UpdateStartAndEndDates(newStartDate, newEndDate);

            // Assert
            Assert.Equal(newStartDate, projectActivity.StartDate);
            Assert.Equal(newEndDate, projectActivity.EndDate);
            Assert.True(projectActivity.UpdatedAt >= beforeUpdate);
        }

        [Fact]
        public void UpdateStartAndEndDates_WhenEndDateIsBeforeStartDate_ShouldThrowException()
        {
            // Arrange
            var projectActivity = new ProjectActivity(_validActivityId, _validProjectId, _validStartDate, _validEndDate, null, _defaultStatus);
            var newStartDate = DateTime.UtcNow.AddDays(10);
            var invalidEndDate = DateTime.UtcNow.AddDays(9);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => projectActivity.UpdateStartAndEndDates(newStartDate, invalidEndDate));
        }
    }
}