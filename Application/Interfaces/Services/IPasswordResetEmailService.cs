namespace Application.Interfaces.Services
{
    public interface IPasswordResetEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}