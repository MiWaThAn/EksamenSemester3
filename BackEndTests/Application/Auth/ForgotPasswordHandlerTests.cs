using Application.Commands.Account;
using Application.Commands.Account.Handlers;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entity.Person;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application.Auth
{
    public class ForgotPasswordHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPasswordResetEmailService> _mockEmailService;
        private readonly ForgotPasswordHandler _handler;

        public ForgotPasswordHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailService = new Mock<IPasswordResetEmailService>();
            _handler = new ForgotPasswordHandler(_mockUnitOfWork.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_DoesNotSendEmail()
        {
            // Arrange
            var email = "nonexistent@test.dk";
            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByEmployeeEmailAsync(email))
                          .ReturnsAsync((Account?)null);

            var command = new ForgotPasswordCommand(email);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmailService.Verify(s => s.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_AccountExists_GeneratesTokenAndSendsEmail()
        {
            // Arrange
            var email = "test@firma.dk";
            var dummyAccount = new Account();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByEmployeeEmailAsync(email))
                          .ReturnsAsync(dummyAccount);

            var command = new ForgotPasswordCommand(email);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);

            _mockEmailService.Verify(s => s.SendPasswordResetEmailAsync(
                email,
                It.Is<string>(t => !string.IsNullOrEmpty(t))),
                Times.Once);
        }
    }
}