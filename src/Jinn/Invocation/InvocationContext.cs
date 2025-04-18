namespace Jinn;

[PublicAPI]
public sealed class InvocationContext
{
    public ParseResult ParseResult { get; }
    public int ExitCode { get; set; }

    public InvocationContext(ParseResult parseResult)
    {
        ParseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
    }
}