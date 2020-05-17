using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Persistence.Sql.Tests
{
	public partial class TodoItemDataAccessTests
	{
		public string EntityKey => "Domain.Entities.TodoItem";

		[Fact]
		public async Task TodoItem_Id_ConfiguredCorrectly()
		{
			// Arrange
			await using var context = await GetSqliteContext();

			// Act & Assert
			var entity = context.Model.FindEntityType(EntityKey);
			Assert.NotNull(entity);

			var id = entity.FindProperty(nameof(Domain.Entities.TodoItem.Id));
			Assert.Null(id.GetValueGeneratorFactory());
			Assert.True(id.IsPrimaryKey());
			Assert.False(id.IsNullable);
		}

		[Fact]
		public async Task TodoItem_Description_ConfiguredCorrectly()
		{
			// Arrange
			await using var context = await GetSqliteContext();

			// Act & Assert
			var entity = context.Model.FindEntityType(EntityKey);
			Assert.NotNull(entity);

			var description = entity.FindProperty(nameof(Domain.Entities.TodoItem.Description));
			Assert.False(description.IsNullable);
		}

		[Fact]
		public async Task TodoItem_CreatedOn_ConfiguredCorrectly()
		{
			// Arrange
			await using var context = await GetSqliteContext();

			// Act & Assert
			var entity = context.Model.FindEntityType(EntityKey);
			Assert.NotNull(entity);

			var createdOn = entity.FindProperty(nameof(Domain.Entities.TodoItem.CreatedOn));
			Assert.False(createdOn.IsNullable);
		}

		[Fact]
		public async Task TodoItem_CompletedOn_ConfiguredCorrectly()
		{
			// Arrange
			await using var context = await GetSqliteContext();

			// Act & Assert
			var entity = context.Model.FindEntityType(EntityKey);
			Assert.NotNull(entity);

			var completedOn = entity.FindProperty(nameof(Domain.Entities.TodoItem.CompletedOn));
			Assert.True(completedOn.IsNullable);
		}

		[Fact]
		public async Task TodoItem_DueOn_ConfiguredCorrectly()
		{
			// Arrange
			await using var context = await GetSqliteContext();

			// Act & Assert
			var entity = context.Model.FindEntityType(EntityKey);
			Assert.NotNull(entity);

			var dueOn = entity.FindProperty(nameof(Domain.Entities.TodoItem.DueOn));
			Assert.True(dueOn.IsNullable);
		}
	}
}
