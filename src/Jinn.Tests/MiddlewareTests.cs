namespace Jinn.Tests;

public sealed class MiddlewareTests
{
    [Fact]
    public async Task Should_Invoke_Help_Provider_If_Help_Is_Specified()
    {
        // Given
        var helpInvoked = false;
        var fixture = new RootCommandFixture();
        fixture.Configuration.SetHelpProvider(ctx =>
        {
            helpInvoked = true;
        });

        // When
        await fixture.Invoke("--help");

        // Then
        helpInvoked.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Invoke_Help_Provider_And_Have_The_Correct_Command_Set_If_Help_Is_Specified()
    {
        // Given
        var invokedCommand = default(Command?);
        var command = new Command("test");

        var fixture = new RootCommandFixture();
        fixture.AddCommand(command);
        fixture.Configuration.SetHelpProvider(ctx =>
        {
            invokedCommand = ctx.ParseResult.ParsedCommand.CommandSymbol;
        });

        // When
        await fixture.Invoke("test --help");

        // Then
        invokedCommand
            .ShouldNotBeNull()
            .ShouldBe(command);
    }

    [Fact]
    public async Task Should_Throw_If_No_Help_Provider_Has_Been_Registered()
    {
        // Given
        var fixture = new RootCommandFixture();

        // When
        var result = await Record.ExceptionAsync(
            async () => await fixture.Invoke("--help"));

        // Then
        result.ShouldBeOfType<InvalidOperationException>()
            .And().Message.ShouldBe("No help provider has been registered");
    }

    [Fact]
    public async Task Should_Invoke_Parse_Error_Diagnostic_Middleware_If_Errors_Are_Present()
    {
        // Given
        var diagnostics = default(Diagnostics);

        var rootCommand = new RootCommandFixture();
        rootCommand.AddArgument(new Argument<int>("VALUE").HasArity(2, 2));
        rootCommand.Configuration.SetParseErrorHandler(ctx =>
        {
            diagnostics = ctx.ParseResult.Diagnostics;
            ctx.SetExitCode(9);
        });

        // When
        var result = await rootCommand.Invoke("42");

        // Then
        diagnostics.ShouldNotBeNull();
        diagnostics.Count.ShouldBe(1);
        result.ShouldBe(9);
    }

    [Fact]
    public async Task Should_Throw_If_No_Parse_Error_Handler_Has_Been_Registered()
    {
        // Given
        var fixture = new RootCommandFixture();
        fixture.AddArgument(new Argument<int>("VALUE").HasArity(2, 2));

        // When
        var result = await Record.ExceptionAsync(
            async () => await fixture.Invoke("42"));

        // Then
        result.ShouldBeOfType<InvalidOperationException>()
            .And().Message.ShouldBe("No parse error handler has been registered");
    }
}