namespace Jinn;

[PublicAPI]
internal sealed class SyntaxTree
{
    public required CommandSyntax Root { get; init; }
    public required Configuration Configuration { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }
    public required IReadOnlyList<Token> Unmatched { get; init; }
}