using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entity.Person;
using Domain.ValueObjects;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application
{
    public class GetEmployeeByIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetEmployeeByIdHandler _handler;

        public GetEmployeeByIdHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetEmployeeByIdHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_EmployeeDoesNotExist_ReturnsNull()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(employeeId))
                          .ReturnsAsync((Employee?)null);

            var query = new GetEmployeeByIdQuery(employeeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_EmployeeExists_ReturnsMappedEmployeeDTO()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var dummyEmployee = new Employee();
            dummyEmployee.Id = employeeId;
            dummyEmployee.Name = "Søren Poulsen";

            dummyEmployee.EmployeeType = EmployeeType.Formand;

            dummyEmployee.Email = new EmailAddress("soeren@firma.dk");
            dummyEmployee.IsAutonomous = true;

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(employeeId))
                          .ReturnsAsync(dummyEmployee);

            var query = new GetEmployeeByIdQuery(employeeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(dummyEmployee.Id, result.Id);
            Assert.Equal("Søren Poulsen", result.Name);
        }
    }
}