namespace Jinn;

internal sealed class ParserContext
{
    private readonly TokenizeResult _tokenizationResult;
    private readonly TokenReader _reader;

    public CommandSymbol RootCommand { get; }
    public CommandSymbol CurrentCommand { get; private set; }
    public int CurrentArgumentCount { get; set; }
    public int CurrentArgumentIndex { get; set; }
    public List<Token> Unmatched { get; } = [];

    public Token? PreviousToken => _reader.Previous();
    public Token CurrentToken => _reader.Current ?? throw new InvalidOperationException("Reached end of token stream");

    [MemberNotNullWhen(false, nameof(CurrentToken))]
    public bool IsAtEnd => _reader.IsAtEnd;

    public ParserContext(TokenizeResult result)
    {
        _tokenizationResult = result;
        _reader = new TokenReader(result.Tokens);

        RootCommand = CurrentCommand = result.Root;
    }

    public void ConsumeToken()
    {
        _reader.Consume();
    }

    public T ExpectCurrentSymbol<T>()
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
        command.Parent = CurrentCommand;
        CurrentCommand = command;

        CurrentArgumentCount = 0;
        CurrentArgumentIndex = 0;
    }

    public void AddCurrentTokenToUnmatched()
    {
        if (CurrentToken.TokenType != TokenType.DoubleDash)
        {
            Unmatched.Add(CurrentToken);
        }
    }

    public ParseResult CreateResult()
    {
        // Perform validation
        var errors = ParseValidator.Validate(CurrentCommand);

        return new ParseResult
        {
            Root = RootCommand,
            Command = CurrentCommand,
            Tokens = _tokenizationResult.Tokens,
            UnmatchedTokens = Unmatched,
            Arguments = _tokenizationResult.Arguments,
            Errors = errors,
        };
    }
}