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
        var result = fixture.ParseAndReturnTokens("foo");

        // Then
        result.ShouldHaveTokens("(Argument)<ExecutableName> (Argument)foo");
    }

    [Fact]
    public void Should_Tokenize_Multiple_Argument()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Arguments.Add(new Argument<List<string>>("<FOO>"));

        // When
        var result = fixture.ParseAndReturnTokens("foo");

        // Then
        result.ShouldHaveTokens("(Argument)<ExecutableName> (Argument)foo");
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
        var result = fixture.ParseAndReturnTokens("foo bar");

        // Then
        result.ShouldHaveTokens("(Argument)<ExecutableName> (Argument)foo (Argument)bar");
    }

    [Theory]
    [InlineData("--lol", "(Argument)<ExecutableName> (Option)--lol")]
    [InlineData("--lol 42", "(Argument)<ExecutableName> (Option)--lol (Argument)42")]
    [InlineData("--lol 42 32", "(Argument)<ExecutableName> (Option)--lol (Argument)42 (Argument)32")]
    public void Should_Tokenize_Option(string args, string expected)
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndReturnTokens(args);

        // Then
        result.ShouldHaveTokens(expected);
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
        var result = fixture.ParseAndReturnTokens(args);

        // Then
        result.ShouldHaveTokens(
            "(Argument)<ExecutableName> (Option)--lol (Argument)32");
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
        var result = fixture.ParseAndReturnTokens("-ac");

        // Then
        result.ShouldHaveTokens(
            "(Argument)<ExecutableName> (Option)-a (Option)-c");
    }

    [Fact]
    public void Should_Acknowledge_Double_Dash()
    {
        // Given
        var fixture = new RootCommandFixture();

        // When
        var result = fixture.ParseAndReturnTokens("--");

        // Then
        result.ShouldHaveTokens("(Argument)<ExecutableName> (DoubleDash)--");
    }

    [Fact]
    public void Should_Treat_Everything_After_Double_Dash_As_Arguments()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("--lol"));

        // When
        var result = fixture.ParseAndReturnTokens("-- --lol");

        // Then
        result.ShouldHaveTokens("(Argument)<ExecutableName> (DoubleDash)-- (Argument)--lol");
    }

    [Theory]
    [InlineData("-abf", "(Argument)<ExecutableName> (Option)-a (Option)-b (Argument)f")]
    [InlineData("-afg", "(Argument)<ExecutableName> (Option)-a (Argument)fg")]
    public void Should_Unbundle_Options_And_Treat_Unknown_Part_As_Argument(string args, string expected)
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.Options.Add(new Option<int>("-a"));
        fixture.Options.Add(new Option<int>("-b"));
        fixture.Options.Add(new Option<int>("-c"));

        // When
        var result = fixture.ParseAndReturnTokens(args);

        // Then
        result.ShouldHaveTokens(expected);
    }

    [Fact]
    public void Should_Tokenize_Sub_Commands_Correctly()
    {
        // Given
        var command = new Command("foo");
        command.Arguments.Add(new Argument<string>("<FOO>"));
        command.Options.Add(new Option<int>("--lol"));
        command.Options.Add(new Option<bool>("--bar"));

        var command2 = new Command("bar");
        command2.Options.Add(new Option<bool>("--corgi"));
        command.Commands.Add(command2);

        var fixture = new RootCommandFixture(command);

        // When
        var result = fixture.ParseAndReturnTokens(
            "foo root --bar --lol qux bar --corgi");

        // Then
        result.ShouldHaveTokens(
            "(Argument)<ExecutableName> (Command)foo (Argument)root (Option)--bar " +
            "(Option)--lol (Argument)qux (Command)bar (Option)--corgi");
    }

    [Fact]
    public void Should_Preserve_Positions_In_Token()
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
        var result = fixture.ParseAndReturnTokens(
            "foo root --bar -abf --lol qux bar --corgi");

        // Then
        result.ShouldHaveTokenSpans(
            "(0:10)<ExecutableName> (11:3)foo (15:4)root (20:5)--bar (27:1)-a " +
            "(28:1)-b (29:1)f (31:5)--lol (37:3)qux (41:3)bar (45:7)--corgi");
    }
}