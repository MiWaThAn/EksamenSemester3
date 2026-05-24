using Application.Commands.Account;
using Application.Commands.Account.Handlers;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entity.Person;
using Domain.Interfaces.Item;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests
{
    public class ResetPasswordHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IHashingService> _mockHashingService;
        private readonly ResetPasswordHandler _handler;

        public ResetPasswordHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHashingService = new Mock<IHashingService>();
            _handler = new ResetPasswordHandler(_mockUnitOfWork.Object, _mockHashingService.Object);
        }

        [Fact]
        public async Task Handle_AccountNotFound_ReturnsFalse()
        {
            // Arrange
            var email = "unknown@test.dk";
            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByEmployeeEmailAsync(email))
                          .ReturnsAsync((Account?)null);

            var command = new ResetPasswordCommand(email, "some-token", "new-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var email = "user@test.dk";
            var account = new Account();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByEmployeeEmailAsync(email))
                          .ReturnsAsync(account);

            _mockHashingService.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-pass");

            var command = new ResetPasswordCommand(email, "wrong-token", "new-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesPasswordAndReturnsTrue()
        {
            // Arrange
            var email = "user@test.dk";
            var newPassword = "new-secure-password";
            var hashed = "hashed-password";

            var account = new Account();
            var token = account.GeneratePasswordResetToken();

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetByEmployeeEmailAsync(email))
                          .ReturnsAsync(account);
            _mockHashingService.Setup(h => h.Hash(newPassword)).Returns(hashed);

            var command = new ResetPasswordCommand(email, token, newPassword);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
    }
}