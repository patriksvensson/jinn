namespace Jinn;

internal sealed class TokenizerContext
{
    private readonly Dictionary<string, Symbol> _known;
    private readonly string[] _args;
    private readonly List<Token> _tokens;
    private int _index;

    public bool ReachedEnd => Index >= _args.Length - 1;
    public int Index => _index;
    public IReadOnlyList<Token> Tokens => _tokens;

    public TokenizerContext(CommandSymbol root, IEnumerable<string> args)
    {
        _known = new Dictionary<string, Symbol>(StringComparer.Ordinal);
        _args = args.ToArray();
        _tokens = [];
        _index = -1;

        SetCurrentCommand(root);
    }

    public bool Read([NotNullWhen(true)] out string? arg)
    {
        if (ReachedEnd)
        {
            arg = null;
            return false;
        }

        _index++;
        arg = _args[_index];
        return true;
    }

    public void AddToken(TokenType type, Symbol? symbol, string name)
    {
        _tokens.Add(new Token(type, symbol, Index, name));
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