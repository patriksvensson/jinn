namespace Jinn;

[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class Token
{
    public TokenType Type { get; }
    public Symbol? Symbol { get; }
    public TextSpan Span { get; }
    public string Value { get; }

    public Token(TokenType type, Symbol? symbol, TextSpan span, string? value)
    {
        Type = type;
        Symbol = symbol;
        Span = span;
        Value = value ?? string.Empty;
    }

    private string GetDebugString()
    {
        return !string.IsNullOrWhiteSpace(Value)
            ? $"{Type}: \"{Value}\"" : $"{Type}";
    }
}

public enum TokenType
{
    Command,
    Argument,
    Option,
    OptionArgument,
    DoubleDash,
}