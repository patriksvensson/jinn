namespace Jinn;

internal sealed class ParserContext
{
    private CommandSymbolResult? _innerMostCommand;

    public Command Root { get; }
    public SymbolResultTree Tree { get; }
    public TokenReader Reader { get; }

    public ParserContext(Command root, List<Token> tokens)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));
        Tree = new SymbolResultTree(root);
        Reader = new TokenReader(tokens);
    }

    public CommandSymbolResult AddCommand(Command command)
    {
        _innerMostCommand = new CommandSymbolResult(command, Reader.Current!, Tree, _innerMostCommand);
        Tree.Add(command, _innerMostCommand);
        return _innerMostCommand;
    }

    public OptionSymbolResult AddOption(Option option)
    {
        var result = new OptionSymbolResult(option, Reader.Current!, Tree, _innerMostCommand);
        Tree.Add(option, result);
        return result;
    }

    public ArgumentSymbolResult AddOptionArgument(OptionSymbolResult option, Argument argument)
    {
        var argumentResult = new ArgumentSymbolResult(argument, Tree, option);
        Tree.Add(argument, argumentResult);
        return argumentResult;
    }
}