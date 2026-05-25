using Application.Commands.Person;
using Application.Commands.Person.Handlers;
using Application.Interfaces;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Domain.ValueObjects;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application
{
    public class UpdateEmployeeDetailsHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateEmployeeDetailsHandler _handler;

        public UpdateEmployeeDetailsHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateEmployeeDetailsHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_EmployeeDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdWithAccountAsync(employeeId))
                          .ReturnsAsync((Employee?)null);

            var command = new UpdateEmployeeDetailsCommand(
                employeeId,
                "Test Navn",
                "test@example.com",
                "12345678"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_EmployeeExistsWithoutAccount_UpdatesEmployeeAndReturnsTrue()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var dummyEmployee = new Employee();
            dummyEmployee.Id = employeeId;
            dummyEmployee.Account = null!;

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdWithAccountAsync(employeeId))
                          .ReturnsAsync(dummyEmployee);

            var command = new UpdateEmployeeDetailsCommand(
                employeeId,
                "Mads Nielsen",
                "mads@test.dk",
                "88888888"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal("Mads Nielsen", dummyEmployee.Name);

            Assert.Equal("mads@test.dk", dummyEmployee.Email?.Value);

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_EmployeeExistsWithAccount_UpdatesEmployeeAndAccountAndReturnsTrue()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var dummyEmployee = new Employee();
            dummyEmployee.Id = employeeId;

            var dummyAccount = new Account();
            dummyAccount.Id = Guid.NewGuid();

            dummyEmployee.Account = dummyAccount;

            _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdWithAccountAsync(employeeId))
                          .ReturnsAsync(dummyEmployee);

            var command = new UpdateEmployeeDetailsCommand(
                employeeId,
                "Freja Jensen",
                "freja@firma.dk",
                "45454545"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal("Freja Jensen", dummyEmployee.Name);

            Assert.Equal("freja@firma.dk", dummyEmployee.Email?.Value);

            Assert.NotNull(dummyEmployee.Account);
            Assert.Equal("45454545", dummyEmployee.Account.PhoneNumber?.Value);

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
    }
}