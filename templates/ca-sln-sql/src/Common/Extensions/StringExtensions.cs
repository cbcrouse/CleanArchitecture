using System.Diagnostics;
using System.Linq;

namespace Common.Extensions
{
	/// <summary>
	/// Provides extensions for System.String.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Returns true if all characters in the string are digits.
		/// </summary>
		[DebuggerStepThrough]
		public static bool IsDigitsOnly(this string value)
		{
			return value.IsPresent() && value.All(char.IsDigit);
		}

		/// <summary>
		/// Returns true if the string is not null, empty, or whitespace.
		/// </summary>
		[DebuggerStepThrough]
		public static bool IsPresent(this string value)
		{
			return !string.IsNullOrWhiteSpace(value);
		}
	}
}
