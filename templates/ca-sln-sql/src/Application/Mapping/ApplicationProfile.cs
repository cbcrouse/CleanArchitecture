using Application.TodoItem.Commands;
using AutoMapper;

namespace Application.Mapping
{
	/// <summary>
	/// Holds the mapping definitions for the application layer.
	/// </summary>
	public class ApplicationProfile : Profile
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public ApplicationProfile()
		{
			CreateMap<CreateTodoItemCommand, Domain.Entities.TodoItem>()
				.NewStringKey(x => x.Id)
				.Now(x => x.CreatedOn);
		}
	}
}
