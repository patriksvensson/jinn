namespace Jinn;

[PublicAPI]
public delegate Task InvocationMiddleware(
    InvocationContext context,
    Func<InvocationContext, CancellationToken, Task> next,
    CancellationToken cancellationToken);