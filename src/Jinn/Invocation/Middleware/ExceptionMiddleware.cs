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
            // Invoke the handler and set the invocation result.
            // If no exception handler has been set, we let the exception bubble up.
            ctx.InvocationResult = await ctx.Configuration.ExceptionHandler(ex, ctx.ParseResult);
        }
    }
}

internal sealed class CustomExceptionInvocationResult : IInvocationResult
{
    private readonly Action<InvocationContext, Exception> _action;
    private readonly Exception _exception;

    public CustomExceptionInvocationResult(
        Action<InvocationContext, Exception> action,
        Exception exception)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    public void Invoke(InvocationContext context)
    {
        _action(context, _exception);
    }
}