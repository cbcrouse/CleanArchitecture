using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.API.HealthChecks
{
	/// <summary>
	/// A health check publisher to determine application readiness.
	/// </summary>
	public class ReadinessPublisher : IHealthCheckPublisher
	{
		private readonly ILogger _logger;
		private readonly ISystemClock _clock;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="logger">A generic interface for logging where the category name is derived from the specified.</param>
		/// <param name="clock">Abstracts the system clock to facilitate testing.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public ReadinessPublisher(ILogger<ReadinessPublisher> logger, ISystemClock clock)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			_logger = logger;
			_clock = clock;
		}

		/// <summary>
		/// A collection of <see cref="HealthReport"/> entries.
		/// </summary>
		public List<(HealthReport report, CancellationToken cancellationToken)> Entries { get; } = new List<(HealthReport report, CancellationToken cancellationToken)>();

		/// <summary>
		/// Thrown during <see cref="PublishAsync"/> if not null.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Publishes the <see cref="HealthReport"/> if no exceptions occurred.
		/// </summary>
		/// <param name="report">Represents the result of executing a group of <see cref="T:Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck" /> instances.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
		{
			Entries.Add((report, cancellationToken));

			_logger.LogInformation("{TIMESTAMP} Readiness Probe Status: {RESULT}", _clock.UtcNow, report.Status);

			if (Exception != null)
			{
				throw Exception;
			}

			cancellationToken.ThrowIfCancellationRequested();

			return Task.CompletedTask;
		}
	}
}
