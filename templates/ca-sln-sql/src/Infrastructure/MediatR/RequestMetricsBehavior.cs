using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.MediatR
{
    /// <summary>
    /// An instance of <see cref="RequestMetricsBehavior{TRequest, TResponse}"/>.
    /// </summary>
    public class RequestMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
		private readonly Stopwatch _timer;
		private readonly ILogger<TRequest> _logger;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="loggerFactory">Represents a type used to configure the logging system and create instances of <see cref="ILogger"/> from the registered <see cref="ILoggerProvider"/>s.</param>
		public RequestMetricsBehavior(ILoggerFactory loggerFactory)
		{
			_timer = new Stopwatch();
			_logger = loggerFactory.CreateLogger<TRequest>();
		}

		/// <summary>
		/// Handles the interception for metrics.
		/// </summary>
		/// <param name="request">The <see cref="IRequest"/> object.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		/// <param name="next">The next handler in the pipeline.</param>
		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			_timer.Start();

			TResponse response = await next();

			string formattedRequest = JsonConvert.SerializeObject(request, Formatting.Indented,
				new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});

			if (_timer.ElapsedMilliseconds <= 500)
				return response;

			string name = typeof(TRequest).Name;

			_logger.LogWarning("Mediator Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds) {@Request}", name, _timer.ElapsedMilliseconds, formattedRequest);

			return response;
		}
    }
}
