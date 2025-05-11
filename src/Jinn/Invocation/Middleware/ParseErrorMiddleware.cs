namespace Jinn;

internal static class ParseErrorMiddleware
{
    public static async Task Invoke(InvocationContext ctx, Func<InvocationContext, Task> next)
    {
        if (ctx.ParseResult.Diagnostics.HasErrors)
        {
            if (ctx.Configuration.ErrorDiagnosticHandler == null)
            {
                throw new InvalidOperationException("Parse result contains errors");
            }

            ctx.InvocationResult = await ctx.Configuration.ErrorDiagnosticHandler(ctx.ParseResult);
            return;
        }

        await next(ctx);
    }
}