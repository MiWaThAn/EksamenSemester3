using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Entity.Person;
using Domain.Entity.Item; // Antager at jeres Project ligger her, tilpas hvis det er under Person
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
    public class GetProjectsByEmployeeHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetProjectsByEmployeeHandler _handler;

        public GetProjectsByEmployeeHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetProjectsByEmployeeHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Account?)null);

            var query = new GetProjectsByEmployeeQuery(accountId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Konto ikke fundet.", exception.Message);
        }

        [Fact]
        public async Task Handle_AccountIsNotEmployee_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.EmployeeId = null;

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            var query = new GetProjectsByEmployeeQuery(accountId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Kun medarbejdere kan hente personlige projekter.", exception.Message);
        }

        [Fact]
        public async Task Handle_NoProjectsFound_ReturnsEmptyEnumerable()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.EmployeeId = employeeId;

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            _mockUnitOfWork.Setup(uow => uow.Projects.GetProjectsRelatedToEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((IEnumerable<Project>?)null);

            var query = new GetProjectsByEmployeeQuery(accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedCompanyProjects()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.EmployeeId = employeeId;

            var proj1 = new Project { Id = Guid.NewGuid(), Name = "Eksamensprojekt Semester 3" };
            var proj2 = new Project { Id = Guid.NewGuid(), Name = "Intern tidsregistrerings-app" };
            var dummyProjects = new List<Project> { proj1, proj2 };

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            _mockUnitOfWork.Setup(uow => uow.Projects.GetProjectsRelatedToEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyProjects);

            var query = new GetProjectsByEmployeeQuery(accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var projectList = result.ToList();
            Assert.Equal(2, projectList.Count);

            Assert.Equal(proj1.Id, projectList[0].Id);
            Assert.Equal("Eksamensprojekt Semester 3", projectList[0].ProjectName);

            Assert.Equal(proj2.Id, projectList[1].Id);
            Assert.Equal("Intern tidsregistrerings-app", projectList[1].ProjectName);
        }
    }
}