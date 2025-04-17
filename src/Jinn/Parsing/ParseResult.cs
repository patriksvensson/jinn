namespace Jinn;

[PublicAPI]
public sealed class ParseResult
{
    public required CommandSymbol Root { get; init; }
    public required CommandSymbol Command { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
    public required IReadOnlyList<Token> UnmatchedTokens { get; init; }
    public required IReadOnlyList<string> Arguments { get; init; }
}