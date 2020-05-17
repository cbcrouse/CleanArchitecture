using System;
using AutoMapper;

namespace Application.Interfaces.ValueResolvers
{
	/// <summary>
	/// Provides a way to resolve a <see cref="DateTimeOffset"/> property during mapping.
	/// </summary>
	/// <typeparam name="TSource">The source object.</typeparam>
	/// <typeparam name="TDestination">The destination object.</typeparam>
	public interface INowValueResolver<in TSource, in TDestination> : IValueResolver<TSource, TDestination, DateTimeOffset>
	{
		// Intentionally blank
	}
}
