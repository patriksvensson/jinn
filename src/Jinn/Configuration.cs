namespace Jinn;

public sealed class Configuration
{
    public Action<ParseResult>? ErrorHandler { get; set; }
}