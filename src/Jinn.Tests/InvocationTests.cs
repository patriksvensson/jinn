namespace Jinn.Tests;

public sealed class InvocationTests
{
    [Fact]
    public async Task Should_Invoke_Command()
    {
        // Given
        var invoked = false;

        var rootCommand = new RootCommand();
        rootCommand.Options.Add(new Option<bool>("--lol"));
        rootCommand.SetHandler(_ =>
        {
            invoked = true;
            return Task.FromResult(1);
        });

        // When
        var result = await rootCommand.Invoke(["--lol"]);

        // Then
        invoked.ShouldBeTrue();
        result.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Invoke_Middleware()
    {
        // Given
        var invoked = false;

        var rootCommand = new RootCommand();
        rootCommand.Options.Add(new Option<bool>("--lol"));
        rootCommand.AddMiddleware(async (ctx, next) =>
        {
            invoked = true;
            await next(ctx);
        });

        // When
        var result = await rootCommand.Invoke(["--lol"]);

        // Then
        invoked.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Stop_Invocation_Of_Command_If_Middleware_Never_Calls_Next()
    {
        // Given
        var invoked = false;

        var rootCommand = new RootCommand();
        rootCommand.Options.Add(new Option<bool>("--lol"));

        rootCommand.AddMiddleware((ctx, _) =>
        {
            ctx.ExitCode = 2;
            return Task.CompletedTask;
        });

        rootCommand.SetHandler(_ =>
        {
            invoked = true;
            return Task.FromResult(1);
        });

        // When
        var result = await rootCommand.Invoke(["--lol"]);

        // Then
        invoked.ShouldBeFalse();
        result.ShouldBe(2);
    }
}