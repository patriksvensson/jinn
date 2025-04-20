namespace Jinn.Tests;

public sealed class TextSpanTests
{
    [Fact]
    public void Should_Throw_If_Position_Is_Negative()
    {
        // Given, When
        var result = Record.Exception(() => new TextSpan(-1, 0));

        // Then
        result.ShouldBeOfType<ArgumentException>()
            .And().Message.ShouldBe("Position must be greater than or equal to zero (Parameter 'position')");
    }

    [Fact]
    public void Should_Throw_If_Length_Is_Negative()
    {
        // Given, When
        var result = Record.Exception(() => new TextSpan(0, -1));

        // Then
        result.ShouldBeOfType<ArgumentException>()
            .And().Message.ShouldBe("Length must be greater than or equal to zero (Parameter 'position')");
    }

    public sealed class TheContainsMethod
    {
        [Theory]
        [InlineData(5, 10, 5)]
        [InlineData(5, 10, 6)]
        [InlineData(5, 10, 7)]
        [InlineData(5, 10, 8)]
        [InlineData(5, 10, 9)]
        [InlineData(5, 10, 10)]
        [InlineData(5, 10, 11)]
        [InlineData(5, 10, 12)]
        [InlineData(5, 10, 13)]
        [InlineData(5, 10, 14)]
        [InlineData(5, 10, 15)]
        public void Should_Return_True_If_Offset_Exist_Within_Span(int position, int length, int offset)
        {
            // Given
            var span = new TextSpan(position, length);

            // When
            var result = span.Contains(offset);

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(5, 10, 4)]
        [InlineData(5, 10, 16)]
        public void Should_Return_False_If_Offset_Do_Not_Exist_Within_Span(int position, int length, int offset)
        {
            // Given
            var span = new TextSpan(position, length);

            // When
            var result = span.Contains(offset);

            // Then
            result.ShouldBeFalse();
        }
    }
}
