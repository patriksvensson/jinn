namespace Jinn;

[PublicAPI]
[DebuggerDisplay("Pos={Position,nq}, Len={Length,nq}")]
public readonly struct TextSpan
{
    public int Position { get; }
    public int Length { get; }

    public int Start => Position;
    public int End => Position + Length;

    public static TextSpan Empty { get; } = new TextSpan(0, 0);

    public TextSpan(int position, int length)
    {
        if (position < 0)
        {
            throw new ArgumentException("Position must be greater than or equal to zero", nameof(position));
        }

        if (length < 0)
        {
            throw new ArgumentException("Length must be greater than or equal to zero", nameof(position));
        }

        Position = position;
        Length = length;
    }

    internal static TextSpan? Between(IReadOnlyList<Token> tokens)
    {
        var first = tokens.FirstOrDefault(x => x.Span != null)?.Span;
        var last = tokens.LastOrDefault(x => x.Span != null)?.Span;

        if (first == null && last == null)
        {
            return null;
        }

        if (first == null || last == null)
        {
            return first ?? last;
        }

        var start = Math.Min(first.Value.Position, last.Value.Position);
        var end = Math.Max(first.Value.Position + first.Value.Length, last.Value.Position + last.Value.Length);
        return new TextSpan(start, end - start);
    }

    public static TextSpan Between(TextSpan first, TextSpan second)
    {
        var start = Math.Min(first.Position, second.Position);
        var end = Math.Max(first.Position + first.Length, second.Position + second.Length);
        return new TextSpan(start, end - start);
    }

    public bool Contains(int offset)
    {
        return offset >= Position && offset <= Position + Length;
    }

    public override string ToString()
    {
        return $"{Position}:{Length}";
    }
}