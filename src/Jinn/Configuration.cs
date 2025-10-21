namespace Jinn;

[PublicAPI]
public sealed class Configuration
{
    public string ExecutableName { get; init; } = GetExecutableName();

    public Func<ParseResult, Task<IInvocationResult>>? HelpProvider { get; set; }
    public Func<ParseResult, Task<IInvocationResult>>? ParseErrorHandler { get; set; }
    public Func<Exception, ParseResult, Task<IInvocationResult>>? ExceptionHandler { get; set; }

    internal List<InvocationMiddleware> Middlewares { get; } = [];

    public void AddMiddleware(InvocationMiddleware middleware)
    {
        Middlewares.Add(middleware);
    }

    private static string GetExecutableName()
    {
        var path = Environment.GetCommandLineArgs().First();
        return Path.GetFileNameWithoutExtension(path);
    }
}

[PublicAPI]
public static class ConfigurationExtensions
{
    extension(Configuration configuration)
    {
        public void SetHelpProvider(Action<InvocationContext> action)
        {
            configuration.HelpProvider = _ =>
                Task.FromResult<IInvocationResult>(
                    new CustomHelpInvocationResult(action));
        }

        public void SetParseErrorHandler(Action<InvocationContext> action)
        {
            configuration.ParseErrorHandler = _ =>
                Task.FromResult<IInvocationResult>(
                    new CustomParseErrorInvocationResult(action));
        }

        public void SetExceptionHandler(Action<InvocationContext, Exception> action)
        {
            configuration.ExceptionHandler = (ex, _) =>
                Task.FromResult<IInvocationResult>(
                    new CustomExceptionInvocationResult(action, ex));
        }
    }
}