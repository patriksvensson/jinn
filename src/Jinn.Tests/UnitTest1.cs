using Jinn.Testing;

namespace Jinn.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Given
        var command = new Command("foo");
        command.Arguments.Add(new Argument("<FOO>"));
        command.Options.Add(new Option<int>("--lol"));
        command.Options.Add(new Option<bool>("--bar"));
        var root = new CommandContainer([command]);

        // When
        var result = Tokenizer.Tokenize("foo root --bar --lol qux", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Argument)root (Option)--bar (Option)--lol (OptionArgument)qux");
    }

    [Fact]
    public void Test1_1()
    {
        // Given
        var command = new Command("foo");
        command.Options.Add(new Option<int>("--lol"));
        var root = new CommandContainer([command]);

        // When
        var result = Tokenizer.Tokenize("foo --lol=32", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Option)--lol (OptionArgument)32");
    }

    [Fact]
    public void Test1_2()
    {
        // Given
        var command = new Command("foo");
        command.Options.Add(new Option<int>("-a"));
        command.Options.Add(new Option<int>("-b"));
        command.Options.Add(new Option<int>("-c"));
        var root = new CommandContainer([command]);

        // When
        var result = Tokenizer.Tokenize("foo -ac", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Option)-a (Option)-c");
    }

    [Fact]
    public void Test1_3()
    {
        // Given
        var command = new Command("foo");
        command.Options.Add(new Option<int>("-a"));
        command.Options.Add(new Option<int>("-b"));
        command.Options.Add(new Option<int>("-c"));
        var root = new CommandContainer([command]);

        // When
        var result = Tokenizer.Tokenize("foo -af", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Option)-a (Argument)f");
    }

    [Fact]
    public void Test2()
    {
        // Givenn
        var command = new Command("foo");
        command.Arguments.Add(new Argument("<FOO>"));
        command.Options.Add(new Option<int>("--lol"));
        command.Options.Add(new Option<bool>("--bar"));

        var command2 = new Command("bar");
        command2.Options.Add(new Option<bool>("--corgi"));
        command.Commands.Add(command2);

        var root = new CommandContainer([command]);

        // When
        var result = Tokenizer.Tokenize("foo root --bar --lol qux bar --corgi", root);

        // Then
        result.ShouldHaveTokens(
            "(Command)foo (Argument)root (Option)--bar (Option)--lol (OptionArgument)qux " +
            "(Command)bar (Option)--corgi");
    }
}