using Infrastructure.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;

namespace Presentation.AzureFunction.Startup
{
    /// <summary>
    /// Default startup orchestrator for the azure function.
    /// </summary>
    public class Startup : PresentationStartupOrchestrator<AppStartupOrchestrator>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
        {
            ServiceRegistrationExpressions.Add(() => RegisterLogging());
        }

        private void RegisterLogging()
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithExceptionDetails()
                .CreateLogger();
            ServiceCollection.AddSingleton<ILoggerFactory>(serviceProvider =>
            {
                var factory = new SerilogLoggerFactory(logger, true);

                foreach (ILoggerProvider provider in serviceProvider.GetServices<ILoggerProvider>())
                    factory.AddProvider(provider);

                return factory;
            });
            ServiceCollection.AddLogging(builder => builder.AddSerilog(logger, true));
        }
    }
}
