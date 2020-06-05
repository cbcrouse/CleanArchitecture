using AutoMapper;
using Moq;

namespace Common.Testing
{
	/// <summary>
	/// Provides extended functionality when working with a mocked <see cref="IMapper"/>.
	/// </summary>
	public static class MockIMapperExtensions
	{
		/// <summary>
		/// Sets up the 'Map' method.
		/// </summary>
		/// <typeparam name="TSource">The source type.</typeparam>
		/// <typeparam name="TDestination">The destination type.</typeparam>
		/// <param name="mapper">An mocked abstraction for mapping one object to another.</param>
		public static Mock<IMapper> SetupMap<TSource, TDestination>(this Mock<IMapper> mapper)
		{
			mapper.Setup(x => x.Map<TDestination>(It.IsAny<TSource>())).Returns(It.IsAny<TDestination>());
			return mapper;
		}

		/// <summary>
		/// Sets up the 'Map' method with return object.
		/// </summary>
		/// <typeparam name="TSource">The source type.</typeparam>
		/// <typeparam name="TDestination">The destination type.</typeparam>
		/// <param name="mapper">An mocked abstraction for mapping one object to another.</param>
		/// <param name="returnObject">The object to return.</param>
		public static Mock<IMapper> SetupMap<TSource, TDestination>(this Mock<IMapper> mapper, TDestination returnObject)
		{
			mapper.Setup(x => x.Map<TDestination>(It.IsAny<TSource>())).Returns(returnObject);
			return mapper;
		}

		/// <summary>
		/// Verify the 'Map' method was called once.
		/// </summary>
		/// <typeparam name="TSource">The source type.</typeparam>
		/// <typeparam name="TDestination">The destination type.</typeparam>
		/// <param name="mapper">An mocked abstraction for mapping one object to another.</param>
		public static void VerifyMapOnce<TSource, TDestination>(this Mock<IMapper> mapper)
		{
			mapper.Verify(x => x.Map<TDestination>(It.IsAny<TSource>()), Times.Once);
		}
	}
}
