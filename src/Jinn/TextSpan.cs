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

    public override string ToString()
    {
        return $"{Position}:{Length}";
    }
}