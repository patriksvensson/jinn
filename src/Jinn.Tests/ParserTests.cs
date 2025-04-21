namespace Jinn.Tests;

public sealed class ParserTests
{
    [Fact]
    public void Should_Preserve_Unmatched_Tokens()
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

        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.Parse("foo root --bar -abf --lol qux bar --corgi");

        // Then
        result.Unmatched.ShouldHaveTokens(
            "(Argument)<ExecutableName> (Argument)f");
    }
}