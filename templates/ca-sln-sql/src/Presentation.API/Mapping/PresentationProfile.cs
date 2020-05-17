using Application.TodoItem.Commands;
using AutoMapper;
using Presentation.API.Models;

namespace Presentation.API.Mapping
{
	/// <summary>
	/// Maintains mapping definitions for the presentation layer.
	/// </summary>
	public class PresentationProfile : Profile
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public PresentationProfile()
		{
			CreateMap<CreateTodoItemRequest, CreateTodoItemCommand>();
		}
	}
}
