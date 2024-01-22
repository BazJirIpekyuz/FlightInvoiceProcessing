using FlightInvoiceProcessing.EmailService.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FlightInvoiceProcessing.EmailService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, Action<EmailConfiguration> setupAction)
        {
            services.TryAddScoped<IEmailSender, EmailSender>();

            services.AddOptions();
            services.Configure(setupAction);

            services.TryAddSingleton<IEmailConfiguration>(sp =>
                    sp.GetRequiredService<IOptions<EmailConfiguration>>().Value);

            return services;
        }
    }
}
