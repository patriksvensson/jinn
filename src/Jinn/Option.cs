namespace Jinn;

[PublicAPI]
[DebuggerDisplay("{Name,nq}")]
public abstract class Option : Symbol
{
    public OptionName Name { get; }
    public IReadOnlySet<OptionName> Names { get; }
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

    protected Option(string template, Argument argument)
    {
        // Parse all names from the provided template.
        // Use the first provided long name in the template as the option name.
        // If none exist, use the first name in the list.
        var names = TemplateParser.ParseOption(template ?? throw new ArgumentNullException(nameof(template)));
        Names = new HashSet<OptionName>(names, OptionName.Comparer.Shared);
        Name = names.FirstOrDefault(x => x.IsShort) ?? names[0];

        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
    }
}

[PublicAPI]
public sealed class Option<T> : Option
{
    public Option(string template)
        : base(template, new Argument<T>("value"))
    {
    }
}

[PublicAPI]
[DebuggerDisplay("{Expanded,nq} ({Name,nq})")]
public sealed class OptionName
{
    public string Name { get; }
    public bool IsLong { get; }
    public bool IsShort => !IsLong;

    private OptionName(string name, bool isLong)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsLong = isLong;
    }

    internal static OptionName Long(string name)
    {
        return new OptionName(name, true);
    }

    internal static OptionName Short(string name)
    {
        return new OptionName(name, false);
    }

    public override string ToString()
    {
        return Name;
    }

    public sealed class Comparer : IEqualityComparer<OptionName>
    {
        public static Comparer Shared { get; } = new();

        public bool Equals(OptionName? x, OptionName? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null)
            {
                return false;
            }

            if (y is null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Name == y.Name && x.IsLong == y.IsLong;
        }

        public int GetHashCode(OptionName obj)
        {
            return HashCode.Combine(obj.Name, obj.IsLong);
        }
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