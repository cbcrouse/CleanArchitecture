using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.TodoItem.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.API.Models;

namespace Presentation.API.Controllers
{
	/// <summary>
	/// Provides API access to work with <see cref="Domain.Entities.TodoItem"/>s.
	/// </summary>
	[ApiController]
	[Route("todo")]
	public class TodoItemController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="mapper">An abstraction for mapping one object to another.</param>
		/// <param name="mediator">An abstraction for accessing the application layer.</param>
		public TodoItemController(IMapper mapper, IMediator mediator)
		{
			_mapper = mapper;
			_mediator = mediator;
		}

		/// <summary>
		/// Create a new todo item.
		/// </summary>
		/// <param name="request">The request to create a new todo item.</param>
		[AllowAnonymous]
		[HttpPost("")]
		public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemRequest request)
		{
			var command = _mapper.Map<CreateTodoItemCommand>(request);
			var result = await _mediator.Send(command, CancellationToken.None);
			var location = Path.Join($"{Request.Scheme}://{Request.Host}", Request.Path, result);
			var locationUri = new Uri(location);
			return Created(locationUri, result);
		}
	}
}
