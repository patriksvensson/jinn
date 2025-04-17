namespace Jinn.Testing;

public static class Verifier
{
    public static void ShouldHaveTokens(this IReadOnlyList<Token> tokens, string expected)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            builder.Append($"({token.TokenType}){token.Value}");
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
            builder.Append($"({token.Span.Position}:{token.Span.Length}){token.Value}");
            builder.Append(' ');
        }

        var result = builder.ToString().Trim();
        result.ShouldBe(expected);
    }
}