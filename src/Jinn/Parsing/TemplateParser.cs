using System.Globalization;
using System.Text;
using Jinn.Exceptions;

namespace Jinn;

internal static class TemplateParser
{
    public static OptionName[] ParseOption(string template)
    {
        return ParseOptionNames(template).ToArray();
    }

    private static IEnumerable<OptionName> ParseOptionNames(string template)
    {
        var longCount = 0;
        var shortCount = 0;

        foreach (var token in TemplateTokenizer.Tokenize(template))
        {
            if (token.TokenKind is TemplateToken.Kind.LongOption or TemplateToken.Kind.ShortOption)
            {
                if (string.IsNullOrWhiteSpace(token.Value))
                {
                    throw JinnTemplateExceptionFactory.OptionsMustHaveName(template, token);
                }

                if (char.IsDigit(token.Value[0]))
                {
                    throw JinnTemplateExceptionFactory.OptionNamesCannotStartWithDigit(template, token);
                }

                foreach (var character in token.Value)
                {
                    if (!char.IsLetterOrDigit(character) && character != '-' && character != '_' && character != '?')
                    {
                        throw JinnTemplateExceptionFactory.InvalidCharacterInOptionName(template, token, character);
                    }
                }
            }

            if (token.TokenKind == TemplateToken.Kind.LongOption)
            {
                if (token.Value.Length == 1)
                {
                    throw JinnTemplateExceptionFactory.LongOptionMustHaveMoreThanOneCharacter(template, token);
                }

                longCount++;
                yield return OptionName.Long(token.Lexeme);
            }

            if (token.TokenKind == TemplateToken.Kind.ShortOption)
            {
                if (token.Value.Length > 1)
                {
                    throw JinnTemplateExceptionFactory.ShortOptionMustOnlyBeOneCharacter(template, token);
                }

                shortCount++;
                yield return OptionName.Short(token.Lexeme);
            }
        }

        if (longCount == 0 && shortCount == 0)
        {
            throw JinnTemplateExceptionFactory.MissingLongAndShortName(template, null);
        }
    }
}

file static class JinnTemplateExceptionFactory
{
    internal static JinnTemplateException MissingLongAndShortName(string template, TemplateToken? token)
    {
        return new JinnTemplateException(
            "No long or short name for option has been specified")
        {
            Template = template,
            Position = token?.Position,
            Lexeme = token?.Lexeme,
        };
    }

    public static JinnTemplateException ShortOptionMustOnlyBeOneCharacter(string template, TemplateToken token)
    {
        // Rewrite the token to point to the option name instead of the whole option.
        token = new TemplateToken(token.TokenKind, token.Position + 1, token.Value, token.Value);

        return new JinnTemplateException(
            "Short option names can not be longer than one character",
            "Invalid option name")
        {
            Template = template,
            Position = token.Position,
            Lexeme = token.Lexeme,
        };
    }

    internal static JinnTemplateException LongOptionMustHaveMoreThanOneCharacter(string template, TemplateToken token)
    {
        // Rewrite the token to point to the option name instead of the whole option.
        token = new TemplateToken(token.TokenKind, token.Position + 2, token.Value, token.Value);

        return new JinnTemplateException(
            "Long option names must consist of more than one character",
            "Invalid option name")
        {
            Template = template,
            Position = token.Position,
            Lexeme = token.Lexeme,
        };
    }

    public static JinnTemplateException InvalidCharacterInOptionName(string template, TemplateToken token, char character)
    {
        // Rewrite the token to point to the invalid character instead of the whole value.
        var position = (token.TokenKind == TemplateToken.Kind.ShortOption
            ? token.Position + 1
            : token.Position + 2) + token.Value.OrdinalIndexOf(character);

        token = new TemplateToken(
            token.TokenKind, position,
            token.Value, character.ToString(CultureInfo.InvariantCulture));

        return new JinnTemplateException(
            $"Encountered invalid character '{character}' in option name",
            "Invalid character")
        {
            Template = template,
            Position = token.Position,
            Lexeme = token.Lexeme,
        };
    }

    public static JinnTemplateException OptionNamesCannotStartWithDigit(string template, TemplateToken token)
    {
        // Rewrite the token to point to the option name instead of the whole string.
        token = new TemplateToken(
            token.TokenKind,
            token.TokenKind == TemplateToken.Kind.ShortOption ? token.Position + 1 : token.Position + 2,
            token.Value, token.Value);

        return new JinnTemplateException(
            "Option names cannot start with a digit",
            "Invalid option name")
        {
            Template = template,
            Position = token.Position,
            Lexeme = token.Lexeme,
        };
    }

    public static JinnTemplateException OptionsMustHaveName(string template, TemplateToken token)
    {
        return new JinnTemplateException(
            "Options without name are not allowed",
            "Missing option name")
        {
            Template = template,
            Position = token.Position,
            Lexeme = token.Lexeme,
        };
    }
}

