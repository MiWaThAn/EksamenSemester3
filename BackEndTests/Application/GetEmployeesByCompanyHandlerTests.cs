using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Moq;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application
{
    public class GetEmployeesByCompanyHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetEmployeesByCompanyHandler _handler;

        public GetEmployeesByCompanyHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetEmployeesByCompanyHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Account?)null);

            var query = new GetEmployeesByCompanyQuery(Guid.NewGuid(), Guid.NewGuid());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Bruger-konto ikke fundet.", exception.Message);
        }

        [Fact]
        public async Task Handle_UserIsEmployeeAndNotAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.EmployeeId = Guid.NewGuid();

            var employeeRole = new Role("Employee");
            employeeRole.Id = Guid.NewGuid();
            dummyAccount.AddRole(employeeRole);

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            var query = new GetEmployeesByCompanyQuery(Guid.NewGuid(), accountId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Du har ikke tilladelse til at se dette firmas administrationspanel.", exception.Message);
        }

        [Fact]
        public async Task Handle_CompanyIdMismatchForNonAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var userCompanyId = Guid.NewGuid();
            var requestedCompanyId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.CompanyId = userCompanyId;
            dummyAccount.EmployeeId = null;

            var managerRole = new Role("Manager");
            managerRole.Id = Guid.NewGuid();
            dummyAccount.AddRole(managerRole);

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            var query = new GetEmployeesByCompanyQuery(requestedCompanyId, accountId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Du kan kun administrere medarbejdere for din egen virksomhed.", exception.Message);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedEmployees()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var dummyAccount = new Account
            {
                Id = accountId,
                CompanyId = companyId
            };

            var adminRole = new Role("Admin") { Id = Guid.NewGuid() };
            dummyAccount.AddRole(adminRole);

            var dummyCompany = new Company();
            dummyCompany.Id = companyId;

            var builder1 = new EmployeeBuilder().WithName("Søren Hansen");
            var builder2 = new EmployeeBuilder().WithName("Mette Jensen");

            var emp1 = dummyCompany.CreateEmployee(builder1);
            var emp2 = dummyCompany.CreateEmployee(builder2);

            emp1.Id = Guid.NewGuid();
            emp2.Id = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(dummyAccount);

            _mockUnitOfWork.Setup(uow => uow.Companies.GetWithEmployeesAsync(companyId))
                          .ReturnsAsync(dummyCompany);

            var query = new GetEmployeesByCompanyQuery(companyId, accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var employeeList = result.ToList();
            Assert.Equal(2, employeeList.Count);

            Assert.Equal("Søren Hansen", employeeList[0].FullName);
            Assert.Equal("Mette Jensen", employeeList[1].FullName);
        }
    }
}