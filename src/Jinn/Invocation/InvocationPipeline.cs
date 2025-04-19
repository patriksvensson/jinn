namespace Jinn;

internal sealed class InvocationPipeline
{
    private readonly ParseResult _parseResult;
    private readonly List<InvocationMiddleware> _middlewares;
    private readonly Configuration _configuration;

    public InvocationPipeline(
        ParseResult parseResult,
        List<InvocationMiddleware> middlewares,
        Configuration configuration)
    {
        _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<int> Invoke()
    {
        var context = new InvocationContext(_parseResult, _configuration);
        var chain = BuildInvocationChain(context);

        await chain.Invoke(context, static _ => Task.CompletedTask);
        return context.ExitCode;
    }

    private InvocationMiddleware BuildInvocationChain(InvocationContext context)
    {
        var middleware = new List<InvocationMiddleware>();
        middleware.AddRange(_middlewares);

        // Call the command handler as the last step
        // in the invocation chain.
        middleware.Add(async (invocationContext, _) =>
        {
            var handler = invocationContext.ParseResult.Command.Owner.Handler;
            if (handler is not null)
            {
                context.ExitCode = await handler(context);
            }
        });

        return middleware.Aggregate(
            (first, second) =>
                (ctx, next) =>
                    first(ctx, c => second(c, next)));
    }
}