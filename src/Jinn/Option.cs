namespace Jinn;

[PublicAPI]
public abstract class Option : Symbol
{
    public HashSet<string> Aliases { get; init; } = [];
    internal Argument Argument { get; }

    public Arity Arity
    {
        get => Argument.Arity;
        set => Argument.Arity = value;
    }

    public bool IsRequired
    {
        get => Argument.IsRequired;
        set => Argument.IsRequired = value;
    }

    public string Name
    {
        // TODO: Make sure this can't be cleared
        get => Aliases.First();
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

[PublicAPI]
public static class OptionExtensions
{
    public static Option<T> AddAlias<T>(this Option<T> option, string alias)
    {
        ArgumentNullException.ThrowIfNull(option);
        ArgumentNullException.ThrowIfNull(alias);
        option.Aliases.Add(alias);
        return option;
    }

    public static Option<T> AddAliases<T>(this Option<T> option, params string[] aliases)
    {
        ArgumentNullException.ThrowIfNull(option);
        ArgumentNullException.ThrowIfNull(aliases);

        foreach (var alias in aliases)
        {
            option.AddAlias(alias);
        }

        return option;
    }

    public static Option<T> HasArity<T>(this Option<T> option, Arity arity)
    {
        ArgumentNullException.ThrowIfNull(option);
        option.Arity = arity;
        return option;
    }

    public static Option<T> HasArity<T>(this Option<T> option, int min, int max)
    {
        ArgumentNullException.ThrowIfNull(option);
        option.Arity = new Arity(min, max);
        return option;
    }

    public static Option<T> Required<T>(this Option<T> option, bool isRequired = true)
    {
        ArgumentNullException.ThrowIfNull(option);
        option.IsRequired = isRequired;
        return option;
    }
}