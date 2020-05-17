using AutoMapper;

namespace Application.Interfaces.ValueResolvers
{
	/// <summary>
	/// Provides a way to resolve a <see cref="string"/> ID or Key property during mapping.
	/// </summary>
	/// <typeparam name="TSource">The source object.</typeparam>
	/// <typeparam name="TDestination">The destination object.</typeparam>
	public interface IStringKeyValueResolver<in TSource, in TDestination> : IValueResolver<TSource, TDestination, string>
	{
		// Intentionally blank
	}
}
