using Application.TodoItem.Validators;
using FluentValidation.Validators;
using FluentValidation.Validators.UnitTestExtension.Composer;
using FluentValidation.Validators.UnitTestExtension.Core;
using Xunit;

namespace Business.Tests.TodoItem
{
	public class ValidatorTests
	{
		[Fact]
		public void CreateTodoItemValidator_HasCorrectValidators()
		{
			// Arrange
			var sut = new CreateTodoItemValidator();

			// Act & Assert
			sut.ShouldHaveRules(x => x.Description,
				BaseVerifiersSetComposer.Build()
					.AddPropertyValidatorVerifier<NotEmptyValidator>()
					.Create());

			// Assert
		}
	}
}
