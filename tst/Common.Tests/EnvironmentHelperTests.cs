using Common.Configuration;
using System;
using Xunit;

namespace Common.Tests
{
    public class EnvironmentHelperTests
    {
        private readonly string _variableName;

        public EnvironmentHelperTests()
        {
            _variableName = "ASPNETCORE_ENVIRONMENT";
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void GetRequiredEnvironmentVariable_ThrowsArgumentNullException_OnMissingName(string name)
        {
            // Act
            string Act()
            {
                return EnvironmentHelper.GetRequiredEnvironmentVariable(name);
            }

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void GetRequiredEnvironmentVariable_ThrowsArgumentException_OnMissingValue()
        {
            // Arrange
            var name = Guid.NewGuid().ToString();
            var expectedExceptionMessage = $"Environment variable '{name}' is required.";

            // Act
            string Act()
            {
                return EnvironmentHelper.GetRequiredEnvironmentVariable(name);
            }

            // Assert
            var ex = Assert.Throws<ArgumentException>(Act);
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }

        [Fact]
        public void GetRequiredEnvironmentVariable_ReturnsEnvironmentVariable()
        {
            // Arrange
            var name = Guid.NewGuid().ToString();
            var value = "bar";
            Environment.SetEnvironmentVariable(name, value);

            // Act
            var result = EnvironmentHelper.GetRequiredEnvironmentVariable(name);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void IsDevelopment_ReturnsFalse_WhenValueSetToNull()
        {
            // Arrange
            Environment.SetEnvironmentVariable(_variableName, null);

            // Act
            var result = EnvironmentHelper.IsDevelopment();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsDevelopment_ReturnsTrue_WhenValueSetToDevelopment()
        {
            // Arrange
            Environment.SetEnvironmentVariable(_variableName, "Development");

            // Act
            var result = EnvironmentHelper.IsDevelopment();

            // Assert
            Assert.True(result);
        }
    }
}
