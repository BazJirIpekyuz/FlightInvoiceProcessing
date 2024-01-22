namespace FlightInvoiceProcessing.WorkerService.Configuration
{
    public class FlightInvoiceFileConfiguration : IFlightInvoiceFileConfiguration
    {
        public string FilePath { get; set; }
        public int InvoiceNumberIndex { get; set; }
        public int InvoiceLineStartIndex { get; set; }
        public string CultureName { get; set; }
    }
}
