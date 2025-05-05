namespace Jinn.Binding;

internal static class ArgumentBinder
{
    public static ArgumentConversionResult GetValue(ArgumentResult result)
    {
        if (result.Arity.Minimum == 1 && result.Arity.Maximum == 1)
        {
            Trace.Assert(result.Tokens.Count > 0, "Expected at least one token");

            if (!ArgumentBinders.TryGetBinder(result, out var binder))
            {
                // JINN2000: Can't find converter
                return new ArgumentConversionResult.Failure(
                    result.Argument,
                    BindingErrors.JINN2000_CannotFindConverter(result.Argument)
                        .ToDiagnostic(span: null));
            }

            var token = result.Tokens[0];
            if (!binder(token, out var bound))
            {
                // JINN2001: Can't bind argument
                return new ArgumentConversionResult.Failure(
                    result.Argument,
                    BindingErrors.JINN2001_CannotBindArgument(result.Argument, token)
                        .ToDiagnostic(token.Span));
            }

            return new ArgumentConversionResult.Success(result.Argument, bound);
        }

        if (result.Arity.Maximum == 0)
        {
            throw new NotImplementedException("Arity ?,0");
        }

        if (result.Arity.Maximum == 1)
        {
            throw new NotImplementedException("Arity ?,1");
        }

        throw new NotImplementedException("Arity ?,>1");
    }
}

internal static class BindingErrors
{
    public static DiagnosticDescriptor JINN2000_CannotFindConverter(Argument argument) =>
    new("JINN2000", Severity.Error, "Cannot bind argument",
        $"Cannot find converter for argument {argument.Name}");

    public static DiagnosticDescriptor JINN2001_CannotBindArgument(Argument argument, Token token) =>
        new("JINN2001", Severity.Error, "Cannot bind argument",
            $"Cannot bind argument {argument.Name} with value {token.Lexeme}");
}

public abstract class ArgumentConversionResult
{
    public Argument Argument { get; }

    protected ArgumentConversionResult(Argument argument)
    {
        Argument = argument;
    }

    public sealed class Success : ArgumentConversionResult
    {
        public object? Value { get; }

        public Success(Argument argument, object? value)
            : base(argument)
        {
            Value = value;
        }
    }

    public sealed class Failure : ArgumentConversionResult
    {
        public Diagnostic Error { get; }

        public Failure(Argument argument, Diagnostic error)
            : base(argument)
        {
            Error = error;
        }
    }
}

internal delegate bool TryBindSingleToken(Token token, out object? result);

internal static class ArgumentBinders
{
    private static readonly Dictionary<Type, TryBindSingleToken> _binders = new()
    {
        { typeof(int), TryBindInt },
    };

    public static bool TryGetBinder(ArgumentResult result, [NotNullWhen(true)] out TryBindSingleToken? binder)
    {
        return _binders.TryGetValue(result.Argument.ValueType, out binder);
    }

    private static bool TryBindInt(Token token, out object? result)
    {
        if (int.TryParse(token.Lexeme, out var intResult))
        {
            result = intResult;
            return true;
        }

        result = null;
        return false;
    }
}