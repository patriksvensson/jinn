namespace Jinn;

public class Option : Symbol
{
    public HashSet<string> Aliases { get; init; } = [];
    public Arity Arity { get; init; }
    public Type ValueType { get; }

    protected Option(Type valueType, string name)
    {
        ValueType = valueType;
        Arity = Arity.Resolve(valueType);
        Aliases.Add(name);
    }

    public override IEnumerable<Symbol> GetOwnedSymbols()
    {
        yield break;
    }

    public override IEnumerable<string> GetNames()
    {
        return Aliases;
    }
}

public sealed class Option<T> : Option
{
    public Option(string name)
        : base(typeof(T), name)
    {
    }
}