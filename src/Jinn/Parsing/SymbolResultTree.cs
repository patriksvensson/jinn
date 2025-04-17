namespace Jinn;

internal sealed class SymbolResultTree : Dictionary<Symbol, SymbolResult>
{
    public CommandSymbol Root { get; }
    public List<Token> Unmatched { get; }

    public SymbolResultTree(CommandSymbol root)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));
        Unmatched = [];
    }

    public bool TryGetResult<T>(Symbol key, [NotNullWhen(true)] out T? result)
        where T : SymbolResult
    {
        if (TryGetValue(key, out var symbolResult))
        {
            result = symbolResult as T;
            return result != null;
        }

        result = null;
        return false;
    }
}