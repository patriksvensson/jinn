using Jinn.Binding;

namespace Jinn;

[PublicAPI]
public sealed class InvocationContext
{
    private readonly ArgumentBinderContext _binderContext;
    private readonly Dictionary<string, object?> _properties;

    public ParseResult ParseResult { get; }
    public Configuration Configuration { get; }
    public IInvocationResult? InvocationResult { get; set; }

    public InvocationContext(ParseResult parseResult)
    {
        _binderContext = new ArgumentBinderContext();
        _properties = new Dictionary<string, object?>(StringComparer.Ordinal);

        ParseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        Configuration = parseResult.Configuration;
    }

    public T? GetValue<T>(Option<T> option)
    {
        if (!ParseResult.Lookup.TryGetValue(option.Argument, out var result))
        {
            Trace.Assert(!option.IsRequired, "Validation middleware should have caught this");
            return (T?)option.Argument.GetDefaultValue();
        }

        if (result is not ArgumentResult argumentResult)
        {
            return (T?)option.Argument.GetDefaultValue();
        }

        return ArgumentBinder.GetValue<T>(_binderContext, argumentResult);
    }

    public T? GetValue<T>(Argument<T> argument)
    {
        if (!ParseResult.Lookup.TryGetValue(argument, out var result))
        {
            Trace.Assert(!argument.IsRequired, "Validation middleware should have caught this");
            return (T?)argument.GetDefaultValue();
        }

        if (result is not ArgumentResult argumentResult)
        {
            return (T?)argument.GetDefaultValue();
        }

        return ArgumentBinder.GetValue<T>(_binderContext, argumentResult);
    }

    public T? GetProperty<T>(string name)
    {
        if (_properties.TryGetValue(name, out var value) && value is T casted)
        {
            return casted;
        }

        return default;
    }

    public void SetProperty<T>(string name, T value)
    {
        _properties[name] = value;
    }
}

[PublicAPI]
public static class InvocationContextExtensions
{
    public static int GetExitCode(this InvocationContext ctx)
    {
        return ctx.GetProperty<int>(Constants.Invocation.ExitCode);
    }

    public static void SetExitCode(this InvocationContext ctx, int exitCode)
    {
        ctx.SetProperty(Constants.Invocation.ExitCode, exitCode);
    }
}