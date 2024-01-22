using FlightInvoiceProcessing.WorkerService.Models;

namespace FlightInvoiceProcessing.WorkerService.Processing
{
    public interface IFlightInvoiceProcessor
    {
        Task<InvoiceProcessingResult> Process();
    }
}