file static class TemplateTokenizer
{
    public static IEnumerable<TemplateToken> Tokenize(string template)
    {
        using var buffer = new TextBuffer(template);

        while (!buffer.ReachedEnd)
        {
            EatWhitespace(buffer);

            if (!buffer.TryPeek(out var character))
            {
                break;
            }

            if (character == '-')
            {
                yield return ReadOption(buffer);
            }
            else if (character == '|')
            {
                buffer.Consume();
            }
            else if (char.IsAsciiLetterOrDigit(character))
            {
                yield return ReadName(buffer);
            }
            else
            {
                throw new JinnTemplateException(
                    $"Unexpected character '{character}'")
                {
                    Position = buffer.Position,
                    Template = template,
                };
            }
        }
    }

    private static void EatWhitespace(TextBuffer buffer)
    {
        while (!buffer.ReachedEnd)
        {
            var character = buffer.Peek();
            if (!char.IsWhiteSpace(character))
            {
                break;
            }

            buffer.Read();
        }
    }

    private static TemplateToken ReadOption(TextBuffer buffer)
    {
        var position = buffer.Position;

        buffer.Consume('-');
        if (!buffer.IsNext('-'))
        {
            var shortName = ReadName(buffer).Lexeme;
            return new TemplateToken(
                TemplateToken.Kind.ShortOption, position,
                shortName, $"-{shortName}");
        }

        buffer.Consume('-');
        var longValue = ReadName(buffer).Lexeme;
        return new TemplateToken(
            TemplateToken.Kind.LongOption, position,
            longValue, $"--{longValue}");
    }

    private static TemplateToken ReadName(TextBuffer buffer)
    {
        var position = buffer.Position;

        var builder = new StringBuilder();
        while (!buffer.ReachedEnd)
        {
            var character = buffer.Peek();
            if (char.IsWhiteSpace(character) || character == '|')
            {
                break;
            }

            builder.Append(buffer.Read());
        }

        var lexeme = builder.ToString();
        return new TemplateToken(
            TemplateToken.Kind.Name, position,
            lexeme, lexeme);
    }
}

file sealed class TemplateToken
{
    public Kind TokenKind { get; }
    public int Position { get; }
    public string Value { get; }
    public string Lexeme { get; }

    public TemplateToken(Kind kind, int position, string value, string lexeme)
    {
        TokenKind = kind;
        Position = position;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Lexeme = lexeme ?? throw new ArgumentNullException(nameof(lexeme));
    }

    public enum Kind
    {
        ShortOption,
        LongOption,
        Name,
    }
}

file sealed class TextBuffer : IDisposable
{
    private readonly StringReader _reader;

    public bool ReachedEnd => _reader.Peek() == -1;
    public string Original { get; }
    public int Position { get; private set; }

    public TextBuffer(string text)
    {
        _reader = new StringReader(text);
        Original = text;
        Position = 0;
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    public char Peek()
    {
        return (char)_reader.Peek();
    }

    public bool TryPeek(out char character)
    {
        var value = _reader.Peek();
        if (value == -1)
        {
            character = '\0';
            return false;
        }

        character = (char)value;
        return true;
    }

    public void Consume()
    {
        EnsureNotAtEnd();
        Read();
    }

    public void Consume(char character)
    {
        EnsureNotAtEnd();
        if (Read() != character)
        {
            throw new InvalidOperationException($"Expected '{character}' token.");
        }
    }

    public bool IsNext(char character)
    {
        if (TryPeek(out var result))
        {
            return result == character;
        }

        return false;
    }

    public char Read()
    {
        EnsureNotAtEnd();
        var result = (char)_reader.Read();
        Position++;
        return result;
    }

    private void EnsureNotAtEnd()
    {
        if (ReachedEnd)
        {
            throw new InvalidOperationException("Can't read past the end of the buffer.");
        }
    }
}