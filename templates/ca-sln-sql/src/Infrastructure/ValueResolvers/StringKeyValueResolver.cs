using Application.Interfaces;
using Application.Interfaces.ValueResolvers;
using AutoMapper;

namespace Infrastructure.ValueResolvers
{
	/// <summary>
	/// Resolves a <see cref="string"/> using <see cref="IKeyGenerator"/>.
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TDestination"></typeparam>
	public class StringKeyValueResolver<TSource, TDestination> : IStringKeyValueResolver<TSource, TDestination>
	{
		private readonly IKeyGenerator _keyGenerator;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="keyGenerator">An abstraction for generating unique keys.</param>
		public StringKeyValueResolver(IKeyGenerator keyGenerator)
		{
			_keyGenerator = keyGenerator;
		}

		/// <summary>
		/// Implementors use source object to provide a destination object.
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="destination">Destination object, if exists</param>
		/// <param name="destMember">Destination member</param>
		/// <param name="context">The context of the mapping</param>
		/// <returns>Result, typically build from the source resolution result</returns>
		public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
		{
			return _keyGenerator.NewStringKey();
		}
	}
}
