namespace Jinn.Tests;

public sealed class ParserTests
{
    [Fact]
    public void Should_Parse_Empty_Root_Command()
    {
        // Given
        var fixture = new RootCommandFixture();

        // When
        var result = fixture.ParseAndSerialize("");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand />
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Root_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("VALUE"));

        // When
        var result = fixture.ParseAndSerialize("42");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Argument Name="VALUE">
                    <Token Lexeme="42" Kind="Argument" Span="0:2" />
                  </Argument>
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Sub_Command()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Commands.Add(new Command("foo"));

        // When
        var result = fixture.ParseAndSerialize("foo");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Command Name="foo" />
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Multiple_Sub_Commands()
    {
        // Given
        var command = new Command("foo");
        command.AddCommand(new Command("bar"));
        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndSerialize("foo bar");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Command Name="foo">
                    <Command Name="bar" />
                  </Command>
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Be_Able_To_Parse_Root_Command_With_Sub_Command_Present()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Commands.Add(new Command("foo"));

        // When
        var result = fixture.ParseAndSerialize("");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand />
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Root_Option()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<bool>("--flag"));
        fixture.Options.Add(new Option<int>("--value"));

        // When
        var result = fixture.ParseAndSerialize("--flag --value 42");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Option Name="--flag" />
                  <Option Name="--value">
                    <Token Lexeme="42" Kind="Argument" Span="15:2" />
                  </Option>
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Option_With_Arity_Of_One()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("--lol 1 2 3");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Option Name="--lol">
                    <Token Lexeme="1" Kind="Argument" Span="6:1" />
                  </Option>
                </RootCommand>
              </ParsedCommand>
              <Unmatched>
                <Token Lexeme="2" Kind="Argument" Span="8:1" />
                <Token Lexeme="3" Kind="Argument" Span="10:1" />
              </Unmatched>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Option_With_Arity_Greater_Than_One()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<List<int>>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("--lol 1 2 3");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Option Name="--lol">
                    <Token Lexeme="1" Kind="Argument" Span="6:1" />
                    <Token Lexeme="2" Kind="Argument" Span="8:1" />
                    <Token Lexeme="3" Kind="Argument" Span="10:1" />
                  </Option>
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Argument_With_Arity_Of_One()
    {
        // Given
        var command = new Command("foo");
        command.AddArgument(new Argument<int>("VALUE"));
        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndSerialize("foo 1 2 3");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Command Name="foo">
                    <Argument Name="VALUE">
                      <Token Lexeme="1" Kind="Argument" Span="4:1" />
                    </Argument>
                  </Command>
                </RootCommand>
              </ParsedCommand>
              <Unmatched>
                <Token Lexeme="2" Kind="Argument" Span="6:1" />
                <Token Lexeme="3" Kind="Argument" Span="8:1" />
              </Unmatched>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Argument_With_Arity_Greater_Than_One()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<int>("VALUE"));

        // When
        var result = fixture.ParseAndSerialize("1 2 3");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Argument Name="VALUE">
                    <Token Lexeme="1" Kind="Argument" Span="0:1" />
                  </Argument>
                </RootCommand>
              </ParsedCommand>
              <Unmatched>
                <Token Lexeme="2" Kind="Argument" Span="2:1" />
                <Token Lexeme="3" Kind="Argument" Span="4:1" />
              </Unmatched>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Bundled_Options()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("-a"));
        fixture.Options.Add(new Option<int>("-b"));
        fixture.Options.Add(new Option<bool>("-c"));

        // When
        var result = fixture.ParseAndSerialize("-ab 42");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Option Name="-a" />
                  <Option Name="-b">
                    <Token Lexeme="42" Kind="Argument" Span="4:2" />
                  </Option>
                </RootCommand>
              </ParsedCommand>
              <Unmatched />
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Parse_Treat_Unknown_Unbundled_Option_As_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<bool>("-a"));
        fixture.Options.Add(new Option<bool>("-b"));
        fixture.Options.Add(new Option<bool>("-c"));

        // When
        var result = fixture.ParseAndSerialize("-abf 42");

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <ParsedCommand>
                <RootCommand>
                  <Option Name="-a" />
                  <Option Name="-b" />
                </RootCommand>
              </ParsedCommand>
              <Unmatched>
                <Token Lexeme="f" Kind="Argument" Span="3:1" />
                <Token Lexeme="42" Kind="Argument" Span="5:2" />
              </Unmatched>
            </ParseResult>
            """);
    }
}