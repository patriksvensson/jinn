using System.Diagnostics.CodeAnalysis;

namespace Jinn;

internal sealed class TokenReader
{
    private readonly List<Token> _tokens;
    private int _index;

    public Token? Current => _tokens[_index];

    [MemberNotNullWhen(false, nameof(Current))]
    public bool IsAtEnd { get; private set; }

    public int Position => _index;

    public TokenReader(List<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        _index = 0;

        IsAtEnd = _index >= _tokens.Count;
    }

    public void Consume()
    {
        if (_index < _tokens.Count - 1)
        {
            _index++;
        }
        else
        {
            IsAtEnd = true;
        }
    }

    public Token? Read()
    {
        if (_index > _tokens.Count - 1)
        {
            return null;
        }

        var current = _tokens[_index];

        if (_index >= _tokens.Count - 1)
        {
            IsAtEnd = true;
        }
        else
        {
            _index++;
        }

        return current;
    }

    public Token? Previous()
    {
        if (_index < 1)
        {
            return null;
        }

        return _tokens[_index - 1];
    }
}