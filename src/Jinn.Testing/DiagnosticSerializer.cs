using Spectre.Console.Testing;

namespace Jinn.Testing;

public static class DiagnosticSerializer
{
    extension(ParseResult result)
    {
        public string ToErrata()
        {
            var console = new TestConsole();

            // Render errata to the test console
            var renderer = new DiagnosticRenderer(console);
            renderer.Render(result, result.Diagnostics, new ReportSettings
            {
                LeftPadding = false,
            });

            // Trim the end of each line
            var lines = new List<string>();
            foreach (var line in console.Output.Split("\n"))
            {
                lines.Add(line.TrimEnd());
            }

            return string.Join("\n", lines).Trim().NormalizeLineEndings();
        }
    }
}