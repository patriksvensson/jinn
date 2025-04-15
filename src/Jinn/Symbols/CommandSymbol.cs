namespace Jinn;

public sealed class CommandSymbol : Symbol
{
    public required string Name { get; init; }
    public required List<CommandSymbol> Commands { get; init; }
    public required List<ArgumentSymbol> Arguments { get; init; }
    public required List<OptionSymbol> Options { get; init; }

    public override IEnumerable<string> GetNames()
    {
        yield return Name;
    }

    public override IEnumerable<Symbol> GetChildren()
    {
        foreach (var command in Commands)
        {
            yield return command;
        }

        foreach (var argument in Arguments)
        {
            yield return argument;
        }

        foreach (var option in Options)
        {
            yield return option;
        }
    }
}
