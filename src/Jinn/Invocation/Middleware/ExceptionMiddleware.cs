namespace Jinn;

internal static class ExceptionMiddleware
{
    public static async Task Invoke(InvocationContext ctx, Func<InvocationContext, Task> next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            if (ctx.Configuration.ExceptionHandler == null)
            {
                throw new InvalidOperationException("No exception handler has been specified");
            }

            // Invoke the handler and set the invocation result
            ctx.InvocationResult = await ctx.Configuration.ExceptionHandler(ex, ctx.ParseResult);
        }
    }
}