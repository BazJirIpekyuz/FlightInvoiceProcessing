using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInvoiceProcessing.WorkerService.Configuration
{
    public class FlightInvoiceFileConfiguration : IFlightInvoiceFileConfiguration
    {
        public string FilePath { get; set; }
        public int InvoiceNumberIndex { get; set; }
        public int InvoiceLineStartIndex { get; set; }
        public string CultureName { get; set; }
    }
}
