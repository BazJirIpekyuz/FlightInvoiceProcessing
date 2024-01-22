using FlightInvoiceProcessing.WorkerService.Models;

namespace FlightInvoiceProcessing.WorkerService.Services
{
    public interface IMailService
    {
        Task Send(
            List<EmailRecipient> recipients,
            string subject,
            string content,
            string attachmentPath);
    }
}
