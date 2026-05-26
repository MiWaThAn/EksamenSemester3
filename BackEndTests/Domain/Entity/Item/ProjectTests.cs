using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item
{
    public class ProjectTests
    {
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly string _validName = "Enterprise ERP Implementation";
        private readonly string _validDescription = "Overhauling company-wide logistics software structures.";
        private readonly Status _defaultStatus = Status.Åben;

        private readonly Employee _stubEmployee = new Employee { Id = Guid.NewGuid() };
        private readonly Customer _stubCustomer = new Customer { Id = Guid.NewGuid() };
        private readonly Address _stubAddress = new Address();

        #region Helper Methods

        private Project CreateProjectInstance()
        {
            var project = new Project(
                _validName,
                _validCompanyId,
                _stubCustomer.Id,
                _stubEmployee.Id,
                _validDescription,
                _defaultStatus,
                _stubAddress);

            // Assign a test GUID to Project.Id via reflection if read-only base property
            typeof(Project).GetProperty("Id")?.SetValue(project, Guid.NewGuid());
            return project;
        }

        #endregion

        #region Constructor Constraints

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Act
            var project = new Project(_validName, _validCompanyId, null, null, _validDescription, _defaultStatus, null);

            // Assert
            Assert.Equal(_validName, project.Name);
            Assert.Equal(_validCompanyId, project.CompanyId);
            Assert.Equal(_validDescription, project.Description);
            Assert.Equal(_defaultStatus, project.Status);
            Assert.Empty(project.Activities);
            Assert.Empty(project.Registrations);
            Assert.Empty(project.Assignments);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            Assert.ThrowsAny<Exception>(() => new Project(invalidName, _validCompanyId, null, null, _validDescription, _defaultStatus, null));
        }

        [Fact]
        public void Constructor_WhenCompanyIdIsEmpty_ShouldThrowException()
        {
            Assert.ThrowsAny<Exception>(() => new Project(_validName, Guid.Empty, null, null, _validDescription, _defaultStatus, null));
        }

        #endregion

        #region Assignment Rules (Employees)

        [Fact]
        public void AssignEmployee_WhenNotAssigned_ShouldAddProjectAssignment()
        {
            // Arrange
            var project = CreateProjectInstance();

            // Act
            project.AssignEmployee(_stubEmployee);

            // Assert
            Assert.Single(project.Assignments);
            Assert.Equal(_stubEmployee.Id, project.Assignments.First().EmployeeId);
        }

        [Fact]
        public void AssignEmployee_WhenEmployeeIsAlreadyAssigned_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var project = CreateProjectInstance();
            project.AssignEmployee(_stubEmployee);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => project.AssignEmployee(_stubEmployee));
            Assert.Equal("Medarbejderen er allerede tildelt dette projekt.", exception.Message);
        }

        [Fact]
        public void UnAssignEmployee_WhenAssigned_ShouldRemoveAssignment()
        {
            // Arrange
            var project = CreateProjectInstance();
            project.AssignEmployee(_stubEmployee);

            // Act
            project.UnAssignEmployee(_stubEmployee);

            // Assert
            Assert.Empty(project.Assignments);
        }

        [Fact]
        public void UnAssignEmployee_WhenEmployeeNotAssigned_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var project = CreateProjectInstance();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => project.UnAssignEmployee(_stubEmployee));
            Assert.Equal("Medarbejderen er ikke tildelt dette projekt.", exception.Message);
        }

        #endregion

        #region Project Activity Management

        [Fact]
        public void CreateProjectActivity_WithValidBuilderMatch_ShouldAppendToActivitiesList()
        {
            // Arrange
            var project = CreateProjectInstance();
            var activityId = Guid.NewGuid();

            var builder = new ProjectActivityBuilder()
                .WithStartAndEndDates(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5))
                .WithStatus(Status.Åben);

            // Mocking builder target internals using reflection since Build() extracts project settings
            typeof(ProjectActivityBuilder).GetField("ActivityId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(builder, activityId);

            // Act
            var activity = project.CreateProjectActivity(builder);

            // Assert
            Assert.NotNull(activity);
            Assert.Single(project.Activities);
            Assert.Equal(project.Id, activity.ProjectId);
        }

        [Fact]
        public void RemoveProjectActivity_WhenActivityExists_ShouldExecuteSoftDelete()
        {
            // Arrange
            var project = CreateProjectInstance();
            var activityId = Guid.NewGuid();
            var activity = new ProjectActivity { Id = activityId };

            // Injecting mock activity directly into underlying private list boundary
            var field = typeof(Project).GetField("_activities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ((List<ProjectActivity>)field.GetValue(project)).Add(activity);

            // Act
            project.RemoveProjectActivity(activityId);

            // Assert
            // It should be missing from the public property list because .Activities filters out IsDeleted records
            Assert.Empty(project.Activities);
            Assert.True(activity.IsDeleted);
        }

        #endregion

        #region Value Mutation & Lifecycle Conversions

        [Fact]
        public void LinkToEmployee_WithValidEmployee_ShouldUpdateResponsibleField()
        {
            var project = CreateProjectInstance();
            var alternativeEmployee = new Employee { Id = Guid.NewGuid() };

            project.LinkToEmployee(alternativeEmployee);

            Assert.Equal(alternativeEmployee.Id, project.ResponsibleEmployeeId);
        }

        [Fact]
        public void StatusTransitions_ShouldAlterStateCorrectly()
        {
            var project = CreateProjectInstance();

            project.MarkAsClosed();
            Assert.Equal(Status.Lukket, project.Status);

            project.MarkAsOnHold();
            Assert.Equal(Status.Godkendes, project.Status);

            project.MarkAsOpen();
            Assert.Equal(Status.Åben, project.Status);
        }

        [Fact]
        public void UpdateProjectName_WithValidData_ShouldAlterProperty()
        {
            var project = CreateProjectInstance();
            string updatedName = "Completely Rewritten Architecture Blueprint Name";

            project.UpdateProjectName(updatedName);

            Assert.Equal(updatedName, project.Name);
        }

        #endregion
    }
}
