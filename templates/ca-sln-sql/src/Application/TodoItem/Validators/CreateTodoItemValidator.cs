using Application.TodoItem.Commands;
using FluentValidation;

namespace Application.TodoItem.Validators
{
	/// <summary>
	/// Creates the business rules for the <see cref="CreateTodoItemCommand"/>.
	/// </summary>
	public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemCommand>
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public CreateTodoItemValidator()
		{
			RuleFor(x => x.Description).NotEmpty();
		}
	}
}
