namespace Jinn;

[PublicAPI]
public sealed class InvocationContext
{
    public ParseResult ParseResult { get; }
    public Configuration Configuration { get; }
    public int ExitCode { get; set; }

    public InvocationContext(ParseResult parseResult, Configuration configuration)
    {
        ParseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        Configuration = configuration;
    }
}