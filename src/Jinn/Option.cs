namespace Jinn;

public class Option
{
    public HashSet<string> Aliases { get; init; } = [];
    public Arity Arity { get; init; }
    public Type ValueType { get; }

    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public Option(Type type, string name)
    {
        ValueType = type;
        Arity = Arity.Resolve(type);
        Aliases.Add(name);
    }

    internal OptionSymbol CreateSymbol()
    {
        return new OptionSymbol
        {
            Aliases = new HashSet<string>(Aliases, StringComparer.Ordinal),
            Description = Description,
            Hidden = Hidden,
            Argument = new ArgumentSymbol
            {
                Arity = Arity,
                Name = "VALUE",
                Hidden = false,
                ValueType = ValueType,
            },
        };
    }
}

public sealed class Option<T> : Option
{
    public Option(string name)
        : base(typeof(T), name)
    {
    }
}