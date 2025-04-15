namespace Jinn;

public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public abstract IEnumerable<Symbol> GetChildren();
    public abstract IEnumerable<string> GetNames();
}