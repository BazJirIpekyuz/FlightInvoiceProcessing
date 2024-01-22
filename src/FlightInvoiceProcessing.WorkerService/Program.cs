using FlightInvoiceProcessing.EmailService;
using FlightInvoiceProcessing.EmailService.Configuration;
using FlightInvoiceProcessing.WorkerService.BackgroundServices;
using FlightInvoiceProcessing.WorkerService.Configuration;
using FlightInvoiceProcessing.WorkerService.DbContexts;
using FlightInvoiceProcessing.WorkerService.Helpers;
using FlightInvoiceProcessing.WorkerService.Parsers;
using FlightInvoiceProcessing.WorkerService.Processing;
using FlightInvoiceProcessing.WorkerService.Repositories;
using FlightInvoiceProcessing.WorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<OdeonDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:OdeonDBConnectionString"]);
});

builder.Services.Configure<FlightInvoiceFileConfiguration>(builder.Configuration.GetSection("FlightInvoiceFileSettings"));

builder.Services.TryAddSingleton<IFlightInvoiceFileConfiguration>(sp =>
    sp.GetRequiredService<IOptions<FlightInvoiceFileConfiguration>>().Value); // forwarding via implementation factory

builder.Services.AddHostedService<FlightInvoiceProcessingService>();

builder.Services.AddScoped<IFlightInvoiceProcessor, FlightInvoiceProcessor>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddSingleton<IFlightInvoiceFileParser, FlightInvoicePdfFileParser>();

builder.Services.AddSingleton<IDataToFileWriter, DataToFileWriter>();

builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddEmailService(options =>
        {
            var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailConfiguration>();
            options.Server = emailSettings.Server;
            options.Port = emailSettings.Port;
            options.SenderName = emailSettings.SenderName;
            options.SenderEmail = emailSettings.SenderEmail;
            options.UserName = emailSettings.UserName;
            options.Password = emailSettings.Password;
        });

var host = builder.Build();
host.Run();
