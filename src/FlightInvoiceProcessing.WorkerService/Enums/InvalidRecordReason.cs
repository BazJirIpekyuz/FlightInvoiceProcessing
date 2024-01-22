namespace FlightInvoiceProcessing.WorkerService.Enums
{
    public enum InvalidInvoiceRecordReason
    {
        UnmatchedRecord,
        DuplicateInvoice,
        DifferentPrice,
        DifferentNumberOfSoldSeats
    }
}
