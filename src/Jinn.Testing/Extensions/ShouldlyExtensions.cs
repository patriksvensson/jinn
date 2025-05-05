using Shouldly;
using Spectre.Console.Testing;

namespace Jinn.Testing;

[PublicAPI]
public static class ShouldlyExtensions
{
    public static T And<T>(this T obj)
    {
        return obj;
    }

    public static T And<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }

    public static void ShouldHaveDiagnostics(this ParseResult hir, string expected)
    {
        hir.ToErrata()
           .TrimLines()
           .ShouldBe(expected.TrimLines());
    }
}