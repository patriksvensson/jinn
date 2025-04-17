namespace Jinn;

/// <summary>
/// Represents a snapshot at the time of parsing.
/// </summary>
public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }
}

/// <summary>
/// Represents a snapshot of an argument at the time of parsing.
/// </summary>
public sealed class ArgumentSymbol : Symbol
{
    public required string Name { get; init; }
    public required Arity Arity { get; init; }
    public required Type ValueType { get; init; }

    public bool IsBoolean() => ValueType == typeof(bool);
}

/// <summary>
/// Represents a snapshot of a command at the time of parsing.
/// </summary>
public sealed class CommandSymbol : Symbol
{
    public required string Name { get; init; }

    public bool HasArguments => Arguments.Count > 0;
    public bool HasOptions => Options.Count > 0;

    public required List<CommandSymbol> Commands { get; init; }
    public required List<ArgumentSymbol> Arguments { get; init; }
    public required List<OptionSymbol> Options { get; init; }
}

/// <summary>
/// Represents a snapshot of an option at the time of parsing.
/// </summary>
public sealed class OptionSymbol : Symbol
{
    public required HashSet<string> Aliases { get; init; }
    public required ArgumentSymbol Argument { get; init; }
}
