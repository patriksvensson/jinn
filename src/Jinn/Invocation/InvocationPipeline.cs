namespace Jinn;

internal sealed class InvocationPipeline
{
    private readonly ParseResult _parseResult;

    public InvocationPipeline(ParseResult parseResult)
    {
        _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
    }

    public async Task<int> Invoke()
    {
        var context = new InvocationContext(_parseResult);
        var chain = BuildInvocationChain(context);

        await chain.Invoke(context, static _ => Task.CompletedTask);

        context.InvocationResult?.Invoke(context);
        return context.ExitCode;
    }

    private InvocationMiddleware BuildInvocationChain(InvocationContext context)
    {
        List<InvocationMiddleware> middlewares =
        [
            ExceptionMiddleware.Invoke,
            ErrorDiagnosticMiddleware.Invoke,
            .. _parseResult.Configuration.Middlewares,
            async (invocationContext, _) =>
            {
                // Call the command handler as the last step in the invocation chain.
                var handler = invocationContext.ParseResult.Root.Command.Handler;
                if (handler is not null)
                {
                    await handler(context);
                }
            }
        ];

        return middlewares.Aggregate(
            (first, second) =>
                (ctx, next) =>
                    first(ctx, c => second(c, next)));
    }
}