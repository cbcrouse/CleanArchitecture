using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.MediatR
{
	/// <summary>
	/// A <see cref="RequestLogger{TRequest}"/>
	/// </summary>
	public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="loggerFactory">Represents a type used to configure the logging system and create instances of <see cref="ILogger"/> from the registered <see cref="ILoggerProvider"/>s.</param>
		public RequestLogger(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<TRequest>();
		}

		/// <summary>
		/// This method executes before calling the Handle method on your handler.
		/// </summary>
		/// <param name="request">The <see cref="IRequest"/> object.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		public Task Process(TRequest request, CancellationToken cancellationToken)
		{
			string name = typeof(TRequest).Name;

			string formattedRequest = JsonConvert.SerializeObject(request, Formatting.Indented,
				new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});

			_logger.LogDebug("Mediator Request: {Name} {@Request}", name, formattedRequest);

			return Task.CompletedTask;
		}
	}
}
