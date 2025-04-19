namespace Jinn;

/// <summary>
/// Represents a symbol at the time of parsing.
/// </summary>
[PublicAPI]
public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }
    public Symbol? Parent { get; internal set; }

    public abstract SymbolResult? GetResult();
}

/// <summary>
/// Represents a symbol that has a value.
/// </summary>
[PublicAPI]
public abstract class ValueSymbol : Symbol
{
    public Arity Arity { get; }
    public Type ValueType { get; }
    public bool IsRequired { get; }

    protected ValueSymbol(Arity arity, Type valueType, bool isRequired)
    {
        Arity = arity;
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        IsRequired = isRequired;
    }
}

/// <summary>
/// Represents a command at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class CommandSymbol : Symbol
{
    public Command Owner { get; }
    public string Name { get; }

    public bool IsRootCommand { get; }
    public bool HasArguments => Arguments.Count > 0;
    public bool HasOptions => Options.Count > 0;

    public IReadOnlyList<CommandSymbol> Commands { get; }
    public IReadOnlyList<ArgumentSymbol> Arguments { get; }
    public IReadOnlyList<OptionSymbol> Options { get; }
    public CommandResult? Result { get; internal set; }

    internal CommandSymbol(Command owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        IsRootCommand = owner is RootCommand;
        Name = owner.Name;
        Description = owner.Description;
        Hidden = owner.Hidden;
        Commands = owner.Commands.ConvertAll(x => x.CreateSymbol());
        Arguments = owner.Arguments.ConvertAll(x => x.CreateSymbol());
        Options = owner.Options.ConvertAll(x => x.CreateSymbol());
    }

    public override SymbolResult? GetResult()
    {
        return Result;
    }

    private string GetDebugString()
    {
        return Name;
    }
}

/// <summary>
/// Represents a argument at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class ArgumentSymbol : ValueSymbol
{
    public Argument Owner { get; }
    public string Name { get; }
    public ArgumentResult? Result { get; internal set; }

    public bool IsBoolean() => ValueType == typeof(bool);

    internal ArgumentSymbol(Argument owner)
        : base(owner.Arity, owner.ValueType, owner.IsRequired)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Name = owner.Name;
        Description = owner.Description;
    }

    public override SymbolResult? GetResult()
    {
        return Result;
    }

    private string GetDebugString()
    {
        return $"{Name} {Arity.GetDebugString()}";
    }
}

/// <summary>
/// Represents an option at the time of parsing.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{GetDebugString(),nq}")]
public sealed class OptionSymbol : ValueSymbol
{
    public Option Owner { get; }

    public IReadOnlySet<string> Aliases { get; }
    public OptionResult? Result { get; internal set; }

    public bool IsBoolean() => ValueType == typeof(bool);

    internal OptionSymbol(Option owner)
        : base(owner.Arity, owner.ValueType, owner.IsRequired)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Aliases = new HashSet<string>(owner.Aliases, StringComparer.Ordinal);
        Description = owner.Description;
        Hidden = owner.Hidden;
    }

    public override SymbolResult? GetResult()
    {
        return Result;
    }

    private string GetDebugString()
    {
        return $"{Aliases.First()} ({Arity.GetDebugString()})";
    }
}