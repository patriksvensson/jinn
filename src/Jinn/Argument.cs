namespace Jinn;

[PublicAPI]
public abstract class Argument : Symbol
{
    public string Name { get; }
    public Arity Arity { get; set; }
    public Type ValueType { get; }
    public bool IsRequired { get; }

    protected Argument(Type type, string name)
    {
        ValueType = type;
        Arity = Arity.Resolve(type);
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public bool IsBoolean()
    {
        return ValueType == typeof(bool);
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