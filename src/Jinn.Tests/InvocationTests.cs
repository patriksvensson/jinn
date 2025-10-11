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
    public async Task Should_Not_Call_Command_Handler_If_Argument_Handler_Was_Called_And_It_Returned_False()
    {
        // Given
        var invokedArgument = false;
        var invokedCommand = false;

        var argument = new Argument<bool>("TEST");
        argument.Arity = Arity.ZeroOrOne;
        argument.SetHandler(_ =>
        {
            invokedArgument = true;
            return false;
        });

        var rootCommand = new RootCommand();
        rootCommand.Arguments.Add(argument);
        rootCommand.SetHandler(_ =>
        {
            invokedCommand = true;
        });

        // When
        await rootCommand.Invoke(["FOO"]);

        // Then
        invokedCommand.ShouldBeFalse();
        invokedArgument.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Call_Command_Handler_If_Argument_Handler_Was_Called_And_It_Returned_True()
    {
        // Given
        var invokedArgument = false;
        var invokedCommand = false;

        var argument = new Argument<bool>("TEST");
        argument.Arity = Arity.ZeroOrOne;
        argument.SetHandler(_ =>
        {
            invokedArgument = true;
            return true;
        });

        var rootCommand = new RootCommand();
        rootCommand.Arguments.Add(argument);
        rootCommand.SetHandler(_ =>
        {
            invokedCommand = true;
        });

        // When
        await rootCommand.Invoke(["FOO"]);

        // Then
        invokedCommand.ShouldBeTrue();
        invokedArgument.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Call_Command_Handler_If_Option_Handler_Was_Called_And_It_Returned_False()
    {
        // Given
        var invokedArgument = false;
        var invokedCommand = false;
        var rootCommand = new RootCommand();

        var option = new Option<bool>("--test");
        option.Arity = Arity.ZeroOrOne;
        option.SetHandler(_ =>
        {
            invokedArgument = true;
            return false;
        });

        rootCommand.Options.Add(option);
        rootCommand.SetHandler(_ =>
        {
            invokedCommand = true;
        });

        // When
        await rootCommand.Invoke(["--test"]);

        // Then
        invokedCommand.ShouldBeFalse();
        invokedArgument.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Call_Command_Handler_If_Option_Handler_Was_Called_And_It_Returned_True()
    {
        // Given
        var invokedArgument = false;
        var invokedCommand = false;
        var rootCommand = new RootCommand();

        var option = new Option<bool>("--test");
        option.Arity = Arity.ZeroOrOne;
        option.SetHandler(_ =>
        {
            invokedArgument = true;
            return true;
        });

        rootCommand.Options.Add(option);
        rootCommand.SetHandler(_ =>
        {
            invokedCommand = true;
        });

        // When
        await rootCommand.Invoke(["--test"]);

        // Then
        invokedCommand.ShouldBeTrue();
        invokedArgument.ShouldBeTrue();
    }
}