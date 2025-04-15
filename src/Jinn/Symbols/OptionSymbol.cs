namespace Jinn;

public sealed class OptionSymbol : Symbol
{
    public required HashSet<string> Aliases { get; init; }
    public required ArgumentSymbol Argument { get; init; }
}
