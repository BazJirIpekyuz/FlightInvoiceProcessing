using FlightInvoiceProcessing.WorkerService.DbContexts;
using FlightInvoiceProcessing.WorkerService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightInvoiceProcessing.WorkerService.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly OdeonDbContext _dbContext;

        public ReservationRepository(OdeonDbContext dbContext)
        {
            _dbContext = dbContext ??
                throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Reservation>> GetReservations(
            string carrierCode,
            long flightNo,
            DateOnly flightDate)
        {
            return await _dbContext.Reservations
                .Where(q => q.CarrierCode == carrierCode
                    && q.FlightNo == flightNo
                    && q.FlightDate == flightDate.ToString("M/d/yyyy"))
                .ToListAsync();
        }

        public async Task<bool> UpdateInvoiceNumberForReservation(
            string carrierCode,
            long flightNo,
            DateOnly flightDate,
            string invoiceNumber)
        {
            return await _dbContext.Reservations
                .Where(q => q.CarrierCode == carrierCode
                    && q.FlightNo == flightNo
                    && q.FlightDate == flightDate.ToString("M/d/yyyy"))
                .ExecuteUpdateAsync(setters => setters.SetProperty(r => r.InvoiceNumber, invoiceNumber)) > 1;
        }
    }
}
