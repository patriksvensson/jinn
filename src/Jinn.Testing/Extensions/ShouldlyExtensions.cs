using Shouldly;
using Spectre.Console.Testing;

namespace Jinn.Testing;

[PublicAPI]
public static class ShouldlyExtensions
{
    extension(ParseResult hir)
    {
        public void ShouldHaveDiagnostics(string expected)
        {
            hir.ToErrata()
                .TrimLines()
                .ShouldBe(expected.TrimLines());
        }
    }

    extension<T>(T obj)
    {
        public T And()
        {
            return obj;
        }

        public T And(Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}