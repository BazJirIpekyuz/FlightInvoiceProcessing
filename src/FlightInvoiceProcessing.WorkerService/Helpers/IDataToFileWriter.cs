
namespace FlightInvoiceProcessing.WorkerService.Helpers
{
    public interface IDataToFileWriter
    {
        string WriteToCsv<T>(List<T> data, bool hasHeaderRecord = true);
    }
}