namespace Jinn;

internal abstract class SymbolResult
{
    private readonly List<Token> _tokens;

    public SymbolResultTree Tree { get; }
    public SymbolResult? Parent { get; }
    public IReadOnlyList<Token> Tokens => _tokens;

    protected SymbolResult(SymbolResultTree tree, SymbolResult? parent)
    {
        _tokens = new List<Token>();

        Tree = tree ?? throw new ArgumentNullException(nameof(tree));
        Parent = parent;
    }

    public void AddToken(Token token)
    {
        _tokens.Add(token);
    }
}

internal sealed class CommandSymbolResult : SymbolResult
{
    public Command Symbol { get; }
    public Token IdentifierToken { get; }

    public CommandSymbolResult(
        Command command,
        Token token,
        SymbolResultTree tree,
        SymbolResult? parent = null)
        : base(tree, parent)
    {
        Symbol = command;
        IdentifierToken = token;
    }
}

internal sealed class OptionSymbolResult : SymbolResult
{
    public Option Symbol { get; }
    public Token IdentifierToken { get; }

    public OptionSymbolResult(
        Option option,
        Token token,
        SymbolResultTree tree,
        SymbolResult? parent = null)
        : base(tree, parent)
    {
        Symbol = option;
        IdentifierToken = token;
    }
}

internal sealed class ArgumentSymbolResult : SymbolResult
{
    public Argument Symbol { get; }

    public ArgumentSymbolResult(
        Argument argument,
        SymbolResultTree tree,
        SymbolResult? parent = null)
        : base(tree, parent)
    {
        Symbol = argument;
    }
}

internal sealed class SymbolResultTree : Dictionary<Symbol, SymbolResult>
{
    public Command Root { get; }

    public SymbolResultTree(Command root)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));
    }
}