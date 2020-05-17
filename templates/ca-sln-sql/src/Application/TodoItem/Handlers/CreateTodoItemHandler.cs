using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.TodoItem.Commands;
using AutoMapper;
using MediatR;

namespace Application.TodoItem.Handlers
{
	/// <summary>
	/// The <see cref="IRequestHandler{T}"/> responsible for creating a <see cref="Domain.Entities.TodoItem"/>.
	/// </summary>
	public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, string>
	{
		private readonly ITodoItemCommandDataAccess _commands;
		private readonly IMapper _mapper;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="commands">An abstraction for running task commands.</param>
		/// <param name="mapper">An abstraction for mapping one object to another.</param>
		public CreateTodoItemHandler(ITodoItemCommandDataAccess commands, IMapper mapper)
		{
			_commands = commands;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the request to create a <see cref="Domain.Entities.TodoItem"/>.
		/// </summary>
		/// <param name="request">The <see cref="IRequest"/> object used to create a <see cref="Domain.Entities.TodoItem"/>.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		public async Task<string> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
		{
			var entity = _mapper.Map<Domain.Entities.TodoItem>(request);
			await _commands.Create(entity, cancellationToken);
			return entity.Id;
		}
	}
}
