namespace Jinn.Testing;

public static class ParserFixture
{
    public static ParseResult Parse(RootCommand command, string args)
    {
        return command.Parse(StringSplitter.Split(args));
    }

    public static IReadOnlyList<Token> ParseAndReturnTokens(RootCommand command, string args)
    {
        var parts = StringSplitter.Split(args);
        return command.Parse(parts).Tokens;
    }
}