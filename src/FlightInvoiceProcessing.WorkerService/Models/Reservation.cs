using Microsoft.EntityFrameworkCore;

namespace FlightInvoiceProcessing.WorkerService.Models
{
    [Keyless]
    public class Reservation
    {
        public long BookingId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string CarrierCode { get; set; } = string.Empty;
        public long FlightNo { get; set; }

        // Since Sqlite doesn't support Date type, I define it as string.
        public string FlightDate { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? InvoiceNumber { get; set; }
    }
}
