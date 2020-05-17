using Microsoft.EntityFrameworkCore;
using Persistence.Sql.TodoItem;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.Sql.Tests
{
	/// <summary>
	/// Partial class for testing commands.
	/// </summary>
	public partial class TodoItemDataAccessTests
	{
		[Fact]
		public async Task Can_Create()
		{
			// Arrange
			await using var context = await GetInMemContext();
			var entity = new Domain.Entities.TodoItem
			{
				Id = "1",
				Description = "",
				CreatedOn = DateTimeOffset.Now,
				CompletedOn = DateTimeOffset.Now,
				DueOn = DateTimeOffset.Now
			};
			var sut = new TodoItemCommandDataAccess(context);

			// Act
			await sut.Create(entity, CancellationToken.None);
			await context.SaveChangesAsync();

			// Assert
			var total = await context.TodoItems.Select(x => x.Id).CountAsync();
			Assert.Equal(1, total);
			Assert.Equal(entity.Id, context.TodoItems.Single().Id);
		}
	}
}
