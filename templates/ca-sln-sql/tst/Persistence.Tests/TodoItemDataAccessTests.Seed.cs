using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Persistence.Sql.Tests.TestClasses;

namespace Persistence.Sql.Tests
{
	/// <summary>
	/// Base partial class for seeding test data.
	/// </summary>
	public partial class TodoItemDataAccessTests : DbContextTestBase<MyDbContext>
	{
		public TodoItemDataAccessTests()
		{
			MockMapper = new Mock<IMapper>();
		}

		public Mock<IMapper> MockMapper { get; set; }

		/// <summary>
		/// Add data required for testing to the context.
		/// </summary>
		/// <param name="context">The context to seed.</param>
		protected override Task SeedData(MyDbContext context)
		{
			return Task.CompletedTask;
		}
	}
}
