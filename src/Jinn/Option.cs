namespace Jinn;

[PublicAPI]
public class Option
{
    public HashSet<string> Aliases { get; init; } = [];
    public Arity Arity { get; init; }
    public Type ValueType { get; }

    public string? Description { get; set; }
    public bool Hidden { get; set; }
    public bool IsRequired { get; set; }

    public Option(Type type, string name)
    {
        ValueType = type;
        Arity = Arity.Resolve(type);
        Aliases.Add(name);
    }

    internal OptionSymbol CreateSymbol()
    {
        return new OptionSymbol(this);
    }
}

[PublicAPI]
public sealed class Option<T> : Option
{
    public Option(string name)
        : base(typeof(T), name)
    {
    }
}