using AuthAi.Interfaces;
using AuthAi.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace AuthAi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration) => _configuration = configuration;

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailOptions = new AppMailOptions();
            _configuration.GetSection(AppMailOptions.AppMail).Bind(mailOptions);
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(mailOptions.From, mailOptions.Email));
            emailMessage.To.Add(new MailboxAddress(mailOptions.To, email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("plain") { Text = message };

            var secureSocketOption = mailOptions.Port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.None
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(mailOptions.SmtpAddress, mailOptions.Port, secureSocketOption);
            await client.AuthenticateAsync(mailOptions.Email, mailOptions.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
