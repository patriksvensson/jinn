namespace Jinn;

internal static class ParseErrorMiddleware
{
    public static async Task Invoke(
        InvocationContext ctx, Func<InvocationContext, CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (ctx.ParseResult.Diagnostics.HasErrors)
        {
            if (ctx.Configuration.ParseErrorHandler != null)
            {
                ctx.InvocationResult = await ctx.Configuration.ParseErrorHandler(ctx.ParseResult);
            }
            else
            {
                throw new InvalidOperationException("No parse error handler has been registered");
            }

            return;
        }

        await next(ctx, cancellationToken);
    }
}

internal sealed class CustomParseErrorInvocationResult : IInvocationResult
{
    private readonly Action<InvocationContext> _action;

    public CustomParseErrorInvocationResult(Action<InvocationContext> action)
    {
        _action = action;
    }

    public void Invoke(InvocationContext context)
    {
        _action(context);
    }
}