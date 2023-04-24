using Common.Configuration;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StartupOrchestration.NET;

namespace Presentation.AzureFunction.Startup
{
    /// <inheritdoc />
    public class Startup : StartupOrchestrator<AppStartupOrchestrator>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
        {
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterLogging(config));
        }

        /// <inheritdoc />
        protected override void AddConfigurationProviders(IConfigurationBuilder builder)
        {
            builder.AddPrioritizedSettings();
        }
    }
}
