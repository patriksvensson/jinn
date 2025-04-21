namespace Jinn;

[PublicAPI]
public sealed class InvocationContext
{
    public ParseResult ParseResult { get; }
    public Configuration Configuration { get; }
    public IInvocationResult? InvocationResult { get; set; }
    public int ExitCode { get; set; }

    public InvocationContext(ParseResult parseResult)
    {
        ParseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        Configuration = parseResult.Configuration;
    }
}