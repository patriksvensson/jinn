namespace Jinn;

internal sealed class TokenizerContext
{
    private readonly Dictionary<string, Symbol> _symbols;
    private readonly IReadOnlyList<string> _args;
    private readonly List<Token> _tokens;
    private readonly bool _hasSyntheticRoot;
    private int _index;

    public int Position { get; private set; }
    public bool HasEncounteredDoubleDash { get; private set; }

    public TokenizerContext(Command root, IEnumerable<string> args)
    {
        _symbols = new Dictionary<string, Symbol>(StringComparer.Ordinal);
        _args = NormalizeArguments(args, root, out _hasSyntheticRoot);
        _tokens = [];
        _index = -1;

        Position = 0;

        SetCurrentCommand(root);
    }

    public bool Read([NotNullWhen(true)] out string? arg)
    {
        if (_index >= _args.Count - 1)
        {
            arg = null;
            return false;
        }

        if (_index != -1)
        {
            var isAtSyntheticRoot = _index == 0 && _hasSyntheticRoot;
            if (!isAtSyntheticRoot)
            {
                Position += _args[_index].Length + 1;
            }
        }

        _index++;
        arg = _args[_index];
        return true;
    }

    public void AddToken(TokenKind kind, Symbol? symbol, string lexeme, TextSpan? span = null)
    {
        if (kind == TokenKind.DoubleDash)
        {
            HasEncounteredDoubleDash = true;
        }

        var isAtSyntheticRoot = _index == 0 && _hasSyntheticRoot;
        _tokens.Add(
            new Token(
                kind, symbol,
                isAtSyntheticRoot ? null : span ?? new TextSpan(Position, lexeme.Length),
                lexeme));
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

    public bool TryGetSymbol(string name, [NotNullWhen(true)] out Symbol? result)
    {
        return _symbols.TryGetValue(name, out result);
    }

    public void SetCurrentCommand(Command current)
    {
        _symbols.Clear();

        if (current is RootCommand root)
        {
            _symbols[root.Name] = root;
        }

        foreach (var command in current.Commands)
        {
            if (!_symbols.TryAdd(command.Name, command))
            {
                throw new InvalidOperationException(
                    $"The command name '{command.Name}' is in use by more than one command");
            }

            foreach (var alias in command.Aliases)
            {
                if (!_symbols.TryAdd(alias, command))
                {
                    throw new InvalidOperationException(
                        $"The command name '{alias}' is in use by more than one command");
                }
            }
        }

        foreach (var option in current.Options)
        {
            foreach (var alias in option.NameAndAliases)
            {
                if (!_symbols.TryAdd(alias, option))
                {
                    throw new InvalidOperationException(
                        $"The option name '{alias}' is in use by more than one option");
                }
            }
        }
    }

    public Token? GetLastToken()
    {
        return _tokens.Count == 0 ? null : _tokens[^1];
    }

    public List<Token> GetResult()
    {
        return _tokens;
    }

    private static List<string> NormalizeArguments(
        IEnumerable<string> args, Command command, out bool addedExecutable)
    {
        addedExecutable = false;

        var result = new List<string>(args);

        if (result.Count > 0)
        {
            if (result[0] == command.Name)
            {
                return result;
            }
        }

        if (command is RootCommand)
        {
            addedExecutable = true;
            result.Insert(0, command.Name);
        }

        return result;
    }
}