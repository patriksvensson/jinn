namespace Jinn.Tests;

public sealed class BindingTests
{
    [Fact]
    public async Task Lol()
    {
        // Given
        var rootCommand = new RootCommand();
        var opt = rootCommand.AddOption(new Option<int>("--value"));
        rootCommand.SetHandler(ctx =>
        {
            ctx.TryGetValue(opt);
        });

        // When
        var result = await rootCommand.Invoke(["--value", "42"]);
    }
}
