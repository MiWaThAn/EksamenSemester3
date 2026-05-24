using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using Moq;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests
{
    public class GetDetailedProjectHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetDetailedProjectHandler _handler;

        public GetDetailedProjectHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetDetailedProjectHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ProjectDoesNotExist_ReturnsNull()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Projects.GetByIdWithDetailsAsync(projectId))
                          .ReturnsAsync((Project?)null);

            var query = new GetDetailedProjectQuery(projectId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ProjectExists_ReturnsFullyMappedDetailedProjectModel()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var dummyProject = new Project
            {
                Id = projectId,
                CompanyId = companyId,
                Name = "Storebæltsforbindelsen 2.0",
                Description = "Renovering af pyloner",
                Status = Status.Åben
            };

            dummyProject.Customer = new Customer { Id = Guid.NewGuid(), Name = "Sund & Bælt A/S" };

            var dummyActivityDetails = new Activity { Id = Guid.NewGuid(), Name = "Betonstøbning" };

            var projectActivity = new ProjectActivity
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                ActivityId = dummyActivityDetails.Id,
                Activity = dummyActivityDetails,
                Status = Status.Åben,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(5)
            };

            var internalActivitiesField = typeof(Project).GetField("_activities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (internalActivitiesField != null)
            {
                var list = (List<ProjectActivity>)internalActivitiesField.GetValue(dummyProject)!;
                list.Add(projectActivity);
            }

            var emp1 = new Employee { Id = Guid.NewGuid(), Name = "Klaus Nielsen" };
            var emp2 = new Employee { Id = Guid.NewGuid(), Name = "Berit Friis" };
            var relatedEmployees = new List<Employee> { emp1, emp2 };

            _mockUnitOfWork.Setup(uow => uow.Projects.GetByIdWithDetailsAsync(projectId))
                          .ReturnsAsync(dummyProject);

            _mockUnitOfWork.Setup(uow => uow.Employees.GetEmployeesRelatedToProjectAsync(projectId))
                          .ReturnsAsync(relatedEmployees);

            var query = new GetDetailedProjectQuery(projectId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            Assert.Equal("Storebæltsforbindelsen 2.0", result.ProjectName);
            Assert.Equal("Renovering af pyloner", result.Description);
            Assert.Equal("Åben", result.Status);
            Assert.Equal("Sund & Bælt A/S", result.CustomerName);

            Assert.Single(result.Activities);
            var mappedActivity = result.Activities.First();
            Assert.Equal("Betonstøbning", mappedActivity.ActivityName);
            Assert.Equal("Åben", mappedActivity.Status);
            Assert.Equal(5.0, mappedActivity.TimeEstimate);
            Assert.Equal(5, mappedActivity.ActivityNumber.Length);

            Assert.Equal(2, result.Employees.Count);
            Assert.Equal("Klaus Nielsen", result.Employees[0].FullName);
            Assert.Equal("Berit Friis", result.Employees[1].FullName);
        }
    }
}