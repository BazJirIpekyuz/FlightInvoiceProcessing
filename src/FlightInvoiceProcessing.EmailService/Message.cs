using MimeKit;

namespace FlightInvoiceProcessing.EmailService
{
    public class Message
    {
        internal List<MailboxAddress> To { get; set; }
        internal string Subject { get; set; }
        internal string Content { get; set; }

        internal string? AttachmentPath { get; set; }

        public Message(IEnumerable<(string name, string emailAddress)> to, string subject, string content, string attachmentPath)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x.name, x.emailAddress)));
            Subject = subject;
            Content = content;
            AttachmentPath = attachmentPath;
        }
    }
}
