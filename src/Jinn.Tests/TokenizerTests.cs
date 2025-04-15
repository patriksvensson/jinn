namespace Jinn.Tests;

public class TokenizerTests
{
    [Fact]
    public void Should_Tokenize_Single_Argument()
    {
        // Given
        var command = new RootCommand();
        command.Arguments.Add(new Argument<string>("<FOO>"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "foo");

        // Then
        result.ShouldHaveTokens("(Argument)foo");
    }

    [Fact]
    public void Should_Tokenize_Multiple_Argument()
    {
        // Given
        var command = new RootCommand();
        command.Arguments.Add(new Argument<List<string>>("<FOO>"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "foo");

        // Then
        result.ShouldHaveTokens("(Argument)foo");
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
        var command = new RootCommand();
        command.Arguments.Add(new Argument<string>("<FOO>") { Arity = new Arity(min, max) });

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "foo bar");

        // Then
        result.ShouldHaveTokens("(Argument)foo (Argument)bar");
    }

    [Theory]
    [InlineData("--lol", "(Option)--lol")]
    [InlineData("--lol 42", "(Option)--lol (OptionArgument)42")]
    [InlineData("--lol 42 32", "(Option)--lol (OptionArgument)42 (Argument)32")]
    public void Should_Tokenize_Option(string args, string expected)
    {
        // Given
        var command = new RootCommand();
        command.Options.Add(new Option<int>("--lol"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, args);

        // Then
        result.ShouldHaveTokens(expected);
    }

    [Theory]
    [InlineData("--lol=32")]
    [InlineData("--lol:32")]
    public void Should_Split_Argument_Values(string args)
    {
        // Given
        var command = new RootCommand();
        command.Options.Add(new Option<int>("--lol"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, args);

        // Then
        result.ShouldHaveTokens(
            "(Option)--lol (OptionArgument)32");
    }

    [Fact]
    public void Should_Unbundle_Options()
    {
        // Given
        var command = new RootCommand();
        command.Options.Add(new Option<int>("-a"));
        command.Options.Add(new Option<int>("-b"));
        command.Options.Add(new Option<int>("-c"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "-ac");

        // Then
        result.ShouldHaveTokens(
            "(Option)-a (Option)-c");
    }

    [Fact]
    public void Should_Acknowledge_Double_Dash()
    {
        // Given
        var command = new RootCommand();

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "--");

        // Then
        result.ShouldHaveTokens("(DoubleDash)--");
    }

    [Fact]
    public void Should_Treat_Everything_After_Double_Dash_As_Arguments()
    {
        // Given
        var command = new RootCommand();
        command.Options.Add(new Option<int>("--lol"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, "-- --lol");

        // Then
        result.ShouldHaveTokens("(DoubleDash)-- (Argument)--lol");
    }

    [Theory]
    [InlineData("-abf", "(Option)-a (Option)-b (Argument)f")]
    [InlineData("-afg", "(Option)-a (Argument)fg")]
    public void Should_Unbundle_Options_And_Treat_Unknown_Part_As_Argument(string args, string expected)
    {
        // Given
        var command = new RootCommand();
        command.Options.Add(new Option<int>("-a"));
        command.Options.Add(new Option<int>("-b"));
        command.Options.Add(new Option<int>("-c"));

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            command, args);

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

        var root = new RootCommand(command);

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            root, "foo root --bar --lol qux bar --corgi");

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Argument)root (Option)--bar (Option)--lol (OptionArgument)qux " +
            "(Command)bar (Option)--corgi");
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

        var root = new RootCommand(command);

        // When
        var result = TokenizerFixture.TokenizeAndReturnTokens(
            root, "foo root --bar -abf --lol qux bar --corgi");

        // Then
        result.ShouldHaveTokenSpans(
            "(0:3)foo (4:4)root (9:5)--bar (16:1)-a (17:1)-b (18:1)f " +
            "(20:5)--lol (26:3)qux (30:3)bar (34:7)--corgi");
    }
}