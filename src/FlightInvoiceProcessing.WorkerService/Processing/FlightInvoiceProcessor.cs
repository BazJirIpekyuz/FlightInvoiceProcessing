using FlightInvoiceProcessing.WorkerService.Models;
using FlightInvoiceProcessing.WorkerService.Parsers;
using FlightInvoiceProcessing.WorkerService.Repositories;

namespace FlightInvoiceProcessing.WorkerService.Processing
{
    public class FlightInvoiceProcessor : IFlightInvoiceProcessor
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IFlightInvoiceFileParser _invoiceFileParser;

        public FlightInvoiceProcessor(
            IReservationRepository reservationRepository,
            IFlightInvoiceFileParser invoiceFileParser)
        {
            _reservationRepository = reservationRepository;
            _invoiceFileParser = invoiceFileParser;
        }

        public async Task<InvoiceProcessingResult> Process()
        {
            var invoiceProcessingResult = new InvoiceProcessingResult();

            foreach (var flightInvoiceLines in _invoiceFileParser.Parse())
            {
                await ProcessInvoiceLines(
                    _invoiceFileParser.InvoiceNumber,
                    flightInvoiceLines,
                    invoiceProcessingResult);
            }

            invoiceProcessingResult.FlightInvoiceFileName = _invoiceFileParser.FlightInvoiceFileName;

            return invoiceProcessingResult;
        }

        private async Task ProcessInvoiceLines(
            string? invoiceNumber,
            IEnumerable<FlightInvoiceLine> flightInvoiceLines,
            InvoiceProcessingResult invoiceProcessingResult)
        {
            foreach (var invoiceLine in flightInvoiceLines)
            {
                invoiceProcessingResult.IncrementTotalProcessedRecords();

                var reservationsMatched = await _reservationRepository
                    .GetReservations(
                    invoiceLine.CarrierCode,
                    invoiceLine.FlightNumber,
                    invoiceLine.FlightDate);

                // Invalid record since no matched reservation.
                if (!reservationsMatched.Any())
                {
                    invoiceProcessingResult.AddInvalidUnmatchedRecord(invoiceLine);
                    continue;
                }

                // Reservation matched by flight and price.
                reservationsMatched = reservationsMatched.Where(q => q.Price == invoiceLine.PriceForEachSeat);

                // Invalid record since different price.
                if (!reservationsMatched.Any())
                {
                    invoiceProcessingResult.AddInvalidDifferentPriceRecord(invoiceLine);
                    continue;
                }

                // I suppose that the "different number of sold seats" case also should be considered.
                // Invalid record since unmatched number of sold seats.
                if (reservationsMatched.Count() != invoiceLine.NumberOfSoldSeats)
                {
                    invoiceProcessingResult.AddInvalidDifferentNumberOfSoldSeatsRecord(invoiceLine);
                    continue;
                }

                // Records whose invoice number exists.
                var reservationsMatchedWithInvoiceNumber = reservationsMatched.Where(q => !string.IsNullOrEmpty(q.InvoiceNumber));

                // Invalid record since duplicate invoice.
                if (reservationsMatchedWithInvoiceNumber.Any())
                {
                    invoiceProcessingResult.AddInvalidDuplicateInvoiceRecord(invoiceLine);
                    continue;
                }

                await _reservationRepository.UpdateInvoiceNumberForReservation(
                    invoiceLine.CarrierCode,
                    invoiceLine.FlightNumber,
                    invoiceLine.FlightDate,
                    invoiceNumber);

                invoiceProcessingResult.IncrementTotalSuccessfulRecords();
            }
        }
    }
}
