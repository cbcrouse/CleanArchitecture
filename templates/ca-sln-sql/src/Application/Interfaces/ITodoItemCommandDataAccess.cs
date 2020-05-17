using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	/// <summary>
	/// Provides a dependency-free way to run commands against <see cref="Domain.Entities.TodoItem"/>s.
	/// </summary>
	public interface ITodoItemCommandDataAccess
	{
		/// <summary>
		/// Create a new todo item.
		/// </summary>
		/// <param name="entity">The todo item to create.</param>
		/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
		Task Create(Domain.Entities.TodoItem entity, CancellationToken cancellationToken);
	}
}
