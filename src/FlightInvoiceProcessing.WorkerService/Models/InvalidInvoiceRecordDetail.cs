using FlightInvoiceProcessing.WorkerService.Enums;

namespace FlightInvoiceProcessing.WorkerService.Models
{
    public class InvalidInvoiceRecordDetail
    {
        public FlightInvoiceLine Record { get; set; }
        public InvalidInvoiceRecordReason Reason { get; set; }
        public string Message { get; set; }
    }
}
