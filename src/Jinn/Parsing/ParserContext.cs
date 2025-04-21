namespace Jinn;

internal sealed class ParserContext
{
    private readonly TokenReader _reader;

    public Command RootCommand { get; }
    public Command CurrentCommand { get; private set; }
    public int CurrentArgumentCount { get; set; }
    public int CurrentArgumentIndex { get; set; }
    public List<Token> Unmatched { get; } = [];

    public Token CurrentToken =>
        _reader.Current ?? throw new InvalidOperationException("Reached end of token stream");

    [MemberNotNullWhen(false, nameof(CurrentToken))]
    public bool IsAtEnd => _reader.IsAtEnd;

    public ParserContext(Command root, IReadOnlyList<Token> result)
    {
        _reader = new TokenReader(result);
        CurrentCommand = RootCommand = root;
    }

    public void ConsumeToken()
    {
        _reader.Consume();
    }

    public T ExpectCurrentSymbol<T>()
        where T : Symbol
    {
        if (CurrentToken.Symbol is not T symbol)
        {
            throw new InvalidOperationException(
                $"Expected symbol of type '{typeof(T).Name}'");
        }

        return symbol;
    }

    public bool IsMatch(TokenKind type)
    {
        return !IsAtEnd && CurrentToken.Kind == type;
    }

    public void SetCurrentCommand(Command command)
    {
        CurrentCommand = command;
        CurrentArgumentCount = 0;
        CurrentArgumentIndex = 0;
    }

    public void AddCurrentTokenToUnmatched()
    {
        if (CurrentToken.Kind != TokenKind.DoubleDash)
        {
            Unmatched.Add(CurrentToken);
        }
    }

    // public ParseResult CreateResult()
    // {
    //     // Perform validation
    //     var errors = ParseValidator.Validate(CurrentCommand);
    //
    //     return new ParseResult
    //     {
    //         Root = RootCommand,
    //         Command = CurrentCommand,
    //         Tokens = _tokenizationResult.Tokens,
    //         UnmatchedTokens = Unmatched,
    //         Arguments = _tokenizationResult.Arguments,
    //         Errors = errors,
    //     };
    // }
}