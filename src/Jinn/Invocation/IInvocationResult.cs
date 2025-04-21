namespace Jinn;

[PublicAPI]
public interface IInvocationResult
{
    void Invoke(InvocationContext context);
}