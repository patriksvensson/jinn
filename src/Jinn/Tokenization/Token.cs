namespace Jinn;

[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class Token
{
    public TokenType TokenType { get; }
    public Symbol? Symbol { get; set; }
    public TextSpan Span { get; }
    public string Value { get; }

    public Token(TokenType tokenType, Symbol? symbol, TextSpan span, string? value)
    {
        TokenType = tokenType;
        Symbol = symbol;
        Span = span;
        Value = value ?? string.Empty;
    }

    private string GetDebugString()
    {
        return !string.IsNullOrWhiteSpace(Value)
            ? $"{TokenType}: \"{Value}\"" : $"{TokenType}";
    }
}

[PublicAPI]
public enum TokenType
{
    Command,
    Argument,
    Option,
    DoubleDash,
}