namespace Jinn.Tests;

public class TokenizerTests
{
    [Fact]
    public void Should_Tokenize_Single_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<string>("<FOO>"));

        // When
        var result = fixture.ParseAndSerialize("foo", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="foo" Kind="Argument" Span="0:3" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Tokenize_Multiple_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<List<string>>("<FOO>"));

        // When
        var result = fixture.ParseAndSerialize("foo", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="foo" Kind="Argument" Span="0:3" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(0, 255)]
    [InlineData(1, 255)]
    public void Should_Tokenize_Multiple_Arguments_Regardless_Of_Arity(int min, int max)
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<string>("<FOO>") { Arity = new Arity(min, max) });

        // When
        var result = fixture.ParseAndSerialize("foo bar", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="foo" Kind="Argument" Span="0:3" />
                <Token Lexeme="bar" Kind="Argument" Span="4:3" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Tokenize_Option_Without_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("--lol", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--lol" Kind="Option" Span="0:5" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Tokenize_Option_With_Single_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("--lol 42", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--lol" Kind="Option" Span="0:5" />
                <Token Lexeme="42" Kind="Argument" Span="6:2" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Tokenize_Option_With_Multiple_Arguments()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("--lol 42 32", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--lol" Kind="Option" Span="0:5" />
                <Token Lexeme="42" Kind="Argument" Span="6:2" />
                <Token Lexeme="32" Kind="Argument" Span="9:2" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Theory]
    [InlineData("--lol=32")]
    [InlineData("--lol:32")]
    public void Should_Split_Argument_Values(string args)
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize(args, parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--lol" Kind="Option" Span="0:5" />
                <Token Lexeme="32" Kind="Argument" Span="0:2" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Unbundle_Options()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("-a"));
        fixture.Options.Add(new Option<int>("-b"));
        fixture.Options.Add(new Option<int>("-c"));

        // When
        var result = fixture.ParseAndSerialize("-ac", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="-a" Kind="Option" Span="1:1" />
                <Token Lexeme="-c" Kind="Option" Span="2:1" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Acknowledge_Double_Dash()
    {
        // Given
        var fixture = new RootCommandFixture();

        // When
        var result = fixture.ParseAndSerialize("--", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--" Kind="DoubleDash" Span="0:2" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Treat_Everything_After_Double_Dash_As_Arguments()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndSerialize("-- --lol", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="--" Kind="DoubleDash" Span="0:2" />
                <Token Lexeme="--lol" Kind="Argument" Span="3:5" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Unbundle_Options_And_Treat_Unknown_Part_As_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("-a"));
        fixture.Options.Add(new Option<int>("-b"));
        fixture.Options.Add(new Option<int>("-c"));

        // When
        var result = fixture.ParseAndSerialize("-abfg", parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="-a" Kind="Option" Span="1:1" />
                <Token Lexeme="-b" Kind="Option" Span="2:1" />
                <Token Lexeme="fg" Kind="Argument" Span="3:2" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Tokenize_Sub_Commands_Correctly()
    {
        // Given
        var command = new Command("foo");
        command.AddArgument(new Argument<string>("<FOO>"));
        command.AddOption(new Option<int>("--lol"));
        command.AddOption(new Option<bool>("--bar"));

        var command2 = new Command("bar");
        command2.AddOption(new Option<bool>("--corgi"));
        command.AddCommand(command2);

        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndSerialize(
            "foo root --bar --lol qux bar --corgi",
            parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="foo" Kind="Command" Span="0:3" />
                <Token Lexeme="root" Kind="Argument" Span="4:4" />
                <Token Lexeme="--bar" Kind="Option" Span="9:5" />
                <Token Lexeme="--lol" Kind="Option" Span="15:5" />
                <Token Lexeme="qux" Kind="Argument" Span="21:3" />
                <Token Lexeme="bar" Kind="Command" Span="25:3" />
                <Token Lexeme="--corgi" Kind="Option" Span="29:7" />
              </Tokens>
            </ParseResult>
            """);
    }

    [Fact]
    public void Should_Preserve_Positions_In_Token()
    {
        // Given
        var command = new Command("foo");
        command.AddArgument(new Argument<string>("<FOO>"));
        command.AddOption(new Option<bool>("--bar"));
        command.AddOption(new Option<bool>("-a"));
        command.AddOption(new Option<bool>("-b"));
        command.AddOption(new Option<bool>("-c"));
        command.AddOption(new Option<int>("--lol"));

        var command2 = new Command("bar");
        command2.AddOption(new Option<bool>("--corgi"));
        command.AddCommand(command2);

        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndSerialize(
            "foo root --bar -abf --lol qux bar --corgi",
            parts: ParseResultParts.Tokens);

        // Then
        result.ShouldBe(
            """
            <ParseResult>
              <Tokens>
                <Token Lexeme="foo" Kind="Command" Span="0:3" />
                <Token Lexeme="root" Kind="Argument" Span="4:4" />
                <Token Lexeme="--bar" Kind="Option" Span="9:5" />
                <Token Lexeme="-a" Kind="Option" Span="16:1" />
                <Token Lexeme="-b" Kind="Option" Span="17:1" />
                <Token Lexeme="f" Kind="Argument" Span="18:1" />
                <Token Lexeme="--lol" Kind="Option" Span="20:5" />
                <Token Lexeme="qux" Kind="Argument" Span="26:3" />
                <Token Lexeme="bar" Kind="Command" Span="30:3" />
                <Token Lexeme="--corgi" Kind="Option" Span="34:7" />
              </Tokens>
            </ParseResult>
            """);
    }
}