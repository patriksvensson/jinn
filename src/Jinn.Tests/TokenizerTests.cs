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
        var result = Tokenizer.Tokenize("foo", command);

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
        var result = Tokenizer.Tokenize("foo", command);

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
        var result = Tokenizer.Tokenize("foo bar", command);

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
        var result = Tokenizer.Tokenize(args, command);

        // Then
        result.ShouldHaveTokens(expected);
    }

    [Theory]
    [InlineData("--lol=32")]
    [InlineData("--lol:32")]
    public void Should_Split_Argument_Values(string args)
    {
        // Given
        var root = new RootCommand();
        root.Options.Add(new Option<int>("--lol"));

        // When
        var result = Tokenizer.Tokenize(args, root);

        // Then
        result.ShouldHaveTokens(
            "(Option)--lol (OptionArgument)32");
    }

    [Fact]
    public void Should_Unbundle_Options()
    {
        // Given
        var root = new RootCommand();
        root.Options.Add(new Option<int>("-a"));
        root.Options.Add(new Option<int>("-b"));
        root.Options.Add(new Option<int>("-c"));

        // When
        var result = Tokenizer.Tokenize("-ac", root);

        // Then
        result.ShouldHaveTokens(
            "(Option)-a (Option)-c");
    }

    [Theory]
    [InlineData("-abf", "(Option)-a (Option)-b (Argument)f")]
    [InlineData("-afg", "(Option)-a (Argument)fg")]
    public void Should_Unbundle_Options_And_Treat_Unknown_Part_As_Argument(string args, string expected)
    {
        // Given
        var root = new RootCommand();
        root.Options.Add(new Option<int>("-a"));
        root.Options.Add(new Option<int>("-b"));
        root.Options.Add(new Option<int>("-c"));

        // When
        var result = Tokenizer.Tokenize(args, root);

        // Then
        result.ShouldHaveTokens(expected);
    }


    [Fact]
    public void Should_Tokenize_Sub_Commands_Correctly()
    {
        // Givenn
        var command = new Command("foo");
        command.Arguments.Add(new Argument<string>("<FOO>"));
        command.Options.Add(new Option<int>("--lol"));
        command.Options.Add(new Option<bool>("--bar"));

        var command2 = new Command("bar");
        command2.Options.Add(new Option<bool>("--corgi"));
        command.Commands.Add(command2);

        var root = new RootCommand(command);

        // When
        var result = Tokenizer.Tokenize("foo root --bar --lol qux bar --corgi", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Argument)root (Option)--bar (Option)--lol (OptionArgument)qux " +
            "(Command)bar (Option)--corgi");
    }
}