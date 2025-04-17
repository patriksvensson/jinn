namespace Jinn;

/// <summary>
/// Represents a snapshot at the time of parsing.
/// </summary>
[PublicAPI]
public abstract class Symbol
{
    private readonly List<Token> _tokens;

    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public Symbol? Parent { get; internal set; }
    public IReadOnlyList<Token> Tokens => _tokens;

    protected Symbol()
    {
        _tokens = [];
    }

    internal void AddToken(Token token)
    {
        _tokens.Add(token);
    }
}

/// <summary>
/// Represents a snapshot of an argument at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class ArgumentSymbol : Symbol
{
    public Argument Owner { get; }

    public string Name { get; }
    public Arity Arity { get; }
    public Type ValueType { get; }

    public bool IsBoolean() => ValueType == typeof(bool);

    internal ArgumentSymbol(Argument owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Name = owner.Name;
        Description = owner.Description;
        Arity = owner.Arity;
        ValueType = owner.ValueType;
    }

    private string GetDebugString()
    {
        return $"{Name} {Arity.GetDebugString()}";
    }
}

/// <summary>
/// Represents a snapshot of a command at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class CommandSymbol : Symbol
{
    public Command Owner { get; }
    public string Name { get; }

    public bool HasArguments => Arguments.Count > 0;
    public bool HasOptions => Options.Count > 0;

    public IReadOnlyList<CommandSymbol> Commands { get; }
    public IReadOnlyList<ArgumentSymbol> Arguments { get; }
    public IReadOnlyList<OptionSymbol> Options { get; }

    internal CommandSymbol(Command owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Name = owner.Name;
        Description = owner.Description;
        Hidden = owner.Hidden;
        Commands = owner.Commands.ConvertAll(x => x.CreateSymbol());
        Arguments = owner.Arguments.ConvertAll(x => x.CreateSymbol());
        Options = owner.Options.ConvertAll(x => x.CreateSymbol());
    }

    private string GetDebugString()
    {
        return Name;
    }
}

/// <summary>
/// Represents a snapshot of an option at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class OptionSymbol : Symbol
{
    public Option Owner { get; }

    public IReadOnlySet<string> Aliases { get; }
    public Arity Arity { get; }
    public Type ValueType { get; }

    public bool IsBoolean() => ValueType == typeof(bool);

    internal OptionSymbol(Option owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Aliases = new HashSet<string>(owner.Aliases, StringComparer.Ordinal);
        Description = owner.Description;
        Hidden = owner.Hidden;
        Arity = owner.Arity;
        ValueType = owner.ValueType;
    }

    private string GetDebugString()
    {
        return $"{Aliases.First()} ({Arity.GetDebugString()})";
    }
}
