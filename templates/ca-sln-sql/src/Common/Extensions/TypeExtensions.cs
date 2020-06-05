using System;

namespace Common.Extensions
{
	/// <summary>
	/// Provides extended functionality for <see cref="Type"/>.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Returns true if the type contains an empty constructor.
		/// </summary>
		public static bool HasEmptyConstructor(this Type type)
		{
			return type.GetConstructor(Type.EmptyTypes) != null;
		}
	}
}
