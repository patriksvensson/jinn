namespace Jinn;

public sealed class ParseResult
{
    public required CommandResult Root { get; init; }
    public required CommandResult Command { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
    public required IReadOnlyList<Token> UnmatchedTokens { get; init; }
    public required IReadOnlyList<string> Arguments { get; init; }
}