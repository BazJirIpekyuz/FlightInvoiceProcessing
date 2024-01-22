using FlightInvoiceProcessing.WorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace FlightInvoiceProcessing.WorkerService.DbContexts
{
    public class OdeonDbContext : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }
        public OdeonDbContext(DbContextOptions<OdeonDbContext> options)
            : base(options) { }
    }
}
