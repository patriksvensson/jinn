namespace Jinn;

[PublicAPI]
public abstract class Option : Symbol
{
    public HashSet<string> Aliases { get; init; } = [];
    public Argument Argument { get; }

    public bool IsRequired
    {
        get => Argument.IsRequired;
        set => Argument.IsRequired = value;
    }

    protected Option(string name, Argument argument)
    {
        Aliases.Add(name);
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
    }
}

[PublicAPI]
public sealed class Option<T> : Option
{
    public Option(string name)
        : base(name, new Argument<T>("value"))
    {
    }
}