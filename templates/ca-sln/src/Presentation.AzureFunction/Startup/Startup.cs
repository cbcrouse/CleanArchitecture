using Infrastructure.Startup;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.AzureFunction.Startup;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Presentation.AzureFunction.Startup
{
    /// <summary>
    /// Default startup orchestrator for the azure function.
    /// </summary>
    public class Startup : FunctionStartupOrchestrator<AppStartupOrchestrator>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
        {
            ServiceRegistrationExpressions.Add(() => RegisterLogging());
        }

        /// <summary>
        /// Sets the default base path for configuration files.
        /// </summary>
        /// <param name="builder">Represents a type used to build application configuration.</param>
        protected override void SetBasePath(IConfigurationBuilder builder)
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = Directory.GetParent(Path.GetDirectoryName(assemblyLocation)).FullName;
            builder.SetBasePath(path);
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
