using Application.TodoItem.Commands;
using Common.Testing;
using Presentation.API.Models;
using Xunit;

namespace Mapping.Tests
{
    public partial class MappingTests
    {
        [Theory]
        [AutoFixture]
        public void CreateTodoItemRequest_MapsTo_CreateTodoItemCommand(CreateTodoItemRequest request)
        {
            // Act
            var result = Sut.Map<CreateTodoItemCommand>(request);

            // Assert
            Assert.NotNull(result);

            // Business Validations
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.DueOn, result.DueOn);
        }
    }
}
