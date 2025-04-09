namespace Jinn;

public sealed class Command : Symbol
{
    public string Name { get; }

    public List<Command> Commands { get; init; } = [];
    public List<Argument> Arguments { get; init; } = [];
    public List<Option> Options { get; init; } = [];

    public Command(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
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