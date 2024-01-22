using FlightInvoiceProcessing.WorkerService.Models;

namespace FlightInvoiceProcessing.WorkerService.Parsers
{
    public interface IFlightInvoiceFileParser
    {
        string FlightInvoiceFileName { get; }
        string? InvoiceNumber { get; }

        IEnumerable<IEnumerable<FlightInvoiceLine>> Parse();
    }
}