using Application.Interfaces;
using MongoDB.Bson;

namespace Infrastructure.Services
{
	/// <summary>
	/// Provides an implementation for the <see cref="IKeyGenerator"/> using MongoDB.
	/// </summary>
	public class MongoKeyGenerator : IKeyGenerator
	{
		/// <summary>
		/// Generate a unique identifier using MongoDB's ObjectId generator.
		/// </summary>
		public string NewStringKey()
		{
			return ObjectId.GenerateNewId().ToString();
		}
	}
}
