namespace Jinn;

public sealed class Argument : Symbol
{
    public string Name { get; }

    public Argument(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override IEnumerable<Symbol> GetOwnedSymbols()
    {
        yield break;
    }

    public override IEnumerable<string> GetNames()
    {
        yield return Name;
    }
}