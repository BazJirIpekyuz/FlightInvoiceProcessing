using FlightInvoiceProcessing.WorkerService.Helpers;
using FlightInvoiceProcessing.WorkerService.Models;
using FlightInvoiceProcessing.WorkerService.Processing;
using FlightInvoiceProcessing.WorkerService.Services;

namespace FlightInvoiceProcessing.WorkerService.BackgroundServices
{
    public class FlightInvoiceProcessingService : BackgroundService
    {
        private readonly ILogger<FlightInvoiceProcessingService> _logger;
        private readonly IDataToFileWriter _dataToFileWriter;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public FlightInvoiceProcessingService(
            ILogger<FlightInvoiceProcessingService> logger,
            IDataToFileWriter dataToFileWriter,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _dataToFileWriter = dataToFileWriter;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _serviceProvider.CreateScope();

                    var flightInvoiceProcessor = scope.ServiceProvider.GetRequiredService<IFlightInvoiceProcessor>();
                    var invoiceProcessingResult = await flightInvoiceProcessor.Process();

                    // Write data to a CSV file.
                    var csvFilePath = _dataToFileWriter.WriteToCsv(invoiceProcessingResult.InvalidRecordDetails);

                    try
                    {
                        await SendInvoiceProcessingResultEmail(invoiceProcessingResult, csvFilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An exception occurred while sending the " +
                            "invoice processing result email.");
                    }

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }

                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unhandled exception was thrown.");
            }
        }

        private async Task SendInvoiceProcessingResultEmail(InvoiceProcessingResult invoiceProcessingResult, string csvFilePath)
        {
            var emailRecipients = _configuration
                .GetSection("FlightInvoiceProcessingResultEmailRecipients")
                .Get<List<EmailRecipient>>();

            string subject = invoiceProcessingResult.FlightInvoiceFileName + " invoice file processing result";
            string content = $"Total processed records: {invoiceProcessingResult.TotalProcessedRecords}{Environment.NewLine}" +
                         $"Total successful records: {invoiceProcessingResult.TotalSuccessfulRecords}" +
                         $"Total invalid records: {invoiceProcessingResult.TotalInvalidRecords}";

            using var scope = _serviceProvider.CreateScope();

            var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

            await mailService.Send(
                emailRecipients,
                subject,
                content,
                csvFilePath);
        }
    }
}
