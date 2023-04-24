using Application.Configuration;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.Configuration;
using Infrastructure.MediatR;
using Infrastructure.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Persistence.Sql.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using StartupOrchestration.NET;
using System;
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
            // Configuration Options
            ServiceRegistrationExpressions.Add((services, config) => services.AddOptions());
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(OptionsFactory<>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(OptionsMonitor<>)));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<ApplicationOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<InfrastructureOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<PersistenceOptions>(config));

            // Add MediatR and FluentValidation
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddMediatR(GetMediatRConfiguration()));
            ServiceRegistrationExpressions.Add((services, config) => services.AddValidatorsFromAssemblyContaining(typeof(ApplicationOptions), ServiceLifetime.Transient, null, false));

            // AutoMapper
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterAutoMapper());

            // Services
            ServiceRegistrationExpressions.Add((services, config) => services.AddSingleton<ISystemClock, SystemClock>());
            ServiceRegistrationExpressions.Add((services, config) => services.AddSingleton<IKeyGenerator, MongoKeyGenerator>());

            // Persistence
            ServiceRegistrationExpressions.Add((services, config) => services.AddSqlDataAccess());

            // Startup Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy:MM:dd hh:mm:ss.fff tt}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        private static Action<MediatRServiceConfiguration> GetMediatRConfiguration()
        {
            return configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(ApplicationOptions).Assembly);
            };
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
