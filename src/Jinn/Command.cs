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

    public Command(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        return Parser.Parse(args, this);
    }

    internal CommandSymbol CreateSymbol()
    {
        return new CommandSymbol
        {
            Name = Name,
            Description = Description,
            Hidden = Hidden,
            Commands = Commands.ConvertAll(x => x.CreateSymbol()),
            Arguments = Arguments.ConvertAll(x => x.CreateSymbol()),
            Options = Options.ConvertAll(x => x.CreateSymbol()),
        };
    }
}

[PublicAPI]
public sealed class RootCommand : Command
{
    public RootCommand()
        : base("<root>")
    {
    }

    public RootCommand(params Command[] commands)
        : base("<root>")
    {
        Commands.AddRange(commands);
    }
}