namespace FlightInvoiceProcessing.WorkerService.Models
{
    public class InvoiceProcessingResult
    {
        private int _totalProcessedRecords;
        private int _totalSuccessfulRecords;
        private List<InvalidInvoiceRecordDetail> _invalidRecordDetails
            = new List<InvalidInvoiceRecordDetail>();

        public int TotalProcessedRecords => _totalProcessedRecords;

        public int TotalSuccessfulRecords => _totalSuccessfulRecords;

        public int TotalInvalidRecords => _invalidRecordDetails.Count;

        public List<InvalidInvoiceRecordDetail> InvalidRecordDetails => _invalidRecordDetails;

        public string FlightInvoiceFileName { get; set; }

        public void IncrementTotalProcessedRecords()
        {
            _totalProcessedRecords++;
        }

        public void IncrementTotalSuccessfulRecords()
        {
            _totalSuccessfulRecords++;
        }

        public void AddInvalidUnmatchedRecord(FlightInvoiceLine flightInvoiceLine)
        {
            InvalidRecordDetails.Add(new InvalidInvoiceRecordDetail()
            {
                Record = flightInvoiceLine,
                Reason = Enums.InvalidInvoiceRecordReason.UnmatchedRecord,
                Message = "There is a record on the invoice but we couldn't find it on the database."
            });
        }

        public void AddInvalidDuplicateInvoiceRecord(FlightInvoiceLine flightInvoiceLine)
        {
            InvalidRecordDetails.Add(new InvalidInvoiceRecordDetail()
            {
                Record = flightInvoiceLine,
                Reason = Enums.InvalidInvoiceRecordReason.DuplicateInvoice,
                Message = "We already got a booking's invoice, Airline invoices us twice."
            });
        }

        public void AddInvalidDifferentPriceRecord(FlightInvoiceLine flightInvoiceLine)
        {
            InvalidRecordDetails.Add(new InvalidInvoiceRecordDetail()
            {
                Record = flightInvoiceLine,
                Reason = Enums.InvalidInvoiceRecordReason.DifferentPrice,
                Message = "Invoice and booking prices are different."
            });
        }

        public void AddInvalidDifferentNumberOfSoldSeatsRecord(FlightInvoiceLine flightInvoiceLine)
        {
            InvalidRecordDetails.Add(new InvalidInvoiceRecordDetail()
            {
                Record = flightInvoiceLine,
                Reason = Enums.InvalidInvoiceRecordReason.DifferentNumberOfSoldSeats,
                Message = "Invoice and booking sold seat count are different."
            });
        }
    }
}
