namespace Jinn;

internal static class ParseErrorMiddleware
{
    public static async Task Invoke(InvocationContext ctx, Func<InvocationContext, Task> next)
    {
        if (ctx.ParseResult.Errors.Count > 0)
        {
            if (ctx.Configuration.ParseErrorHandler == null)
            {
                // No error handler attached
                throw new InvalidOperationException("No error handler has been specified");
            }

            // Invoke the handler and set the invocation result
            ctx.InvocationResult = await ctx.Configuration.ParseErrorHandler(ctx.ParseResult);
        }
        else
        {
            await next(ctx);
        }
    }
}