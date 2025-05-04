using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Errata;
using Spectre.Console;

namespace Jinn.Testing;

internal sealed class DiagnosticRenderer
{
    private readonly IAnsiConsole _console;

    public DiagnosticRenderer(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Render(
        ParseResult result,
        Diagnostics diagnostics,
        ReportSettings? settings = null)
    {
        var repo = new InMemorySourceRepository();
        repo.Register("args", string.Join(" ", result.Tokens.Select(x => x.Lexeme)));

        var report = new Report(repo);
        foreach (var group in diagnostics.GroupBy(x => x.Code))
        {
            var aggregatedGroup = group.ToArray();
            var item = report.AddDiagnostic(CreateErrataDiagnostic(aggregatedGroup[0]));

            foreach (var diagnostic in aggregatedGroup)
            {
                if (diagnostic.Span != null)
                {
                    item.WithLabel(
                        new Label(
                                "args",
                                new Errata.TextSpan(
                                    diagnostic.Span.Value.Position,
                                    diagnostic.Span.Value.Position + diagnostic.Span.Value.Length),
                                diagnostic.Message)
                            .WithPriority(1)
                            .WithColor(item.Color));
                }
            }
        }

        _console.WriteLine();
        report.Render(_console, settings);
    }

    private static Errata.Diagnostic CreateErrataDiagnostic(Diagnostic diagnostic)
    {
        var message = diagnostic.Span == null ? diagnostic.Message : diagnostic.Summary;

        var result = diagnostic.Severity switch
        {
            Severity.Information => Errata.Diagnostic.Info(message),
            Severity.Warning => Errata.Diagnostic.Warning(message),
            Severity.Error => Errata.Diagnostic.Error(message),
            _ => throw new InvalidOperationException("Unknown diagnostic severity"),
        };

        return diagnostic.Code != null
            ? result.WithCode(diagnostic.Code)
            : result;
    }
}