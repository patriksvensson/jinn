namespace Jinn;

public sealed class CommandContainer : Symbol
{
    private readonly List<Command> _commands;

    public CommandContainer(IEnumerable<Command> commands)
    {
        _commands = new List<Command>(commands);
    }

    public override IEnumerable<Symbol> GetOwnedSymbols()
    {
        return _commands;
    }

    public override IEnumerable<string> GetNames()
    {
        yield break;
    }
}