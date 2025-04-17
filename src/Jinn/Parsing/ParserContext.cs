namespace Jinn;

internal sealed class ParserContext
{
    private readonly TokenizeResult _tokenizationResult;
    private readonly TokenReader _reader;

    public SymbolResultTree Tree { get; }
    public CommandResult RootCommand { get; }
    public CommandResult CurrentCommand { get; private set; }
    public int CurrentArgumentCount { get; set; }
    public int CurrentArgumentIndex { get; set; }

    public Token? PreviousToken => _reader.Previous();
    public Token CurrentToken => _reader.Current ?? throw new InvalidOperationException("Reached end of token stream");

    [MemberNotNullWhen(false, nameof(CurrentToken))]
    public bool IsAtEnd => _reader.IsAtEnd;

    public ParserContext(TokenizeResult result)
    {
        _tokenizationResult = result;
        _reader = new TokenReader(result.Tokens);

        Tree = new SymbolResultTree(result.Root);
        RootCommand = CurrentCommand = new CommandResult(result.Root, null, Tree, null);
    }

    public void ConsumeToken()
    {
        _reader.Consume();
    }

    public T Expect<T>()
        where T : Symbol
    {
        if (CurrentToken.Symbol is not T command)
        {
            throw new InvalidOperationException(
                $"Expected symbol of type '{typeof(T).Name}'");
        }

        return command;
    }

    public bool IsMatch(TokenType type)
    {
        return !IsAtEnd && CurrentToken.TokenType == type;
    }

    public void AddCommand(CommandSymbol command)
    {
        CurrentCommand = new CommandResult(command, CurrentToken, Tree, CurrentCommand);
        Tree.Add(command, CurrentCommand);

        CurrentArgumentCount = 0;
        CurrentArgumentIndex = 0;
    }

    public void AddCurrentTokenToUnmatched()
    {
        if (CurrentToken.TokenType != TokenType.DoubleDash)
        {
            Tree.Unmatched.Add(CurrentToken);
        }
    }

    public ParseResult CreateResult()
    {
        return new ParseResult
        {
            Root = RootCommand,
            Command = CurrentCommand,
            Tokens = _tokenizationResult.Tokens,
            UnmatchedTokens = Tree.Unmatched,
            Arguments = _tokenizationResult.Arguments,
        };
    }
}