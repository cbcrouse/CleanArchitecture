using Application.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Sql.Configuration
{
	/// <summary>
	/// Provides configuration values for the persistence layer.
	/// </summary>
	public class PersistenceOptions : IValidatable
	{
		/// <summary>
		/// Gets or sets the connection info for the SQL data store.
		/// </summary>
		[Required(ErrorMessage = "'PersistenceOptions.SqlDataStoreOptions' is required.")]
		public DataStoreOptions SqlDataStore { get; set; }
	}
}
