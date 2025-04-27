namespace Jinn;

[PublicAPI]
public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }
    public Symbol? Parent { get; internal set; }
}

public static class SymbolExtensions
{
    public static T Description<T>(this T symbol, string? description)
        where T : Symbol
    {
        symbol.Description = description;
        return symbol;
    }

    public static T Hidden<T>(this T symbol, bool hidden = true)
        where T : Symbol
    {
        symbol.Hidden = hidden;
        return symbol;
    }
}