namespace Jinn;

internal static class HelpMiddleware
{
    public static async Task Invoke(InvocationContext ctx, Func<InvocationContext, Task> next)
    {
        await next(ctx);

        // Should we show help?
        if (ctx.GetProperty<bool>(Constants.Invocation.ShowHelp))
        {
            if (ctx.Configuration.HelpProvider != null)
            {
                ctx.InvocationResult = await ctx.Configuration.HelpProvider(ctx.ParseResult);
            }
            else
            {
                throw new InvalidOperationException("No help provider has been registered");
            }
        }
    }
}

internal sealed class CustomHelpInvocationResult : IInvocationResult
{
    private readonly Action<InvocationContext> _action;

    public CustomHelpInvocationResult(Action<InvocationContext> action)
    {
        _action = action;
    }

    public void Invoke(InvocationContext context)
    {
        _action(context);
    }
}