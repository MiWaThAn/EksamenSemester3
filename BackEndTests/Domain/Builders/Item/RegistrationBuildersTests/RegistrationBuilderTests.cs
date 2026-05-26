using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Builders.Item.RegistrationBuildersTests
{
    #region Test Stubs for Abstract Verification

    // Concrete mock entity to satisfy the generic TEntity constraint
    public class TestRegistrationEntity : Registration
    {
        public TestRegistrationEntity(Guid projectId, WorkLog workLog, Guid? activityId, string? description, RegistrationStatus status)
            : base(projectId, workLog, activityId, description, status) { }
    }

    // Concrete mock builder to expose and test the abstract base class logic
    public class TestRegistrationBuilder : RegistrationBuilder<TestRegistrationBuilder, TestRegistrationEntity>
    {
        // Expose protected fields for explicit assertion checks
        public WorkLog ExposedWorkLog => WorkLog;
        public Guid? ExposedActivityId => ActivityId;
        public Guid ExposedProjectId => ProjectId;
        public RegistrationStatus ExposedStatus => Status;
        public string? ExposedDescription => Description;

        internal override TestRegistrationEntity Build()
        {
            // Simple pass-through implementation to fulfill the contract
            return new TestRegistrationEntity(ProjectId, WorkLog, ActivityId, Description, Status);
        }
    }

    #endregion

    public class RegistrationBuilderTests
    {
        private readonly WorkLog _validWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };
        private readonly Project _validProject = new Project { Id = Guid.NewGuid() };
        private readonly ProjectActivity _validProjectActivity = new ProjectActivity { Id = Guid.NewGuid() };
        private readonly string _validDescription = "Completed standard maintenance operations.";

        #region Fluent Guard Validation Tests

        [Fact]
        public void WithWorkLog_WhenWorkLogIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithWorkLog(null!));
        }

        [Fact]
        public void WithProjectActivity_WhenActivityIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithProjectActivity((ProjectActivity)null!));
        }

        [Fact]
        public void WithProject_WhenProjectIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithProject(null!));
        }

        [Fact]
        public void WithDescription_WhenDescriptionIsNull_ShouldThrowException()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => builder.WithDescription(null!));
        }

        #endregion

        #region Fluent State Assignment Tests

        [Fact]
        public void Builder_ShouldHavePendingAsDefaultStatus()
        {
            // Arrange & Act
            var builder = new TestRegistrationBuilder();

            // Assert
            Assert.Equal(RegistrationStatus.Pending, builder.ExposedStatus);
        }

        [Fact]
        public void WithValidParameters_ShouldPopulateFieldsCorrectly()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();
            var manualActivityGuid = Guid.NewGuid();
            var manualProjectGuid = Guid.NewGuid();

            // Act
            builder.WithWorkLog(_validWorkLog)
                   .WithProjectActivity(_validProjectActivity)
                   .WithProject(_validProject)
                   .WithDescription(_validDescription)
                   .WithStatus(RegistrationStatus.Godkendt);

            // Assert
            Assert.Same(_validWorkLog, builder.ExposedWorkLog);
            Assert.Equal(_validProjectActivity.Id, builder.ExposedActivityId);
            Assert.Equal(_validProject.Id, builder.ExposedProjectId);
            Assert.Equal(_validDescription, builder.ExposedDescription);
            Assert.Equal(RegistrationStatus.Godkendt, builder.ExposedStatus);
        }

        [Fact]
        public void InternalOverloads_WithGuids_ShouldPopulateFieldsCorrectly()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();
            var manualActivityGuid = Guid.NewGuid();
            var manualProjectGuid = Guid.NewGuid();

            // Act
            builder.WithProjectActivity(manualActivityGuid)
                   .WithProject(manualProjectGuid);

            // Assert
            Assert.Equal(manualActivityGuid, builder.ExposedActivityId);
            Assert.Equal(manualProjectGuid, builder.ExposedProjectId);
        }

        [Fact]
        public void FluentMethods_ShouldReturnDerivedBuilderInstance()
        {
            // Arrange
            var builder = new TestRegistrationBuilder();

            // Act
            var logResult = builder.WithWorkLog(_validWorkLog);
            var projectResult = builder.WithProject(_validProject);
            var descResult = builder.WithDescription(_validDescription);
            var statusResult = builder.WithStatus(RegistrationStatus.Pending);

            // Assert
            Assert.Same(builder, logResult);
            Assert.Same(builder, projectResult);
            Assert.Same(builder, descResult);
            Assert.Same(builder, statusResult);
        }

        #endregion
    }
}
