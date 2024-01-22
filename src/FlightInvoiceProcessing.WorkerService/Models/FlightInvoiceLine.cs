namespace FlightInvoiceProcessing.WorkerService.Models
{
    public class FlightInvoiceLine
    {
        public int Season { get; set; }
        public int Vt { get; set; }
        public DateOnly FlightDate { get; set; }
        public string CarrierCode { get; set; }
        public long FlightNumber { get; set; }
        public string Routing { get; set; }
        public int NumberOfSoldSeats  { get; set; }
        public decimal PriceForEachSeat { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
