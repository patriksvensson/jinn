namespace Jinn;

[PublicAPI]
public abstract class Option : Symbol
{
    public string Name { get; }
    public HashSet<string> Aliases { get; init; } = [];
    internal Argument Argument { get; }

    public Func<InvocationContext, Task<bool>>? Handler
    {
        get => Argument.Handler;
        set => Argument.Handler = value;
    }

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

    internal IEnumerable<string> NameAndAliases
    {
        get
        {
            yield return Name;
            foreach (var alias in Aliases)
            {
                yield return alias;
            }
        }
    }

    protected Option(string name, Argument argument)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
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
    extension<T>(Option<T> option)
    {
        public void SetHandler(Func<InvocationContext, Task<bool>> handler)
        {
            option.Handler = async (ctx) => await handler(ctx);
        }

        public void SetHandler(Func<InvocationContext, bool> handler)
        {
            option.Handler = ctx =>
            {
                var result = handler(ctx);
                return Task.FromResult(result);
            };
        }
    }
}