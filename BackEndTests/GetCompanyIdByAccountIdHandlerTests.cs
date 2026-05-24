using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Entity.Person;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests
{
    public class GetCompanyIdByAccountIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetCompanyIdByAccountIdHandler _handler;

        public GetCompanyIdByAccountIdHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetCompanyIdByAccountIdHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ReturnsGuidEmpty()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId))
                          .ReturnsAsync((Account?)null);

            var query = new GetCompanyIdByAccountIdQuery(accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task Handle_AccountHasNoCompanyId_ReturnsGuidEmpty()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.CompanyId = null;

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId))
                          .ReturnsAsync(dummyAccount);

            var query = new GetCompanyIdByAccountIdQuery(accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task Handle_AccountHasCompanyId_ReturnsExpectedCompanyId()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var expectedCompanyId = Guid.NewGuid();

            var dummyAccount = new Account();
            dummyAccount.Id = accountId;
            dummyAccount.CompanyId = expectedCompanyId;

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId))
                          .ReturnsAsync(dummyAccount);

            var query = new GetCompanyIdByAccountIdQuery(accountId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCompanyId, result);
        }
    }
}