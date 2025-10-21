namespace Jinn;

[PublicAPI]
public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }
    public Symbol? Parent { get; internal set; }
}

[PublicAPI]
public static class SymbolExtensions
{
    extension<T>(T symbol)
        where T : Symbol
    {
        public T Description(string? description)
        {
            symbol.Description = description;
            return symbol;
        }

        public T Hidden(bool hidden = true)
        {
            symbol.Hidden = hidden;
            return symbol;
        }
    }
}