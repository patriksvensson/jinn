namespace Jinn;

public abstract class Argument : Symbol
{
    public string Name { get; }
    public Arity Arity { get; init; }
    public Type ValueType { get; }

    protected Argument(Type valueType, string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ValueType = valueType;
        Arity = Arity.Resolve(valueType);
    }

    public override IEnumerable<Symbol> GetOwnedSymbols()
    {
        yield break;
    }

    public override IEnumerable<string> GetNames()
    {
        yield return Name;
    }
}

public sealed class Argument<T> : Argument
{
    public Argument(string name)
        : base(typeof(T), name)
    {
    }
}