using System.ComponentModel.DataAnnotations;

namespace Application.Configuration
{
	/// <summary>
	/// Provides configuration values for the application layer.
	/// </summary>
	public class ApplicationOptions : IValidatable
	{
		public string TestValue { get; set; }
	}
}
