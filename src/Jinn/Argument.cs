namespace Jinn;

[PublicAPI]
public class Argument
{
    public string Name { get; }
    public Arity Arity { get; init; }
    public Type ValueType { get; }
    public string? Description { get; set; }

    public Argument(Type type, string name)
    {
        ValueType = type;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    internal ArgumentSymbol CreateSymbol()
    {
        return new ArgumentSymbol
        {
            Name = Name,
            Arity = Arity,
            ValueType = ValueType,
            Hidden = false,
            Description = Description,
        };
    }
}

[PublicAPI]
public sealed class Argument<T> : Argument
{
    public Argument(string name)
        : base(typeof(T), name)
    {
    }
}