using System;
using MediatR;

namespace Application.TodoItem.Commands
{
	/// <summary>
	/// The <see cref="IRequest"/> object used to create a <see cref="Domain.Entities.TodoItem"/>.
	/// </summary>
	public class CreateTodoItemCommand : IRequest<string>
	{
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the point in time when the task is due.
		/// </summary>
		public DateTimeOffset? DueOn { get; set; }
	}
}
