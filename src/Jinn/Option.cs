namespace Jinn;

[PublicAPI]
public abstract class Option : Symbol
{
    public HashSet<string> Aliases { get; init; } = [];
    public Argument Argument { get; }

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