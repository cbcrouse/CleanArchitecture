namespace Application.Interfaces
{
	/// <summary>
	/// Provides an abstraction for generating unique identifiers.
	/// </summary>
	public interface IKeyGenerator
	{
		/// <summary>
		/// Generate a new string unique identifier.
		/// </summary>
		string NewStringKey();
	}
}
