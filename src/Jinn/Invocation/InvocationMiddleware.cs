namespace Jinn;

public delegate Task InvocationMiddleware(
    InvocationContext context,
    Func<InvocationContext, Task> next);