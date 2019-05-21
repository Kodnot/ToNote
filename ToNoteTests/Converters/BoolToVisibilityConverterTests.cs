using Shouldly;
using System;
using ToNote.Converters;
using Xunit;
using System.Windows;

namespace ToNoteTests.Converters
{
    public class BoolToVisibilityConverterTests
    {
        [Fact]
        public void Convert_WhenValueArgumentIsNotBool_ShouldThrow()
        {
            // Arrange
            var converter = new BoolToVisibilityConverter();

            // Act, Assert
            Should.Throw<ArgumentException>(() => converter.Convert("random" as object, null, null, null))
                 .Message.ShouldBe("Wrong data type. Expected a boolean");
        }

        [Theory]
        [InlineData(true, Visibility.Visible)]
        [InlineData(false, Visibility.Collapsed)]
        public void Convert_WhenValueArgumentIsBool_ShouldReturnCorrespondingResizeMode(bool value, Visibility mode)
        {
            // Arrange
            var converter = new BoolToVisibilityConverter();

            // Act
            var result = converter.Convert(value, null, null, null);

            // Assert
            ((Visibility)result).ShouldBe(mode);
        }

        [Fact]
        public void ConvertBack_WhenCalled_ShouldThrow()
        {
            // Arrange
            var converter = new BoolToVisibilityConverter();

            //Act, Assert
            Should.Throw<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
    }
}
