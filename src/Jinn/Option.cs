namespace Jinn;

public abstract class Option : Symbol
{
    public HashSet<string> Aliases { get; init; } = [];
    public Arity Arity => Argument.Arity;
    public Type ValueType => Argument.ValueType;
    public abstract Argument Argument { get; }

    protected Option(string name)
    {
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
    private readonly Argument<T> _argument;

    public override Argument Argument => _argument;

    public Option(string name)
        : base(name)
    {
        _argument = new Argument<T>(name);
    }
}