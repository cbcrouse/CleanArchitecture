using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.MediatR
{
    /// <summary>
    /// A <see cref="RequestValidationBehavior{TRequest, TResponse}"/>
    /// </summary>
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="validators">A collection of validators for a particular type.</param>
		public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators;
		}

		/// <summary>
		/// Handles the interception for request validation.
		/// </summary>
		/// <param name="request">The <see cref="IRequest"/> object.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		/// <param name="next">The next handler in the pipeline.</param>
		public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
            var context = new ValidationContext<TRequest>(request);

			List<ValidationFailure> failures = _validators
				.Select(v => v.Validate(context))
				.SelectMany(result => result.Errors)
				.Where(f => f != null)
				.ToList();

			if (failures.Count != 0)
			{
				throw new ValidationException(failures);
			}

			return next();
		}
	}
}
