using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item.RegistrationBuildersTests
{
    public class HourRegistrationBuilderTests
    {
        private readonly Project _validProject = new Project { Id = Guid.NewGuid() };
        private readonly WorkLog _validWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };
        private readonly DateTime _validStart = DateTime.UtcNow.AddHours(-4);
        private readonly DateTime _validEnd = DateTime.UtcNow.AddHours(-2);
        private readonly string _validDescription = "Developing new business features.";
        private readonly RegistrationStatus _validStatus = RegistrationStatus.Pending;

        #region Success Tests (Happy Path)

        [Fact]
        public void Build_WithAllValidParameters_ShouldReturnConfiguredHourRegistration()
        {
            // Arrange
            var builder = new HourRegistrationBuilder()
                .WithProject(_validProject)
                .WithWorkLog(_validWorkLog)
                .WithStart(_validStart)
                .WithEnd(_validEnd)
                .WithDescription(_validDescription)
                .WithStatus(_validStatus);

            // Act
            HourRegistration hourRegistration = builder.Build();

            // Assert
            Assert.NotNull(hourRegistration);
            Assert.Equal(_validProject.Id, hourRegistration.ProjectId);
            Assert.Equal(_validWorkLog.EmployeeId, hourRegistration.EmployeeId);
            Assert.Equal(_validWorkLog.Id, hourRegistration.WorkLogId);
            Assert.Equal(_validDescription, hourRegistration.Description);
            Assert.Equal(_validStatus, hourRegistration.Status);

            // Verify structural time mechanics mapped to properties via intervals
            Assert.Equal(_validStart, hourRegistration.StartTime);
            Assert.Equal(_validEnd, hourRegistration.EndTime);
            Assert.True(hourRegistration.IsFinished);
        }

        [Fact]
        public void Build_WithoutOptionalEndTime_ShouldReturnUnfinishedHourRegistration()
        {
            // Arrange
            var builder = new HourRegistrationBuilder()
                .WithProject(_validProject)
                .WithWorkLog(_validWorkLog)
                .WithStart(_validStart)
                .WithDescription(_validDescription)
                .WithStatus(_validStatus);
            // Leaving WithEnd out completely

            // Act
            HourRegistration hourRegistration = builder.Build();

            // Assert
            Assert.NotNull(hourRegistration);
            Assert.Equal(_validStart, hourRegistration.StartTime);
            Assert.Null(hourRegistration.EndTime);
            Assert.False(hourRegistration.IsFinished);
        }

        [Fact]
        public void FluentMethods_ShouldReturnSameBuilderInstance()
        {
            // Arrange
            var builder = new HourRegistrationBuilder();

            // Act & Assert
            Assert.Same(builder, builder.WithStart(_validStart));
            Assert.Same(builder, builder.WithEnd(_validEnd));
            Assert.Same(builder, builder.WithType(TimeType.Work));
        }

        #endregion
    }
}
