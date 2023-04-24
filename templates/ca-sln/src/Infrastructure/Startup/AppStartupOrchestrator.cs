using Application.Configuration;
using AutoMapper;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using StartupOrchestration.NET;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Infrastructure.Startup
{
    /// <summary>
    /// Facilitates dependency registrations for the application.
    /// </summary>
    public class AppStartupOrchestrator : ServiceRegistrationOrchestrator
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AppStartupOrchestrator()
        {
            ServiceRegistrationExpressions.Add((services, config) => services.AddOptions());
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(OptionsFactory<>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(OptionsMonitor<>)));

            // Configuration Options
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<ApplicationOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<InfrastructureOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<PersistenceOptions>(config));

            // AutoMapper
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterAutoMapper());
        }

        /// <inheritdoc/>
        protected override ILogger StartupLogger => new SerilogLoggerFactory(new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Verbose()
            .WriteTo.ApplicationInsights(new TraceTelemetryConverter(), LogEventLevel.Information)
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy:MM:dd hh:mm:ss.fff tt}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
            .CreateLogger()
        ).CreateLogger(nameof(AppStartupOrchestrator));
    }
}
