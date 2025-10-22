namespace Jinn;

[PublicAPI]
public class Command : Symbol
{
    public string Name { get; }
    public HashSet<string> Aliases { get; init; } = [];

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

    public bool HasArguments => Arguments.Count > 0;
    public Func<InvocationContext, Task>? Handler { get; set; }

    public Command(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        // Add default options
        Options.Add(new HelpOption());
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
    extension(Command command)
    {
        public void SetHandler(Action<InvocationContext> handler)
        {
            command.Handler = (ctx) =>
            {
                handler(ctx);
                return Task.CompletedTask;
            };
        }

        public void SetHandler(Func<InvocationContext, Task> handler)
        {
            command.Handler = handler;
        }
    }
}