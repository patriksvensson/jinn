namespace Jinn;

public sealed class Command
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

public sealed class RootCommand
{
    public string Name { get; }
    public string? Description { get; set; }
    public bool Hidden { get; set; }

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

    public RootCommand()
    {
        Name = "<root>";
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