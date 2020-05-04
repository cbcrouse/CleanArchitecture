using System.Collections.Generic;

namespace Presentation.API.Configuration
{
	/// <summary>
	/// Provides configuration settings for HealthChecksUI.
	/// <para>https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks</para>
	/// </summary>
	public class HealthChecksUIOptions
	{
		/// <summary>
		/// Gets or sets whether the health checks UI is enabled.
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the name for the endpoint.
		/// </summary>
		public string EndpointName { get; set; } = "API";

		/// <summary>
		/// Gets or sets the relative endpoint for health checks.
		/// </summary>
		public string RelativeEndpoint { get; set; } = "/health";

		/// <summary>
		/// Gets or sets the health checks evaluation frequency in seconds.
		/// </summary>
		public int EvaluationFrequencyInSeconds { get; set; } = 10;

		/// <summary>
		/// Gets or sets the health checks failure notification frequency in seconds.
		/// </summary>
		public int FailureNotificationsFrequencySeconds { get; set; } = 60;

		/// <summary>
		/// Gets or sets a collection of webhook notifications.
		/// <para>
		/// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/doc/webhooks.md
		/// </para>
		/// </summary>
		public IList<WebHookNotification> WebHookNotifications { get; set; } = new List<WebHookNotification>();
	}

	/// <summary>
	/// Provides configuration settings for webhook notifications.
	/// </summary>
	public class WebHookNotification
	{
		/// <summary>
		/// Gets or sets whether the webhook notification is enabled.
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the webhook URI.
		/// </summary>
		public string Uri { get; set; }

		/// <summary>
		/// Gets or sets the health checks failure notification payload.
		/// </summary>
		public string Payload { get; set; }

		/// <summary>
		/// Gets or sets the health checks restored notification payload.
		/// </summary>
		public string RestoredPayload { get; set; }
	}
}
