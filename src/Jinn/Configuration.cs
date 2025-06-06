namespace Jinn;

[PublicAPI]
public sealed class Configuration
{
    public string ExecutableName { get; init; } = GetExecutableName();
    public bool HelpEnabled { get; set; } = true;

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

public static class ConfigurationExtensions
{
    public static void SetHelpProvider(this Configuration configuration, Action<InvocationContext> action)
    {
        configuration.HelpProvider = _ =>
        {
            return Task.FromResult<IInvocationResult>(
                new CustomHelpInvocationResult(action));
        };
    }

    public static void SetParseErrorHandler(this Configuration configuration, Action<InvocationContext> action)
    {
        configuration.ParseErrorHandler = _ =>
        {
            return Task.FromResult<IInvocationResult>(
                new CustomParseErrorInvocationResult(action));
        };
    }
}