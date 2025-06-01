using System.Globalization;

namespace Jinn.Binding;

internal delegate bool TryBindSingleToken(Token token, out object? result);

internal static class ArgumentConverters
{
    private static readonly Dictionary<Type, TryBindSingleToken> _binders = new()
    {
        { typeof(DateTime), TryConvert<DateTime> },
        { typeof(DateTimeOffset), TryConvert<DateTimeOffset> },
        { typeof(TimeSpan), TryConvert<TimeSpan> },
        { typeof(bool), TryConvert<bool> },
        { typeof(sbyte), TryConvert<sbyte> },
        { typeof(short), TryConvert<short> },
        { typeof(int), TryConvert<int> },
        { typeof(long), TryConvert<long> },
        { typeof(byte), TryConvert<byte> },
        { typeof(ushort), TryConvert<ushort> },
        { typeof(uint), TryConvert<uint> },
        { typeof(ulong), TryConvert<ulong> },
        { typeof(string), TryConvertString },
        { typeof(Uri), TryConvertUri },
    };

    public static bool TryGetConverter(Type type, [NotNullWhen(true)] out TryBindSingleToken? binder)
    {
        return _binders.TryGetValue(type, out binder);
    }

    public static bool TryGetConverter(ArgumentResult result, [NotNullWhen(true)] out TryBindSingleToken? binder)
    {
        return _binders.TryGetValue(result.ArgumentSymbol.ValueType, out binder);
    }

    private static bool TryConvert<T>(Token token, out object? result)
        where T : IParsable<T>
    {
        if (T.TryParse(token.Lexeme, CultureInfo.InvariantCulture, out var intResult))
        {
            result = intResult;
            return true;
        }

        result = null;
        return false;
    }

    private static bool TryConvertString(Token token, out object? result)
    {
        result = token.Lexeme;
        return true;
    }

    private static bool TryConvertUri(Token token, out object? result)
    {
        if (Uri.TryCreate(token.Lexeme, UriKind.RelativeOrAbsolute, out var uri))
        {
            result = uri;
            return true;
        }

        result = null;
        return false;
    }
}