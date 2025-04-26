namespace Jinn.Testing;

public static class TokenExtensions
{
    public static void ShouldHaveTokens(this IReadOnlyList<Token> tokens, string expected)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            builder.Append($"({token.Kind}){token.Lexeme}");
            builder.Append(' ');
        }

        var result = builder.ToString().Trim();
        result.ShouldBe(expected);
    }

    public static void ShouldHaveTokenSpans(this IReadOnlyList<Token> tokens, string expected)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            builder.Append($"({token.Span.Position}:{token.Span.Length}){token.Lexeme}");
            builder.Append(' ');
        }

        var result = builder.ToString().Trim();
        result.ShouldBe(expected);
    }
}