namespace Jinn;

[PublicAPI]
public delegate Task InvocationMiddleware(
    InvocationContext context,
    Func<InvocationContext, Task> next);