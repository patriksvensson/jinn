using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
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
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ParserBenchmarks
{
    private readonly RootCommand _root;

    public ParserBenchmarks()
    {
        var bar = new Command("bar");
        bar.Options.Add(new Option<bool>("--baz"));
        bar.Options.Add(new Option<int>("--qux"));

        var foo = new Command("foo");
        foo.Arguments.Add(new Argument<int>("VALUE"));
        foo.Options.Add(new Option<bool>("--flag"));
        foo.Commands.Add(bar);

        _root = new RootCommand(foo);
    }

    [Benchmark]
    public void Jinn_Parse() => _root.Parse(
        ["foo", "42", "--flag", "bar", "--baz", "--qux", "32"]);

    [Benchmark]
    public async Task Jinn_Invoke() => await _root.Invoke(
        ["foo", "42", "--flag", "bar", "--baz", "--qux", "32"]);
}