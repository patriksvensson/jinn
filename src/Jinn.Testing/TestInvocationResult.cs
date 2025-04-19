namespace Jinn.Testing;

public class TestInvocationResult : IInvocationResult
{
    public void Invoke(InvocationContext context)
    {
        context.ExitCode.ShouldBe(255);
    }
}