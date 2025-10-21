namespace Jinn;

[PublicAPI]
public class Command : Symbol
{
    public string Name { get; }

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

    public bool HasArguments => Arguments.Count > 0;
    internal Func<InvocationContext, Task>? Handler { get; private set; }

    public Command(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        // Add default options
        Options.Add(new HelpOption());
    }

    public void SetHandler(Action<InvocationContext> handler)
    {
        Handler = (ctx) =>
        {
            handler(ctx);
            return Task.CompletedTask;
        };
    }

    public void SetHandler(Func<InvocationContext, Task> handler)
    {
        Handler = handler;
    }
}

[PublicAPI]
public sealed class RootCommand : Command
{
    public Configuration Configuration { get; }

    public RootCommand(params Command[] commands)
        : this(new Configuration())
    {
        Commands.AddRange(commands);
    }

    public RootCommand(Configuration configuration, params Command[] commands)
        : base(configuration.ExecutableName)
    {
        Configuration = configuration;
        Commands.AddRange(commands);
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        return Parser.Parse(Configuration, args, this);
    }

    public async Task<int> Invoke(
        IEnumerable<string> args,
        Action<InvocationContext>? initialize = null)
    {
        var result = Parse(args);
        var pipeline = new InvocationPipeline(result);
        return await pipeline.Invoke(initialize);
    }
}

[PublicAPI]
public static class CommandExtensions
{
    public static Command AddCommand(this Command source, Command command)
    {
        source.Commands.Add(command);
        return command;
    }

    public static Argument AddArgument(this Command source, Argument argument)
    {
        source.Arguments.Add(argument);
        return argument;
    }

    public static Argument<T> AddArgument<T>(this Command source, Argument<T> argument)
    {
        source.Arguments.Add(argument);
        return argument;
    }

    public static Option AddOption(this Command source, Option option)
    {
        source.Options.Add(option);
        return option;
    }

    public static Option<T> AddOption<T>(this Command source, Option<T> option)
    {
        source.Options.Add(option);
        return option;
    }
}