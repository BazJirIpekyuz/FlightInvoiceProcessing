using FlightInvoiceProcessing.WorkerService.Models;

namespace FlightInvoiceProcessing.WorkerService.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetReservations(string carrierCode, long flightNo, DateOnly flightDate);
        Task<bool> UpdateInvoiceNumberForReservation(string carrierCode, long flightNo, DateOnly flightDate, string invoiceNumber);
    }
}