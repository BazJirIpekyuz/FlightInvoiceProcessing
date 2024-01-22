using FlightInvoiceProcessing.WorkerService.Configuration;
using FlightInvoiceProcessing.WorkerService.Models;
using FlightInvoiceProcessing.WorkerService.Parsers;
using FlightInvoiceProcessing.WorkerService.Processing;
using FlightInvoiceProcessing.WorkerService.Tests.Shared;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlightInvoiceProcessing.WorkerService.Tests.Processing
{
    public class FlightInvoiceProcessorTests
    {
        private readonly FlightInvoicePdfFileParser _invoiceFileParser;
        public FlightInvoiceProcessorTests()
        {
            _invoiceFileParser = new FlightInvoicePdfFileParser(
                NullLogger<FlightInvoicePdfFileParser>.Instance,
                new FlightInvoiceFileConfiguration()
                {
                    FilePath = "Invoice_10407.PDF",
                    InvoiceLineStartIndex = 19,
                    InvoiceNumberIndex = 3,
                    CultureName = "de-DE"
                });
        }
        [Fact]
        public async Task Given_NotInvoicedReservationOnDbStore_When_Processed_Then_Return_ResultWithInvoicedReservation()
        {
            // Arrange
            var fakeReservationRepository = new FakeReservationRepository(new List<Reservation>()
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
                });

            var invoiceProcessor = new FlightInvoiceProcessor(
                fakeReservationRepository,
                _invoiceFileParser);

            // Act
            var invoiceProcessingResult = await invoiceProcessor.Process();

            // Assert
            Assert.True(invoiceProcessingResult.TotalSuccessfulRecords == 1);

            var revervationsAffected = await fakeReservationRepository.GetReservations("XQ", 111, new DateOnly(2024, 1, 9));
            Assert.DoesNotContain(revervationsAffected, q => q.InvoiceNumber == null);
        }

        [Fact]
        public async Task Given_AlreadyInvoicedReservationOnDbStore_When_Processed_Then_Return_ResultWithInvalidRecordOfDuplicateReason()
        {
            // Arrange
            var invoiceProcessor = new FlightInvoiceProcessor(
                new FakeReservationRepository(new List<Reservation>()
                {
                    new Reservation()
                    {
                        BookingId = 101012,
                        Customer = "Customer Name 25",
                        CarrierCode = "XQ",
                        FlightNo=111,
                        FlightDate="1/9/2024",
                        Price=66,
                        InvoiceNumber="10407"
                    },
                    new Reservation()
                    {
                        BookingId = 101012,
                        Customer = "Customer Name 26",
                        CarrierCode = "XQ",
                        FlightNo=111,
                        FlightDate="1/9/2024",
                        Price=66,
                        InvoiceNumber="10407"
                    },
                }),
                _invoiceFileParser);

            // Act
            var invoiceProcessingResult = await invoiceProcessor.Process();

            // Assert
            var invalidRecordDetail = invoiceProcessingResult
                .InvalidRecordDetails.Where(x => x.Record.CarrierCode == "XQ"
                && x.Record.FlightNumber == 111
                && x.Record.FlightDate.ToString("M/d/yyyy") == "1/9/2024");

            Assert.True(invoiceProcessingResult.TotalSuccessfulRecords == 0);
            Assert.Single(invalidRecordDetail);
            Assert.True(invalidRecordDetail.First().Reason == Enums.InvalidInvoiceRecordReason.DuplicateInvoice);
        }
    }
}
