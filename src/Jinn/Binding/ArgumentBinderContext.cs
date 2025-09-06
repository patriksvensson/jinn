namespace Jinn.Binding;

internal sealed class ArgumentBinderContext
{
    private readonly Dictionary<ArgumentResult, ArgumentResultValue> _values = new();

    public void SetValue(ArgumentResult result, ArgumentResultValue resultValue)
    {
        _values[result] = resultValue;
    }

    public bool TryGetValue(ArgumentResult result, [NotNullWhen(true)] out ArgumentResultValue? value)
    {
        return _values.TryGetValue(result, out value);
    }
}