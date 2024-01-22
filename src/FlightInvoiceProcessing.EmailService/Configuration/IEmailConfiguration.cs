namespace FlightInvoiceProcessing.EmailService.Configuration
{
    public interface IEmailConfiguration
    {
        string Password { get; set; }
        int Port { get; set; }
        string SenderEmail { get; set; }
        string SenderName { get; set; }
        string Server { get; set; }
        string UserName { get; set; }
    }
}