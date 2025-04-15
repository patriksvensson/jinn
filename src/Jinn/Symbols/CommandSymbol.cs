namespace Jinn;

public sealed class CommandSymbol : Symbol
{
    public required string Name { get; init; }
    public required List<CommandSymbol> Commands { get; init; }
    public required List<ArgumentSymbol> Arguments { get; init; }
    public required List<OptionSymbol> Options { get; init; }
}
