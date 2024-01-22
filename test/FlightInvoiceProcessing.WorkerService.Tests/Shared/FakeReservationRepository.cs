using FlightInvoiceProcessing.WorkerService.Models;
using FlightInvoiceProcessing.WorkerService.Repositories;

namespace FlightInvoiceProcessing.WorkerService.Tests.Shared
{
    public class FakeReservationRepository : IReservationRepository
    {
        public int ExecutionCount;
        public IEnumerable<Reservation> _reservations;

        public FakeReservationRepository()
        {
            _reservations = Reservations;
        }
        public FakeReservationRepository(IEnumerable<Reservation> data)
        {
            _reservations = data;
        }


        public async Task<IEnumerable<Reservation>> GetReservations(string carrierCode, long flightNo, DateOnly flightDate)
        {
            var result = _reservations
                .Where(q => q.CarrierCode == carrierCode
                    && q.FlightNo == flightNo
                    && q.FlightDate == flightDate.ToString("M/d/yyyy"));

            return await Task.FromResult(result);
        }

        public Task<bool> UpdateInvoiceNumberForReservation(string carrierCode, long flightNo, DateOnly flightDate, string invoiceNumber)
        {
            var records = _reservations
                .Where(q => q.CarrierCode == carrierCode
                    && q.FlightNo == flightNo
                    && q.FlightDate == flightDate.ToString("M/d/yyyy")).ToList();

            records.ForEach(q => q.InvoiceNumber = invoiceNumber);

            return Task.FromResult(true);
        }

        private IEnumerable<Reservation> Reservations => new List<Reservation>()
        {
            new Reservation()
            {
                BookingId = 101012,
                Customer = "Customer Name 25",
                CarrierCode = "XQ",
                FlightNo=111,
                FlightDate="1/9/2024",
                Price=66
            },
            new Reservation()
            {
                BookingId = 101012,
                Customer = "Customer Name 26",
                CarrierCode = "XQ",
                FlightNo=111,
                FlightDate="1/9/2024",
                Price=66
            },
            new Reservation()
            {
                BookingId = 101017,
                Customer = "Customer Name 38",
                CarrierCode = "XQ",
                FlightNo=114,
                FlightDate="1/12/2024",
                Price=126,
                InvoiceNumber="10405" // Already invoiced
            },
            new Reservation()
            {
                BookingId = 101017,
                Customer = "Customer Name 39",
                CarrierCode = "XQ",
                FlightNo=114,
                FlightDate="1/12/2024",
                Price=126,
                InvoiceNumber="10405" // Already invoiced
            },
            new Reservation()
            {
                BookingId = 101014,
                Customer = "Customer Name 31",
                CarrierCode = "XQ",
                FlightNo=120,
                FlightDate="1/14/2024",
                Price=100, // Price different
            },
            new Reservation()
            {
                BookingId = 101014,
                Customer = "Customer Name 32",
                CarrierCode = "XQ",
                FlightNo=120,
                FlightDate="1/14/2024",
                Price=100, // Price different
            },
        };
    }
}
