using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.TodoItem.Commands;
using Application.TodoItem.Handlers;
using AutoMapper;
using Moq;
using Xunit;

namespace Business.Tests.TodoItem
{
	public class HandlerTests
	{
		[Fact]
		public async Task CreateTodoItemHandler_CallsDependencies()
		{
			// Arrange
			var request = new CreateTodoItemCommand();
			var dataAccess = new Mock<ITodoItemCommandDataAccess>();
			var mapper = new Mock<IMapper>();
			mapper.Setup(x => x.Map<Domain.Entities.TodoItem>(request)).Returns(new Domain.Entities.TodoItem());
			var sut = new CreateTodoItemHandler(dataAccess.Object, mapper.Object);

			// Act
			var result = await sut.Handle(request, CancellationToken.None);

			// Assert
			dataAccess.Verify(x => x.Create(It.IsAny<Domain.Entities.TodoItem>(), CancellationToken.None), Times.Once);
			mapper.Verify(x => x.Map<Domain.Entities.TodoItem>(request), Times.Once);
		}
	}
}
