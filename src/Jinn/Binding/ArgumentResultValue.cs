namespace Jinn.Binding;

internal abstract class ArgumentResultValue
{
    public Argument Argument { get; }

    private ArgumentResultValue(Argument argument)
    {
        Argument = argument;
    }

    public sealed class Success : ArgumentResultValue
    {
        public object? Value { get; }

        public Success(Argument argument, object? value)
            : base(argument)
        {
            Value = value;
        }
    }

    public sealed class Failure : ArgumentResultValue
    {
        public Diagnostic Error { get; }

        public Failure(Argument argument, Diagnostic error)
            : base(argument)
        {
            Error = error;
        }
    }
}