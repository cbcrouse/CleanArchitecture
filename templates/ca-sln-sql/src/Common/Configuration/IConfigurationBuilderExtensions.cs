using Microsoft.Extensions.Configuration;

namespace Common.Configuration
{
	/// <summary>
	/// Provides extended functionality for <see cref="IConfigurationBuilder"/>.
	/// </summary>
	public static class ConfigurationBuilderExtensions
	{
		/// <summary>
		/// Add several json file providers environment variables to the configuration builder in a specific order.
		/// </summary>
		/// <param name="builder">Represents a type used to build application configuration.</param>
		public static IConfigurationBuilder AddPrioritizedSettings(this IConfigurationBuilder builder)
		{
			string environment = EnvironmentHelper.GetRequiredEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			// Load application settings first
			builder.AddJsonFile("appsettings.core.json", optional: false, reloadOnChange: true);
			if (!string.IsNullOrWhiteSpace(environment))
				builder.AddJsonFile($"appsettings.core.{environment}.json", optional: true, reloadOnChange: true);

			// Load presentation settings next
			builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			if (!string.IsNullOrWhiteSpace(environment))
				builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

			// Load environment variables next to last
			builder.AddEnvironmentVariables();

			// Load local developer settings last
			builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

			return builder;
		}
	}
}
