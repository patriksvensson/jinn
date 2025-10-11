using Jinn;
using Spectre.Console;
using Spectre.Console.Rendering;

var root = new RootCommand();
var option = new Option<List<int>>("--item").HasArity(2, 5);

root.Options.Add(option);
root.SetHandler(ctx =>
{
    var items = ctx.GetValue(option);
    if (items != null)
    {
        foreach (var item in items)
        {
            Console.WriteLine($"Item: #{item}");
        }
    }
});

root.Configuration.SetParseErrorHandler(ctx =>
{
    var rows = new List<IRenderable>();
    foreach (var diagnostic in ctx.ParseResult.Diagnostics)
    {
        rows.Add(new Markup($"[red]{diagnostic.Code}[/]: {diagnostic.Message}"));
    }

    AnsiConsole.Write(new Panel(new Rows(rows))
        .Header($"{ctx.ParseResult.Diagnostics.Count} Error(s)")
        .RoundedBorder());

    ctx.ExitCode = 32;
});

return await root.Invoke(args);