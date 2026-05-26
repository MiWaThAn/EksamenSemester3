using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entity.Person;
using Domain.Builders.Person;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application
{
    public class GetCompanyByIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetCompanyByIdHandler _handler;

        public GetCompanyByIdHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetCompanyByIdHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_CompanyDoesNotExist_ReturnsNull()
        {
            // Arrange
            var companyId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Companies.GetWithEmployeesAsync(companyId))
                          .ReturnsAsync((Company?)null);

            var query = new GetCompanyByIdQuery(companyId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_CompanyExists_ReturnsMappedCompanyDTO()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var account = new Account();

            var dummyCompany = new Company("TestFirma A/S", null!, account, null!);
            dummyCompany.Id = companyId;

            var empBuilder = new EmployeeBuilder().WithName("Ansatte 1");
            dummyCompany.CreateEmployee(empBuilder);

            _mockUnitOfWork.Setup(uow => uow.Companies.GetWithEmployeesAsync(companyId))
                          .ReturnsAsync(dummyCompany);

            var query = new GetCompanyByIdQuery(companyId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(companyId, result.Id);
            Assert.Equal("TestFirma A/S", result.Name);

        }
    }
}