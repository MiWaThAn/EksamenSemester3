using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Entity.Item;
using Domain.Entity.Person;
using Domain.ValueObjects;
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
    public class GetDetailedEmployeeHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetDetailedEmployeeHandler _handler;

        public GetDetailedEmployeeHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetDetailedEmployeeHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_EmployeeDoesNotExist_ReturnsNull()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdWithAccountAsync(employeeId))
                          .ReturnsAsync((Employee?)null);

            var query = new GetDetailedEmployeeQuery(employeeId, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_EmployeeExists_ReturnsFullyMappedDetailedEmployeeModel()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var dummyEmployee = new Employee
            {
                Id = employeeId,
                Name = "Christian Hansen",
                IsLocal = true,
                Email = new EmailAddress("christian@firma.dk")
            };

            var dummyAccount = new Account
            {
                Id = Guid.NewGuid(),
                PhoneNumber = new PhoneNumber("12345678")
            };
            dummyEmployee.Account = dummyAccount;

            var proj1 = new Project { Id = Guid.NewGuid(), Name = "Supermarked Nybyg" };
            var proj2 = new Project { Id = Guid.NewGuid(), Name = "Kontor Renovering" };
            var dummyProjects = new List<Project> { proj1, proj2 };

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdWithAccountAsync(employeeId))
                          .ReturnsAsync(dummyEmployee);

            _mockUnitOfWork.Setup(uow => uow.Projects.GetProjectsRelatedToEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyProjects);

            var query = new GetDetailedEmployeeQuery(employeeId, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("Christian Hansen", result.FullName);
            Assert.Equal("christian@firma.dk", result.Email);
            Assert.Equal("12345678", result.MobileNumber);
            Assert.True(result.IsLocal);

            Assert.Equal(2, result.Projects.Count);
            Assert.Equal(proj1.Id, result.Projects[0].Id);
            Assert.Equal("Supermarked Nybyg", result.Projects[0].ProjectName);

            Assert.Equal(proj2.Id, result.Projects[1].Id);
            Assert.Equal("Kontor Renovering", result.Projects[1].ProjectName);
        }
    }
}