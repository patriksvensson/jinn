namespace Jinn.Tests;

public sealed class InvocationTests
{
    [Fact]
    public async Task Should_Invoke_Command()
    {
        // Given
        var invoked = false;

        var rootCommand = new RootCommand();
        rootCommand.SetHandler(ctx =>
        {
            invoked = true;
            ctx.ExitCode = 1;
        });

        // When
        var result = await rootCommand.Invoke([]);

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
        rootCommand.Configuration.AddMiddleware(async (ctx, next) =>
        {
            invoked = true;
            await next(ctx);
        });

        // When
        await rootCommand.Invoke([]);

        // Then
        invoked.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Invoke_Command_After_Middleware()
    {
        // Given
        var invokedCommand = false;
        var invokedMiddleware = false;
        var rootCommand = new RootCommand();

        rootCommand.Configuration.AddMiddleware(async (ctx, next) =>
        {
            invokedMiddleware = true;
            ctx.ExitCode = 1;
            await next(ctx);
        });

        rootCommand.SetHandler(ctx =>
        {
            invokedCommand = true;
            ctx.ExitCode = 2;
        });

        // When
        var result = await rootCommand.Invoke([]);

        // Then
        invokedMiddleware.ShouldBeTrue();
        invokedCommand.ShouldBeTrue();
        result.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Stop_Invocation_Of_Command_If_Middleware_Never_Calls_Next()
    {
        // Given
        var invoked = false;
        var rootCommand = new RootCommand();

        rootCommand.Configuration.AddMiddleware((ctx, _) =>
        {
            ctx.ExitCode = 1;
            return Task.CompletedTask;
        });

        rootCommand.SetHandler(ctx =>
        {
            invoked = true;
            ctx.ExitCode = 2;
        });

        // When
        var result = await rootCommand.Invoke([]);

        // Then
        invoked.ShouldBeFalse();
        result.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Invoke_Parse_Error_Diagnostic_Middleware_If_Errors_Are_Present()
    {
        // Given
        var diagnostics = default(Diagnostics);

        var rootCommand = new RootCommandFixture();
        rootCommand.AddArgument(new Argument<int>("VALUE").HasArity(2, 2));
        rootCommand.Configuration.ErrorDiagnosticHandler = parseResult =>
        {
            diagnostics = parseResult.Diagnostics;
            return Task.FromResult<IInvocationResult>(
                new CustomInvocationResult(ctx => ctx.ExitCode = 9));
        };

        // When
        var result = await rootCommand.Invoke("42");

        // Then
        diagnostics.ShouldNotBeNull();
        diagnostics.Count.ShouldBe(1);
        result.ShouldBe(9);
    }
}