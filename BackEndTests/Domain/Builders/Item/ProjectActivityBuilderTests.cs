using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item
{
    public class ProjectActivityBuilderTests
    {
        private readonly Activity _validActivity = new Activity { Id = Guid.NewGuid() };
        private readonly Project _validProject = new Project { Id = Guid.NewGuid() };
        private readonly Employee _validEmployee = new Employee { Id = Guid.NewGuid() };
        private readonly DateTime _validStartDate = DateTime.UtcNow.AddDays(1);
        private readonly DateTime _validEndDate = DateTime.UtcNow.AddDays(7);
        private readonly Status _validStatus = Status.Åben;

        //method guard tests

        [Fact]
        public void WithStartAndEndDates_WhenEndDateIsBeforeStartDate_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectActivityBuilder();
            var invalidEndDate = _validStartDate.AddDays(-2);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.WithStartAndEndDates(_validStartDate, invalidEndDate));
        }


        //build guard tests

        [Fact]
        public void Build_WhenActivityIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectActivityBuilder()
                .WithProject(_validProject)
                .WithStartAndEndDates(_validStartDate, _validEndDate);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenProjectIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var builder = new ProjectActivityBuilder()
                .WithActivity(_validActivity)
                .WithStartAndEndDates(_validStartDate, _validEndDate);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenDatesAreNotProvided_ShouldThrowExceptionDueToDefaultDateTimeValues()
        {
            // Arrange
            // Default DateTime is DateTime.MinValue. If both are default, 
            var builder = new ProjectActivityBuilder()
                .WithActivity(_validActivity)
                .WithProject(_validProject);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }


        //success tests

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredProjectActivity()
        {
            // Arrange
            var builder = new ProjectActivityBuilder()
                .WithActivity(_validActivity)
                .WithProject(_validProject)
                .WithStartAndEndDates(_validStartDate, _validEndDate)
                .WithStatus(_validStatus)
                .WithResponsibleEmployee(_validEmployee);

            // Act
            ProjectActivity projectActivity = builder.Build();

            // Assert
            Assert.NotNull(projectActivity);
            Assert.Equal(_validActivity.Id, projectActivity.ActivityId);
            Assert.Equal(_validProject.Id, projectActivity.ProjectId);
            Assert.Equal(_validStartDate, projectActivity.StartDate);
            Assert.Equal(_validEndDate, projectActivity.EndDate);
            Assert.Equal(_validStatus, projectActivity.Status);
            Assert.Equal(_validEmployee.Id, projectActivity.ResponsibleEmployeeId);
        }

        [Fact]
        public void Build_WithoutOptionalResponsibleEmployee_ShouldSucceedWithNullEmployeeId()
        {
            // Arrange
            var builder = new ProjectActivityBuilder()
                .WithActivity(_validActivity)
                .WithProject(_validProject)
                .WithStartAndEndDates(_validStartDate, _validEndDate)
                .WithStatus(_validStatus);

            // Act
            ProjectActivity projectActivity = builder.Build();

            // Assert
            Assert.NotNull(projectActivity);
            Assert.Null(projectActivity.ResponsibleEmployeeId);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new ProjectActivityBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithActivity(_validActivity));
            Assert.Same(builder, builder.WithProject(_validProject));
            Assert.Same(builder, builder.WithStartAndEndDates(_validStartDate, _validEndDate));
            Assert.Same(builder, builder.WithStatus(_validStatus));
            Assert.Same(builder, builder.WithResponsibleEmployee(_validEmployee));
        }
    }
}