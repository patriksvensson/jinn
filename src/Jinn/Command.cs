namespace Jinn;

[PublicAPI]
public class Command
{
    public string Name { get; }
    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

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

    internal CommandSymbol CreateSymbol()
    {
        return new CommandSymbol(this);
    }
}

[PublicAPI]
public sealed class RootCommand : Command
{
    private readonly List<InvocationMiddleware> _middlewares = [];

    public Configuration Configuration { get; set; } = new();

    public RootCommand()
        : base("<root>")
    {
    }

    public RootCommand(params Command[] commands)
        : base("<root>")
    {
        Commands.AddRange(commands);
    }

    public void AddMiddleware(InvocationMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        return Parser.Parse(args, this);
    }

    public async Task<int> Invoke(IEnumerable<string> args)
    {
        var result = Parse(args);
        var pipeline = new InvocationPipeline(result, _middlewares, Configuration);
        return await pipeline.Invoke();
    }
}