using Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence.Sql.TodoItem
{
	/// <summary>
	/// Provides implementation details for running commands against <see cref="Domain.Entities.TodoItem"/>s using SQL.
	/// </summary>
	public class TodoItemCommandDataAccess : ITodoItemCommandDataAccess
	{
		private readonly MyDbContext _dbContext;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="dbContext">Provides accessibility to the sql data store.</param>
		public TodoItemCommandDataAccess(MyDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// Create a new todo item.
		/// </summary>
		/// <param name="entity">The todo item to create.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		public async Task Create(Domain.Entities.TodoItem entity, CancellationToken cancellationToken)
		{
			await _dbContext.TodoItems.AddAsync(entity, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
