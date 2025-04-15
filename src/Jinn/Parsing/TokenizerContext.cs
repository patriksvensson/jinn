namespace Jinn;

internal sealed class TokenizerContext
{
    private readonly Dictionary<string, Symbol> _known;
    private readonly string[] _args;
    private readonly List<Token> _tokens;
    private int _index;
    private int _position;

    public IReadOnlyList<Token> Tokens => _tokens;
    public int Position => _position;

    public TokenizerContext(CommandSymbol root, IEnumerable<string> args)
    {
        _known = new Dictionary<string, Symbol>(StringComparer.Ordinal);
        _args = args.ToArray();
        _tokens = [];
        _index = -1;
        _position = 0;

        SetCurrentCommand(root);
    }

    public bool Read([NotNullWhen(true)] out string? arg)
    {
        if (_index >= _args.Length - 1)
        {
            arg = null;
            return false;
        }

        if (_index != -1)
        {
            _position += _args[_index].Length + 1;
        }

        _index++;
        arg = _args[_index];
        return true;
    }

    public void AddToken(TokenType type, Symbol? symbol, string text, TextSpan? span = null)
    {
        _tokens.Add(new Token(type, symbol, span ?? new TextSpan(Position, text.Length), text));
    }

    public bool TryGetSymbol(string name, [NotNullWhen(true)] out Symbol? result)
    {
        if (_known.TryGetValue(name, out result))
        {
            return true;
        }

        return false;
    }

    public bool TryGetSymbol<T>(string name, [NotNullWhen(true)] out T? result)
        where T : Symbol
    {
        if (TryGetSymbol(name, out var symbol))
        {
            result = symbol as T;
            return result != null;
        }

        result = null;
        return false;
    }

    public void SetCurrentCommand(CommandSymbol current)
    {
        _known.Clear();

        foreach (var command in current.Commands)
        {
            _known[command.Name] = command;
        }

        foreach (var option in current.Options)
        {
            foreach (var alias in option.Aliases)
            {
                _known[alias] = option;
            }
        }
    }
}