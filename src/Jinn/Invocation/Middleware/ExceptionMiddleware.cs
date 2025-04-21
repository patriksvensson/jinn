namespace Jinn;

internal static class ExceptionMiddleware
{
    public static async Task Invoke(InvocationContext ctx, Func<InvocationContext, Task> next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex) when (ctx.Configuration.ExceptionHandler != null)
        {
            // Invoke the handler and set the invocation result
            ctx.InvocationResult = await ctx.Configuration.ExceptionHandler(ex, ctx.ParseResult);
        }
    }
}