namespace Jinn;

internal sealed class StringArgument : IEnumerable<char>
{
    public string Value { get; }
    public bool IsSynthetic { get; }
    public int Length => Value.Length;

    public StringArgument(string value, bool isSynthetic = false)
    {
        Value = value;
        IsSynthetic = isSynthetic;
    }

    public static StringArgument Syntetic(string argument)
    {
        return new StringArgument(argument, isSynthetic: true);
    }

    public static implicit operator string(StringArgument arg) => arg.Value;

    public IEnumerator<char> GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    public override string ToString()
    {
        return Value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}