using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
