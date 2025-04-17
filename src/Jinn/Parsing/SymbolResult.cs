namespace Jinn;

public abstract class SymbolResult
{
    private readonly SymbolResultTree _tree;
    private readonly List<Token> _tokens;

    public SymbolResult? Parent { get; }
    public IReadOnlyList<Token> Tokens => _tokens;

    private protected SymbolResult(SymbolResultTree tree, SymbolResult? parent)
    {
        _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        _tokens = [];

        Parent = parent;
    }

    internal void AddToken(Token token)
    {
        _tokens.Add(token);
    }
}

public sealed class CommandResult : SymbolResult
{
    public CommandSymbol Command { get; }
    public Token? Identifier { get; }

    internal CommandResult(
        CommandSymbol command,
        Token? identifier,
        SymbolResultTree tree,
        SymbolResult? parent = null)
        : base(tree, parent)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Identifier = identifier;
    }
}

public sealed class ArgumentResult : SymbolResult
{
    public ArgumentSymbol Argument { get; }

    internal ArgumentResult(
        ArgumentSymbol argument,
        SymbolResultTree tree,
        SymbolResult? parent = null)
        : base(tree, parent)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
    }
}

public sealed class OptionResult : SymbolResult
{
    public OptionSymbol Option { get; }
    public Token Identifier { get; }

    internal OptionResult(
        OptionSymbol option,
        Token identifier,
        SymbolResultTree tree,
        CommandResult? parent = null)
        : base(tree, parent)
    {
        Option = option ?? throw new ArgumentNullException(nameof(option));
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }
}