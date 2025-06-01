namespace Jinn.Binding;

internal static class ArgumentBinder
{
    public static object? GetValue(
        ArgumentBinderContext context,
        ArgumentResult argumentResult)
    {
        var value = Bind(context, argumentResult);
        switch (value)
        {
            case ArgumentResultValue.Failure failure:
                throw new InvalidOperationException(failure.Error.Message);
            case ArgumentResultValue.Success success:
                return success.Value;
            default:
                throw new InvalidOperationException("Unknown argument binding result");
        }
    }

    public static T? GetValue<T>(
        ArgumentBinderContext context,
        ArgumentResult argumentResult)
    {
        var value = Bind(context, argumentResult);
        switch (value)
        {
            case ArgumentResultValue.Failure failure:
                throw new InvalidOperationException(failure.Error.Message);
            case ArgumentResultValue.Success success when success.Value is T converted:
                return converted;
            case ArgumentResultValue.Success:
                return default;
            default:
                throw new InvalidOperationException("Unknown argument binding result");
        }
    }

    public static ArgumentResultValue Bind(
        ArgumentBinderContext context,
        ArgumentResult result)
    {
        if (context.TryGetValue(result, out var value))
        {
            return value;
        }

        value = BindInternal(result);
        context.SetValue(result, value);
        return value;
    }

    private static ArgumentResultValue BindInternal(ArgumentResult result)
    {
        // Want exactly one token?
        if (result.Arity.Minimum == 1 && result.Arity.Maximum == 1)
        {
            Trace.Assert(result.Tokens.Count > 0, "Expected at least one token"); // Validation would have caught this
            return ConvertToken(result, result.ArgumentSymbol.ValueType, result.Tokens[0]);
        }

        // Want no tokens?
        if (result.Arity.Maximum == 0)
        {
            // This would be a boolean flag if anything
            return new ArgumentResultValue.Success(result.ArgumentSymbol, true);
        }

        // Want a maximum of one token?
        if (result.Arity.Maximum == 1)
        {
            if (result.Tokens.Count == 0)
            {
                // Is this a bool?
                if (result.ArgumentSymbol.ValueType == typeof(bool))
                {
                    return new ArgumentResultValue.Success(result.ArgumentSymbol, true);
                }

                // Get the default value
                var defaultValue = result.ArgumentSymbol.GetDefaultValue();
                return new ArgumentResultValue.Success(result.ArgumentSymbol, defaultValue);
            }

            return ConvertToken(result, result.ArgumentSymbol.ValueType, result.Tokens[0]);
        }

        // Wants a maximum of more than one token
        var container = ArgumentContainer.CreateContainer(result);
        if (container != null)
        {
            for (var i = 0; i < result.Tokens.Count; i++)
            {
                var converted = ConvertToken(result, container.ElementType, result.Tokens[i]);
                if (converted is ArgumentResultValue.Success success)
                {
                    container.Add(i, success.Value);
                }
                else
                {
                    // Return the error
                    return converted;
                }
            }

            return container.GetResult();
        }

        // Custom converter is needed
        return new ArgumentResultValue.Failure(
            result.ArgumentSymbol,
            ArgumentBindingErrors.JINN2002_CustomConverterNeeded(result.ArgumentSymbol)
                .ToDiagnostic(span: null));
    }

    private static ArgumentResultValue ConvertToken(ArgumentResult result, Type type, Token token)
    {
        if (!ArgumentConverters.TryGetConverter(type, out var converter))
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null || !ArgumentConverters.TryGetConverter(underlyingType, out converter))
            {
                // JINN2000: Can't find converter
                return new ArgumentResultValue.Failure(
                    result.ArgumentSymbol,
                    ArgumentBindingErrors.JINN2000_CannotFindConverter(result.ArgumentSymbol)
                        .ToDiagnostic(span: null));
            }
        }

        if (!converter(token, out var bound))
        {
            // JINN2001: Can't convert argument
            return new ArgumentResultValue.Failure(
                result.ArgumentSymbol,
                ArgumentBindingErrors.JINN2001_CannotConvertArgument(result.ArgumentSymbol, token)
                    .ToDiagnostic(token.Span));
        }

        return new ArgumentResultValue.Success(result.ArgumentSymbol, bound);
    }
}