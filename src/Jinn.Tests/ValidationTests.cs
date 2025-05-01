namespace Jinn.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void Should_Not_Produce_Diagnostic_If_Required_Argument_Is_Present()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("VALUE").Required());

        // When
        var result = fixture.Parse("42");

        // Then
        result.Diagnostics.Count.ShouldBe(0);
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Required_Argument_Is_Missing()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("VALUE").Required());

        // When
        var result = fixture.Parse("");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The required argument VALUE is missing");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Required_Argument_For_Sub_Command_Is_Missing()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("FOO").Required());

        var command = new Command("sub");
        command.AddArgument(new Argument<int>("BAR").Required());
        fixture.Commands.Add(command);

        // When
        var result = fixture.Parse("LOL sub");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The required argument BAR is missing");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Required_Argument_For_Root_Command_Is_Missing()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("FOO").Required());

        var command = new Command("sub");
        command.AddArgument(new Argument<int>("BAR").Required());

        fixture.Commands.Add(command);

        // When
        var result = fixture.Parse("sub LOL");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The required argument FOO is missing");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Argument_Has_Too_Few_Values_And_Arity_Is_Same_For_Min_And_Max()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<List<int>>("FOO").HasArity(2, 2));

        // When
        var result = fixture.Parse("42");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The argument FOO expected exactly 2 values, got 1");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Argument_Has_Too_Few_Values_And_Arity_Is_Not_Same_For_Min_And_Max()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<List<int>>("FOO").HasArity(2, 3));

        // When
        var result = fixture.Parse("42");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The argument FOO expected at least 2 values, got 1");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Option_Has_Too_Few_Values_And_Arity_Is_Same_For_Min_And_Max()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.AddOption(new Option<List<int>>("--value").HasArity(2, 2));

        // When
        var result = fixture.Parse("--value 42");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The option --value expected exactly 2 values, got 1");
    }

    [Fact]
    public void Should_Produce_Diagnostic_If_Option_Has_Too_Few_Values_And_Arity_Is_Not_Same_For_Min_And_Max()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.AddOption(new Option<List<int>>("--value").HasArity(2, 3));

        // When
        var result = fixture.Parse("--value 42");

        // Then
        result.Diagnostics.Count.ShouldBe(1);
        result.Diagnostics[0].Message.ShouldBe("The option --value expected at least 2 values, got 1");
    }
}