﻿namespace Jinn;

internal sealed class TokenizerContext
{
    private readonly Dictionary<string, Symbol> _symbols;
    private readonly IReadOnlyList<string> _args;
    private readonly List<Token> _tokens;
    private readonly bool _hasSyntheticRoot;
    private int _index;
    private int _position;
    private bool _hasEncounteredDoubleDash;

    public int Position => _position;
    public bool HasEncounteredDoubleDash => _hasEncounteredDoubleDash;
    public bool HasSyntheticRoot => _hasSyntheticRoot;

    public TokenizerContext(Command root, IEnumerable<string> args)
    {
        _symbols = new Dictionary<string, Symbol>(StringComparer.Ordinal);
        _args = NormalizeArguments(args, root, out _hasSyntheticRoot);
        _tokens = [];
        _index = -1;
        _position = 0;

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
                _position += _args[_index].Length + 1;
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
            _hasEncounteredDoubleDash = true;
        }

        var isAtSyntheticRoot = _index == 0 && _hasSyntheticRoot;
        _tokens.Add(
            new Token(
                kind, symbol,
                isAtSyntheticRoot ? null : span ?? new TextSpan(Position, lexeme.Length),
                lexeme));
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

        if (current is RootCommand root)
        {
            _symbols[root.Name] = root;
        }

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