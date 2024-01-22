namespace FlightInvoiceProcessing.WorkerService.Configuration
{
    public interface IFlightInvoiceFileConfiguration
    {
        public string FilePath { get; set; }
        int InvoiceLineStartIndex { get; set; }
        int InvoiceNumberIndex { get; set; }
        public string CultureName { get; set; }
    }
}