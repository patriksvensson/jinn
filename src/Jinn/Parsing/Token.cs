namespace Jinn;

[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class Token
{
    public TokenKind Kind { get; }
    public Symbol? Symbol { get; set; }
    public TextSpan Span { get; }
    public string Lexeme { get; }

    public Token(TokenKind kind, Symbol? symbol, TextSpan span, string? lexeme)
    {
        Kind = kind;
        Symbol = symbol;
        Span = span;
        Lexeme = lexeme ?? string.Empty;
    }

    private string GetDebugString()
    {
        return !string.IsNullOrWhiteSpace(Lexeme)
            ? $"{Kind}: \"{Lexeme}\"" : $"{Kind}";
    }
}

[PublicAPI]
public enum TokenKind
{
    Command,
    Argument,
    Option,
    DoubleDash,
}