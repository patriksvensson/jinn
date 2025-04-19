namespace Jinn;

public sealed class ParseError
{
    public string Message { get; }
    public TextSpan? Span { get; }

    public ParseError(string message, TextSpan? span = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Span = span;
    }
}