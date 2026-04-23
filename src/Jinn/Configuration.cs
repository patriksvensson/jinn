using System.Reflection;

namespace Jinn;

[PublicAPI]
public sealed class Configuration
{
    public string ExecutableName { get; init; } = Executable.GetName();
    public string ExecutableVersion { get; init; } = Executable.GetVersion();

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

    // Based on https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine/RootCommand.cs
    private static class Executable
    {
        private static Assembly? _assembly;
        private static string? _executableName;
        private static string? _executableVersion;

        public static string GetName()
        {
            return _executableName ??= GetVersionFromArguments();

            static string GetVersionFromArguments()
            {
                var path = Environment.GetCommandLineArgs().First();
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        public static string GetVersion()
        {
            return _executableVersion ??= GetVersionFromEntryAssembly();

            static string GetVersionFromEntryAssembly()
            {
                _assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var assemblyVersionAttribute = _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (assemblyVersionAttribute is null)
                {
                    return _assembly.GetName().Version?.ToString() ?? "1.0";
                }

                return assemblyVersionAttribute.InformationalVersion;
            }
        }
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