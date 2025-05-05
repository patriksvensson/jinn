using Jinn.Binding;

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

    public void TryGetValue<T>(Option<T> option)
    {
        if (!ParseResult.Lookup.TryGetValue(option.Argument, out var result))
        {
            Trace.Assert(option.IsRequired, "Validation middleware should have caught this");
            throw new NotImplementedException("Value not found");
        }

        if (result is not ArgumentResult argumentResult)
        {
            throw new NotImplementedException("Should not occur: Result was wrong type");
        }

        ArgumentBinder.GetValue(argumentResult);
    }
}