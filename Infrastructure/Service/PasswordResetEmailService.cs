using Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class PasswordResetEmailService : IPasswordResetEmailService
    {
        private readonly IConfiguration _configuration;

        // Vi injecter IConfiguration for at kunne læse fra vores User Secrets
        public PasswordResetEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendPasswordResetEmailAsync(string toEmail, string token)
        {
            // Vi henter dine hemmeligheder lynhurtigt fra secrets.json
            var smtpEmail = _configuration["Smtp:Email"];
            var smtpPassword = _configuration["Smtp:Password"];

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("System Support", smtpEmail));
            message.To.Add(new MailboxAddress("Medarbejder", toEmail));
            message.Subject = "Nulstilning af dit kodeord";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family: sans-serif; padding: 20px; background-color: #f3f4f6; max-width: 500px; margin: auto; border: 2px solid #000000; border-radius: 12px; box-shadow: 4px 4px 0px #000000;'>
                        <h2 style='color: #000000; margin-top: 0;'>Ny Registrering</h2>
                        <p style='color: #374151; font-size: 16px;'>Du har anmodet om at nulstille dit kodeord. Her er din token som du skal sætte ind i Reset Password formlen:</p>
                        <div style='margin: 30px 0; text-align: center;'>
                            <a style='background-color: #8fa17d; color: #000000; font-weight: bold; text-decoration: none; padding: 12px 24px; border: 2px solid #000000; border-radius: 8px; box-shadow: 3px 3px 0px #000000; display: inline-block;'>{token}</a>
                        </div>
                        <p style='color: #6b7280; font-size: 12px;'>Hvis du ikke har anmodet om dette, kan du roligt ignorere denne mail.</p>
                    </div>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            // Vi bruger variablerne i stedet for hardcoded tekst! 100% tjekket!
            await client.AuthenticateAsync(smtpEmail, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}