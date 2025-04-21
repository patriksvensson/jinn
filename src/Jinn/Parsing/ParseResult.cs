namespace Jinn;

[PublicAPI]
public sealed class ParseResult
{
    public required RootCommandResult ParsedCommand { get; init; }
    public required Configuration Configuration { get; init; }
    public required IReadOnlyList<Token> Unmatched { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
    public required Dictionary<Symbol, SymbolResult> Lookup { get; init; }
}