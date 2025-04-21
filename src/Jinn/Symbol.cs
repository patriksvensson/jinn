namespace Jinn;

[PublicAPI]
public abstract class Symbol
{
    public string? Description { get; set; }
    public bool Hidden { get; set; }
    public Symbol? Parent { get; internal set; }
}