using System;

namespace Common.Configuration
{
	/// <summary>
	/// Provides additional functionality for working with <see cref="Environment"/>.Variables.
	/// </summary>
	public static class EnvironmentHelper
	{
		/// <summary>
		/// Returns the value from the environment variable or throws an exception if the environment variable does not exist.
		/// </summary>
		/// <param name="name">The environment variable key name.</param>
		/// <exception cref="ArgumentException"></exception>
		public static string GetRequiredEnvironmentVariable(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			return Environment.GetEnvironmentVariable(name) ?? throw new ArgumentException($"Environment variable '{name}' is required.");
		}

		/// <summary>
		/// Returns true if the ASPNETCORE_Environment variable is equal to "Development".
		/// </summary>
		public static bool IsDevelopment()
		{
			return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
		}
	}
}
