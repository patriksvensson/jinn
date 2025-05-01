namespace Jinn.Testing;

public sealed class CustomInvocationResult : IInvocationResult
{
    private readonly Action<InvocationContext>? _action;

    public CustomInvocationResult(Action<InvocationContext>? action = null)
    {
        _action = action;
    }

    public void Invoke(InvocationContext context)
    {
        _action?.Invoke(context);
    }
}