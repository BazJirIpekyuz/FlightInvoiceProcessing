using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace FlightInvoiceProcessing.WorkerService.Helpers
{
    public class DataToFileWriter : IDataToFileWriter
    {
        private readonly string _outputDirectory;

        public DataToFileWriter(IConfiguration configuration)
        {
            _outputDirectory = configuration["FlightInvoiceProcessingResultDirectory"];
        }

        public string WriteToCsv<T>(List<T> data, bool hasHeaderRecord = true)
        {
            string fileName = Guid.NewGuid().ToString() + ".csv";

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord
            };

            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);

            string fileNameWithPath = Path.Combine(_outputDirectory, fileName);

            using (var writer = new StreamWriter(fileNameWithPath))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(data);
            }

            return fileNameWithPath;
        }
    }
}
