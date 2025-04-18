namespace Jinn;

internal sealed class InvocationPipeline
{
    private readonly ParseResult _parseResult;
    private readonly List<InvocationMiddleware> _middlewares;

    public InvocationPipeline(
        ParseResult parseResult,
        List<InvocationMiddleware> middlewares)
    {
        _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
    }

    public async Task<int> Invoke()
    {
        var context = new InvocationContext(_parseResult);
        var chain = BuildInvocationChain(context);

        await chain.Invoke(context, static _ => Task.CompletedTask);
        return context.ExitCode;
    }

    private InvocationMiddleware BuildInvocationChain(InvocationContext context)
    {
        var middleware = new List<InvocationMiddleware>();
        middleware.AddRange(_middlewares);

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