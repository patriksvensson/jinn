namespace Jinn;

public sealed class ArgumentSymbol : Symbol
{
    public required string Name { get; init; }
    public required Arity Arity { get; init; }
    public required Type ValueType { get; init; }

    public override IEnumerable<Symbol> GetChildren()
    {
        yield break;
    }

    public override IEnumerable<string> GetNames()
    {
        yield break;
    }
}