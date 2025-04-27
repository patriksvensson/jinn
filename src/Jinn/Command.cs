namespace Jinn;

[PublicAPI]
public class Command : Symbol
{
    private readonly List<Command> _commands = [];
    private readonly List<Argument> _arguments = [];
    private readonly List<Option> _options = [];

    public string Name { get; }

    public IReadOnlyList<Command> Commands => _commands;
    public IReadOnlyList<Argument> Arguments => _arguments;
    public IReadOnlyList<Option> Options => _options;

    public bool HasArguments => Arguments.Count > 0;
    internal Func<InvocationContext, Task>? Handler { get; private set; }

    public Command(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void AddCommand(Command command)
    {
        _commands.Add(command);
    }

    public void AddArgument(Argument argument)
    {
        _arguments.Add(argument);
    }

    public void AddOption(Option option)
    {
        _options.Add(option);
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
        foreach (var command in commands)
        {
            AddCommand(command);
        }
    }

    public RootCommand(Configuration configuration, params Command[] commands)
        : base(configuration.ExecutableName)
    {
        Configuration = configuration;

        foreach (var command in commands)
        {
            AddCommand(command);
        }
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        return Parser.Parse(Configuration, args, this);
    }

    public async Task<int> Invoke(IEnumerable<string> args)
    {
        var result = Parse(args);
        var pipeline = new InvocationPipeline(result);
        return await pipeline.Invoke();
    }
}