using System.ComponentModel.DataAnnotations;

namespace Persistence.Sql.Configuration
{
	/// <summary>
	/// Provides configuration settings for a data store.
	/// </summary>
	public class DataStoreOptions
	{
		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "'DataStoreOptions.ConnectionString' is required.")]
		public string ConnectionString { get; set; }
	}
}
