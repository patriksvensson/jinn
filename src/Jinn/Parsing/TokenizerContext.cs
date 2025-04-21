namespace Jinn;

internal sealed class TokenizerContext
{
    private readonly Command _root;
    private readonly Dictionary<string, Symbol> _symbols;
    private readonly string[] _args;
    private readonly List<Token> _tokens;
    private int _index;
    private int _position;
    private bool _hasEncounteredDoubleDash;

    public IReadOnlyList<Token> Tokens => _tokens;
    public int Position => _position;
    public bool HasEncounteredDoubleDash => _hasEncounteredDoubleDash;

    public TokenizerContext(Command root, IEnumerable<string> args)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
        _symbols = new Dictionary<string, Symbol>(StringComparer.Ordinal);
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

    public void AddToken(TokenKind kind, Symbol? symbol, string text, TextSpan? span = null)
    {
        if (kind == TokenKind.DoubleDash)
        {
            _hasEncounteredDoubleDash = true;
        }

        _tokens.Add(new Token(kind, symbol, span ?? new TextSpan(Position, text.Length), text));
    }

    public bool TryGetSymbol(string name, [NotNullWhen(true)] out Symbol? result)
    {
        return _symbols.TryGetValue(name, out result);
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

    public void SetCurrentCommand(Command current)
    {
        _symbols.Clear();

        foreach (var command in current.Commands)
        {
            _symbols[command.Name] = command;
        }

        foreach (var option in current.Options)
        {
            foreach (var alias in option.Aliases)
            {
                _symbols[alias] = option;
            }
        }
    }
}