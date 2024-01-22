using FlightInvoiceProcessing.EmailService;
using FlightInvoiceProcessing.WorkerService.Models;

namespace FlightInvoiceProcessing.WorkerService.Services
{
    public class MailService : IMailService
    {
        private readonly IEmailSender _emailSender;

        public MailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Send(
            List<EmailRecipient> recipients,
            string subject,
            string content,
            string attachmentPath)
        {
            Message message = new Message(
                to: recipients.Select(q => (q.Name, q.EmailAddress)).ToList(),
                subject,
                content,
                attachmentPath);

            await _emailSender.SendEmail(message);
        }
    }
}
