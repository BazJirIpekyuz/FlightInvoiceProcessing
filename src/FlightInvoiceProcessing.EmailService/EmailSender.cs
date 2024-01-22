using FlightInvoiceProcessing.EmailService.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace FlightInvoiceProcessing.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IEmailConfiguration _emailConfig;

        public EmailSender(ILogger<EmailSender> logger, IEmailConfiguration emailConfig)
        {
            _logger = logger;
            _emailConfig = emailConfig;
        }

        public async Task SendEmail(Message message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = message.Content;

            if (string.IsNullOrEmpty(message.AttachmentPath))
            {
                bodyBuilder.Attachments.Add(message.AttachmentPath);
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig.Server, _emailConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Sending email failed.");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
