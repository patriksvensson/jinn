using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;

namespace Jinn.Testing;

public static class Verifier
{
    public static void Verify<T>(T obj, string expected)
    {
        var result = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            IndentCharacter = ' ',
            IndentSize = 4,
        });

        result.Trim('[', ']').Trim().ShouldBe(expected.Trim());
    }

    public static void ShouldHaveTokens(this IReadOnlyList<Token> tokens, string expected)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            builder.Append($"({token.Type}){token.Value}");
            builder.Append(' ');
        }

        var result = builder.ToString().Trim();
        result.ShouldBe(expected);
    }
}