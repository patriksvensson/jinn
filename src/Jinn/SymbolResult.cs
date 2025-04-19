namespace Jinn;

public abstract class SymbolResult
{
    private readonly List<Token> _tokens;

    public Symbol Symbol { get; }
    public IReadOnlyList<Token> Tokens => _tokens;

    protected SymbolResult(Symbol symbol)
    {
        _tokens = [];
        Symbol = symbol;
    }

    internal void AddToken(Token token)
    {
        _tokens.Add(token);
    }
}

public sealed class CommandResult : SymbolResult
{
    public CommandResult(CommandSymbol symbol)
        : base(symbol)
    {
    }
}

public sealed class OptionResult : SymbolResult
{
    public Token Identifier { get; }

    public OptionResult(OptionSymbol symbol, Token identifier)
        : base(symbol)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }
}

public sealed class ArgumentResult : SymbolResult
{
    public ArgumentResult(ArgumentSymbol symbol)
        : base(symbol)
    {
    }
}