using Common.Configuration;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using StartupOrchestration.NET;

namespace Presentation.Console
{
    /// <summary>
    /// Provides a startup orchestrator for the console application.
    /// </summary>
    public class Startup : StartupOrchestrator<AppStartupOrchestrator>
	{
        /// <inheritdoc />
        protected override void AddConfigurationProviders(IConfigurationBuilder builder)
        {
            builder.AddPrioritizedSettings();
        }
    }
}
