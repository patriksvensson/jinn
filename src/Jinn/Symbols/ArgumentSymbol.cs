namespace Jinn;

public sealed class ArgumentSymbol : Symbol
{
    public required string Name { get; init; }
    public required Arity Arity { get; init; }
    public required Type ValueType { get; init; }
}