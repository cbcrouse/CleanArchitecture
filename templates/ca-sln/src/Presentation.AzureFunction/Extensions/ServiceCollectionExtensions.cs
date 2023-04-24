using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;

// This namespace has been intentionally changed for convenience.
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a Serilog logger to the <paramref name="serviceCollection"/> and configures it with the provided <paramref name="configuration"/> object.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the logger with.</param>
    /// <param name="configuration">The configuration object used to configure the logger.</param>
    public static void RegisterLogging(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        serviceCollection.AddSingleton<ILoggerFactory>(serviceProvider =>
        {
            var factory = new SerilogLoggerFactory(logger, true);

            foreach (ILoggerProvider provider in serviceProvider.GetServices<ILoggerProvider>())
                factory.AddProvider(provider);

            return factory;
        });
        serviceCollection.AddLogging(builder => builder.AddSerilog(logger, true));
    }
}