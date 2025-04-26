namespace Jinn.Tests;

public sealed class ParserTests
{
    [Fact]
    public void Parses_Consecutive_Tokens_Into_List()
    {
        // Given
        var command = new Command("foo");
        command.Options.Add(new Option<List<int>>("--lol"));
        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndSerialize(
            "foo --lol 1 2 3",
            new ParseResultSerializerOptions
            {
                Parts = ParseResultParts.Parsed,
                ExcludeExecutable = true,
            });

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <Command Name="foo">
                  <Option Name="--lol">
                    <Token Lexeme="1" Kind="Argument" Span="10:1" />
                    <Token Lexeme="2" Kind="Argument" Span="12:1" />
                    <Token Lexeme="3" Kind="Argument" Span="14:1" />
                  </Option>
                </Command>
              </ParsedCommand>
            </ParseResult>
            """);
    }

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
            "(Argument)f");
    }
}