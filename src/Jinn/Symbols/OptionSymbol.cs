namespace Jinn;

public sealed class OptionSymbol : Symbol
{
    public required HashSet<string> Aliases { get; init; }
    public required ArgumentSymbol Argument { get; init; }

    public override IEnumerable<Symbol> GetChildren()
    {
        yield break;
    }

    public override IEnumerable<string> GetNames()
    {
        foreach (var alias in Aliases)
        {
            yield return alias;
        }
    }
}
