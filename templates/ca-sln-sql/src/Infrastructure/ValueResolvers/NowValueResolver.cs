using System;
using Application.Interfaces.ValueResolvers;
using AutoMapper;
using Microsoft.Extensions.Internal;

namespace Infrastructure.ValueResolvers
{
	/// <summary>
	/// Resolves a <see cref="DateTime"/> using <see cref="SystemClock"/>.
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TDestination"></typeparam>
	public class NowValueResolver<TSource, TDestination> : INowValueResolver<TSource, TDestination>
	{
		private readonly ISystemClock _systemClock;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="systemClock"></param>
		public NowValueResolver(ISystemClock systemClock)
		{
			_systemClock = systemClock;
		}

		/// <summary>
		/// Implementors use source object to provide a destination object.
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="destination">Destination object, if exists</param>
		/// <param name="destMember">Destination member</param>
		/// <param name="context">The context of the mapping</param>
		/// <returns>Result, typically build from the source resolution result</returns>
		public DateTimeOffset Resolve(TSource source, TDestination destination, DateTimeOffset destMember, ResolutionContext context)
		{
			return _systemClock.UtcNow;
		}
	}
}
