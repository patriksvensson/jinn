using Jinn.Binding;

namespace Jinn;

[PublicAPI]
public sealed class ParseResult
{
    public required RootCommandResult Root { get; init; }
    public required CommandResult? Command { get; init; }
    public required Diagnostics Diagnostics { get; init; }
    public required Configuration Configuration { get; init; }
    public required IReadOnlyList<Token> Unmatched { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
    public required Dictionary<Symbol, SymbolResult> Lookup { get; init; }
}