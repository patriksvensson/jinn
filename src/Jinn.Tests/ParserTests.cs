using Shouldly;

namespace Jinn.Tests;

public class ParserTests
{
    [Fact]
    public void Test1()
    {
        // Given
        var command = new Command();
        command.Options.Add(new Option<List<string>>("--lol"));

        // When
        var result = Tokenizer.Tokenize("--lol bar --lol qux", command);
        Parser.Parse(command, result);

        // Then
    }
}