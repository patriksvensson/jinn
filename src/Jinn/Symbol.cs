namespace Jinn;

public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public abstract IEnumerable<Symbol> GetOwnedSymbols();
    public abstract IEnumerable<string> GetNames();
}

public static class SymbolExtensions
{
    public static IDictionary<string, Symbol> GetSymbolDictionary(this IEnumerable<Symbol> symbols)
    {
        var result = new Dictionary<string, Symbol>(StringComparer.Ordinal);

        foreach (var symbol in symbols)
        {
            foreach (var name in symbol.GetNames())
            {
                result[name] = symbol;
            }
        }

        return result;
    }
}