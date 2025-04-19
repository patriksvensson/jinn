namespace Jinn.Tests;

public sealed class ParserTests
{
    [Fact]
    public void Should_Return_Unmatched_Tokens()
    {
        // Given
        var command = new Command("foo");
        command.Arguments.Add(new Argument<string>("<FOO>"));
        command.Options.Add(new Option<bool>("--bar"));
        command.Options.Add(new Option<bool>("-a"));
        command.Options.Add(new Option<bool>("-b"));
        command.Options.Add(new Option<bool>("-c"));
        command.Options.Add(new Option<int>("--lol"));

        var command2 = new Command("bar");
        command2.Options.Add(new Option<bool>("--corgi"));
        command.Commands.Add(command2);

        var root = new RootCommand(command);

        // When
        var result = ParserFixture.Parse(root, "foo root --bar -abf --lol qux bar --corgi");

        // Then
        result.UnmatchedTokens.ShouldHaveTokens(
            "(Argument)f");
    }

    [Fact]
    public void Should_Return_Error_If_Option_Values_Exceed_Arity()
    {
        // Given
        var root = new RootCommand();
        root.Options.Add(new Option<int>("--corgi") { IsRequired = true });

        // When
        var result = ParserFixture.Parse(root, "--corgi foo --corgi bar");

        // Then
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Message.ShouldBe("Option expects a single argument");
        result.Errors[0].Span.ShouldNotBeNull();
        result.Errors[0].Span.Value.Position.ShouldBe(0);
        result.Errors[0].Span.Value.Length.ShouldBe(7);
    }
}