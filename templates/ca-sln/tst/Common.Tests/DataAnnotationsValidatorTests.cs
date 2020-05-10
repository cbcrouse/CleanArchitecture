using Common.DataAnnotations;
using Common.Tests.TestClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Common.Tests
{
	public class DataAnnotationsValidatorTests
	{
		private readonly DataAnnotationsValidator _validator;

		public DataAnnotationsValidatorTests()
		{
			SaveValidationContextAttribute.SavedContexts.Clear();
			_validator = new DataAnnotationsValidator();
		}

		[Fact]
		public void TryValidateObject_HasNoErrors_WhenValidParent()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObject(parent, validationResults);

			// Assert
			Assert.True(result);
			Assert.Empty(validationResults);
		}

		[Fact]
		public void TryValidateObject_HasErrors_WhenMissingRequiredProperties()
		{
			// Arrange
			var parent = new Parent { PropertyA = null, PropertyB = null };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObject(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Equal(2, validationResults.Count);
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "Parent PropertyA is required"));
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "Parent PropertyB is required"));
		}

		[Fact]
		public void TryValidateObject_CallsIValidatableObjectMethod()
		{
			// Arrange
			var parent = new Parent { PropertyA = 5, PropertyB = 6 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObject(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Single(validationResults);
			Assert.Equal("Parent PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
		}

		[Fact]
		public void TryValidateObjectRecursive_CallsIValidatableObjectMethod_OnChild()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child { Parent = parent, PropertyA = 5, PropertyB = 6 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Single(validationResults);
			Assert.Equal("Child PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
		}

		[Fact]
		public void TryValidateObject_CallsIValidatableObjectMethod_OnGrandChild()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child
			{
				Parent = parent,
				PropertyA = 1,
				PropertyB = 1,
				GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } }
			};
			var validationResults = new List<ValidationResult>();

			// Act
			bool result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Single(validationResults);
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
		}

		[Fact]
		public void TryValidateObjectRecursive_HasErrors_WhenChildHasInvalidProperties()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Single(validationResults);
			Assert.Equal("Child PropertyA is required", validationResults[0].ErrorMessage);
		}

		[Fact]
		public void TryValidateObjectRecursive_HasErrors_WhenGrandChildHasInvalidProperties()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child
			{
				Parent = parent,
				PropertyA = 1,
				PropertyB = 1,
				GrandChildren = new[] { new GrandChild { PropertyA = 11, PropertyB = 11 } }
			};
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Equal(2, validationResults.Count);
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "GrandChild PropertyA not within range"));
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "GrandChild PropertyB not within range"));
		}

		[Fact]
		public void TryValidateObjectRecursive_ErrorsIgnored_WhenChildHasSkipAttribute()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child { Parent = parent, PropertyA = 1, PropertyB = 1 };
			parent.SkippedChild = new Child { PropertyA = null, PropertyB = 1 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void TryValidateObjectRecursive_PassesValidationContextItems_ToAllValidationCalls()
		{
			// Arrange
			var parent = new Parent { Child = new Child { GrandChildren = new[] { new GrandChild() } } };
			var validationResults = new List<ValidationResult>();
			var contextItems = new Dictionary<object, object> { { "key", 12345 } };

			// Act
			_ = _validator.TryValidateObjectRecursive(parent, validationResults, contextItems);

			// Assert
			Assert.Equal(3, SaveValidationContextAttribute.SavedContexts.Count);
			Assert.True(SaveValidationContextAttribute.SavedContexts.Select(c => c.Items).All(items => items["key"] == contextItems["key"]));
		}

		[Fact]
		public void TryValidateObject_HasErrors_FromAllObjects()
		{
			// Arrange
			var parent = new Parent { PropertyA = 5, PropertyB = 6 };
			parent.Child = new Child
			{
				Parent = parent,
				PropertyA = 5,
				PropertyB = 6,
				GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } }
			};
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Equal(3, validationResults.Count);
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "Parent PropertyA and PropertyB cannot add up to more than 10"));
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "Child PropertyA and PropertyB cannot add up to more than 10"));
			Assert.Equal(1, validationResults.Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
		}

		[Fact]
		public void TryValidateObject_ModifiesMemberNames_ForNestedProperties()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(parent, validationResults);

			// Assert
			Assert.False(result);
			Assert.Single(validationResults);
			Assert.Equal("Child PropertyA is required", validationResults[0].ErrorMessage);
			Assert.Equal("Child.PropertyA", validationResults[0].MemberNames.First());
		}

		[Fact]
		public void TryValidateObject_NoErrors_WithObjectDictionary()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			var classWithDictionary = new ClassWithDictionary
			{
				Objects = new List<Dictionary<string, Child>>
				{
					new Dictionary<string, Child>
					{
						{ "key",
							new Child
							{
								Parent = parent,
								PropertyA = 1,
								PropertyB = 2
							}
						}
					}
				}
			};
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(classWithDictionary, validationResults);

			// Assert
			Assert.True(result);
			Assert.Empty(validationResults);
		}

		[Fact]
		public void TryValidateObject_NoErrors_WithNullEnumerationValues()
		{
			// Arrange
			var parent = new Parent { PropertyA = 1, PropertyB = 1 };
			var classWithNullableEnumeration = new ClassWithNullableEnumeration
			{
				Objects = new List<Child>
				{
					null,
					new Child
					{
						Parent = parent,
						PropertyA = 1,
						PropertyB = 2
					}
				}
			};
			var validationResults = new List<ValidationResult>();

			// Act
			var result = _validator.TryValidateObjectRecursive(classWithNullableEnumeration, validationResults);

			// Assert
			Assert.True(result);
			Assert.Empty(validationResults);
		}
	}
}
