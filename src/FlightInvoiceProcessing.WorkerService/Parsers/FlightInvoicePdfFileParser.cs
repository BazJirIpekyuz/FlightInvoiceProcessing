using FlightInvoiceProcessing.WorkerService.Models;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig;
using FlightInvoiceProcessing.WorkerService.Configuration;
using System.Globalization;

namespace FlightInvoiceProcessing.WorkerService.Parsers
{
    public class FlightInvoicePdfFileParser : IFlightInvoiceFileParser
    {
        private readonly ILogger<IFlightInvoiceFileParser> _logger;
        private readonly IFlightInvoiceFileConfiguration _invoiceFileConfiguration;
        private string? _invoiceNumber;
        private string _fileName = string.Empty;
        private CultureInfo _cultureInfo;

        public FlightInvoicePdfFileParser(
            ILogger<IFlightInvoiceFileParser> logger,
            IFlightInvoiceFileConfiguration invoiceFileConfiguration)
        {
            _logger = logger;
            _invoiceFileConfiguration = invoiceFileConfiguration;
            _cultureInfo = new CultureInfo(invoiceFileConfiguration.CultureName);
        }

        public string? InvoiceNumber => _invoiceNumber;

        public string FlightInvoiceFileName => _fileName;

        public IEnumerable<IEnumerable<FlightInvoiceLine>> Parse()
        {
            bool isInvoiceNumberCaptured = false;

            string filePath = _invoiceFileConfiguration.FilePath;
            _fileName = Path.GetFileName(filePath);

            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    // Either extract based on order in the underlying document with newlines and spaces.
                    var invoiceContents = ContentOrderTextExtractor.GetText(page);
                    var invoiceContentArr = invoiceContents.Split(new string[] { Environment.NewLine },
                        StringSplitOptions.None);

                    if (!isInvoiceNumberCaptured)
                    {
                        _invoiceNumber = invoiceContentArr[2].Trim();
                    }

                    yield return GetInvoiceLinesInPage(invoiceContentArr);
                }
            }
        }

        private List<FlightInvoiceLine> GetInvoiceLinesInPage(string[] invoiceContentArr)
        {
            var lines = new List<FlightInvoiceLine>();
            int InvoiceLineStartIndex = _invoiceFileConfiguration.InvoiceLineStartIndex;

            foreach (var invoiceLineContent in invoiceContentArr.Skip(InvoiceLineStartIndex - 1))
            {
                (bool isValidInvoiceLine, bool ignoreInvoiceLine) = TryParseInvoiceLine(invoiceLineContent, out FlightInvoiceLine? invoiceLine);

                // If it is not valid, that means we have already parsed all invoice lines.
                if (!isValidInvoiceLine)
                {
                    return lines;
                }

                if (!ignoreInvoiceLine)
                {
                    lines.Add(invoiceLine);
                }
            }

            return lines;
        }

        private (bool isValidInvoiceLine, bool ignoreInvoiceLine) TryParseInvoiceLine(string invoiceLineContent, out FlightInvoiceLine? invoiceLine)
        {
            invoiceLine = null;

            try
            {
                string[] invoiceLineContentArr = invoiceLineContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // I suppose that all columns should have value except "Summen in EUR" column in the invoice file. 
                // If all has value, a valid invoice line content arr will have 11 elements; otherwise, 10.
                // If the length is not 10 or 11 then it is not an invoice line.
                if (invoiceLineContentArr.Length < 10 || invoiceLineContentArr.Length > 11)
                {
                    return (false, false);
                }

                // Ignore record which has minus after the number of sold seat.
                if (invoiceLineContentArr[7].EndsWith('-'))
                {
                    return (true, true);
                }

                invoiceLine = new FlightInvoiceLine()
                {
                    Season = int.Parse(invoiceLineContentArr[0]),
                    Vt = int.Parse(invoiceLineContentArr[1]),
                    FlightDate = DateOnly.ParseExact(invoiceLineContentArr[2], "dd.MM.yyyy"),
                    CarrierCode = invoiceLineContentArr[3],
                    FlightNumber = long.Parse(invoiceLineContentArr[4]),
                    Routing = invoiceLineContentArr[5] + " " + invoiceLineContentArr[6],
                    NumberOfSoldSeats = int.Parse(invoiceLineContentArr[7]),
                    PriceForEachSeat = decimal.Parse(invoiceLineContentArr[8], _cultureInfo),
                    TotalPrice = decimal.Parse(invoiceLineContentArr[9], _cultureInfo),
                };

                return (true, false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not parse invoice line in the file.");
                return (false, false);
            }
        }
    }
}
