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
        return Parser.Parse(Configuration, args.ToList(), this);
    }

    public async Task<int> Invoke(IEnumerable<string> args)
    {
        var result = Parse(args);
        var pipeline = new InvocationPipeline(result);
        return await pipeline.Invoke();
    }
}