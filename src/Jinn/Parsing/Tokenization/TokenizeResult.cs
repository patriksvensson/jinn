namespace Jinn;

public sealed class TokenizeResult
{
    public required string Raw { get; init; }
    public required string[] Arguments { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
}