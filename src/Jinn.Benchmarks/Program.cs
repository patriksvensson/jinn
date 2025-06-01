using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Jinn.Benchmarks;

public static class Program
{
    public static int Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);
        return 0;
    }
}

[MemoryDiagnoser]
public class ParserBenchmarks
{
    private readonly RootCommand _root;

    public ParserBenchmarks()
    {
        var bar = new Command("bar");
        bar.AddOption(new Option<bool>("--baz"));
        bar.AddOption(new Option<int>("--qux"));

        var foo = new Command("foo");
        foo.AddArgument(new Argument<int>("VALUE"));
        foo.AddOption(new Option<bool>("--flag"));
        foo.AddCommand(bar);

        _root = new RootCommand(foo);
    }

    [Benchmark]
    public void Jinn_Parse() => _root.Parse(
        ["foo", "42", "--flag", "bar", "--baz", "--qux", "32"]);

    [Benchmark]
    public async Task Jinn_Invoke() => await _root.Invoke(
        ["foo", "42", "--flag", "bar", "--baz", "--qux", "32"]);
}