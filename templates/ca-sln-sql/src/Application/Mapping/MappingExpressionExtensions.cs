using System;
using System.Linq.Expressions;
using Application.Interfaces.ValueResolvers;
using AutoMapper;

namespace Application.Mapping
{
	/// <summary>
	/// Helper extensions to make AutoMapper mapping easier.
	/// </summary>
	public static class MappingExpressionExtensions
	{
		/// <summary>
		/// Resolves a <see cref="DateTime"/> using <see cref="INowValueResolver{TSource, TDestination}"/>.
		/// </summary>
		public static IMappingExpression<TSource, TDestination> Now<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression,
			Expression<Func<TDestination, DateTimeOffset>> destinationMember)
		{
			return mappingExpression.ForMember(destinationMember, opt => opt.MapFrom<INowValueResolver<TSource, TDestination>>());
		}

		/// <summary>
		/// Resolves a <see cref="string"/> using <see cref="IStringKeyValueResolver{TSource, TDestination}"/>.
		/// </summary>
		public static IMappingExpression<TSource, TDestination> NewStringKey<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression,
			Expression<Func<TDestination, string>> destinationMember)
		{
			return mappingExpression.ForMember(destinationMember, opt => opt.MapFrom<IStringKeyValueResolver<TSource, TDestination>>());
		}

		/// <summary>
		/// Resolves a <see cref="bool"/> to true.
		/// </summary>
		public static IMappingExpression<TSource, TDestination> True<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression,
			Expression<Func<TDestination, bool>> destinationMember)
		{
			return mappingExpression.ForMember(destinationMember, opt => opt.MapFrom(_ => true));
		}

		/// <summary>
		/// Resolves a <see cref="bool"/> to false.
		/// </summary>
		public static IMappingExpression<TSource, TDestination> False<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression,
			Expression<Func<TDestination, bool>> destinationMember)
		{
			return mappingExpression.ForMember(destinationMember, opt => opt.MapFrom(_ => false));
		}
	}
}
