namespace Jinn;

public sealed class Configuration
{
    public Func<ParseResult, Task<IInvocationResult>>? ParseErrorHandler { get; set; }
    public Func<Exception, ParseResult, Task<IInvocationResult>>? ExceptionHandler { get; set; }
}