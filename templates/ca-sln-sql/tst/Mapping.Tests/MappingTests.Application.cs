using System;
using Application.TodoItem.Commands;
using Xunit;

namespace Mapping.Tests
{
	public partial class MappingTests
	{
		[Fact]
		public void CreateTodoItemCommand_MapsTo_DomainTodoItem()
		{
			// Arrange
			var command = new CreateTodoItemCommand();

			// Act
			var result = Sut.Map<Domain.Entities.TodoItem>(command);

			// Assert
			Assert.NotNull(result);

			// Business Validations
			Assert.NotNull(result.Id);
			Assert.IsType<string>(result.Id);
			Assert.NotEqual(DateTimeOffset.MinValue, result.CreatedOn);
		}
	}
}
