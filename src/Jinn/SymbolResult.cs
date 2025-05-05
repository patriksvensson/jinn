namespace Jinn;

/// <summary>
/// Represents a parsed result for a symbol.
/// </summary>
[PublicAPI]
public abstract class SymbolResult
{
    private readonly List<SymbolResult> _children = [];
    private readonly List<Token> _tokens = [];
    private Diagnostics? _diagnostics;

    public Symbol Symbol { get; }
    public SymbolResult? Parent { get; }
    public IReadOnlyList<SymbolResult> Children => _children;
    public IReadOnlyList<Token> Tokens => _tokens;
    public IReadOnlyList<Diagnostic>? Diagnostics => _diagnostics;

    protected SymbolResult(Symbol symbol, SymbolResult? parent)
    {
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        Parent = parent;
    }

    public void AddChild(SymbolResult result)
    {
        _children.Add(result);
    }

    public void AddToken(Token token)
    {
        _tokens.Add(token);
    }

    internal void AddDiagnostic(Diagnostic diagnostic)
    {
        _diagnostics ??= new Diagnostics();
        _diagnostics.Add(diagnostic);
    }

    internal TResult? FindImmediateResult<TResult>(Symbol symbol)
        where TResult : SymbolResult
    {
        foreach (var child in Children)
        {
            if (child is TResult argumentResult)
            {
                if (argumentResult.Symbol == symbol)
                {
                    return argumentResult;
                }
            }
        }

        return null;
    }
}

[PublicAPI]
public abstract class CommandResult : SymbolResult
{
    public Command Command { get; }

    protected CommandResult(Command command, SymbolResult? parent)
        : base(command, parent)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
    }
}

[PublicAPI]
public sealed class RootCommandResult : CommandResult
{
    public RootCommandResult(Command command)
        : base(command, null)
    {
    }
}

[PublicAPI]
public sealed class SubCommandResult : CommandResult
{
    public Token Token { get; }

    public SubCommandResult(Command command, Token token, SymbolResult? parent)
        : base(command, parent)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}

[PublicAPI]
public sealed class ArgumentResult : SymbolResult
{
    public Argument Argument { get; }
    public Arity Arity => Argument.Arity;

    public ArgumentResult(Argument argument, SymbolResult? parent)
        : base(argument, parent)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
    }
}

[PublicAPI]
public sealed class OptionResult : SymbolResult
{
    public Option Option { get; }
    public Token Token { get; }

    public Arity Arity => Option.Arity;

    public OptionResult(Option option, Token token, CommandResult? parent)
        : base(option, parent)
    {
        Option = option ?? throw new ArgumentNullException(nameof(option));
        Token = token ?? throw new ArgumentNullException(nameof(token));
    }
}