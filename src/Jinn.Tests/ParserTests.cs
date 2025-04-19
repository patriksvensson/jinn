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
    public void Test()
    {
        // Given
        var root = new RootCommand();
        root.Options.Add(new Option<int>("--corgi") { IsRequired = true });
        root.SetHandler(_ => Task.FromResult(1));

        // When
        var result = ParserFixture.Parse(root, "--corgi foo --corgi bar");

        // Then
    }
}