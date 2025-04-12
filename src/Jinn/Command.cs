namespace Jinn;

public class Command : Symbol
{
    internal const string RootCommandName = "<root>";

    public string Name { get; }

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

    public bool IsRoot => Name.Equals(RootCommandName, StringComparison.Ordinal);

    public Command(string? name = null)
    {
        Name = name ?? RootCommandName;
    }

    public override IEnumerable<Symbol> GetOwnedSymbols()
    {
        foreach (var command in Commands)
        {
            yield return command;
        }

        foreach (var argument in Arguments)
        {
            yield return argument;
        }

        foreach (var option in Options)
        {
            yield return option;
        }
    }

    public override IEnumerable<string> GetNames()
    {
        yield return Name;
    }
}