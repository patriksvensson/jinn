namespace Jinn;

[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class Token
{
    public TokenType Type { get; }
    public Symbol? Symbol { get; }
    public int Position { get; }
    public string Value { get; }

    public Token(TokenType type, Symbol? symbol, int position, string? value)
    {
        Type = type;
        Symbol = symbol;
        Position = position;
        Value = value ?? string.Empty;
    }

    private string GetDebugString()
    {
        return !string.IsNullOrWhiteSpace(Value)
            ? $"{Type}: \"{Value}\"" : $"{Type}";
    }
}