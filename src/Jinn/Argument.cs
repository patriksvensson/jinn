namespace Jinn;

[PublicAPI]
public abstract class Argument : Symbol
{
    public string Name { get; }
    public Arity Arity { get; set; }
    public Type ValueType { get; }
    public bool IsRequired { get; set; }
    public Func<InvocationContext, CancellationToken, Task<bool>>? Handler { get; set; }

    protected Argument(Type type, string name)
    {
        ValueType = type;
        Arity = Arity.Resolve(type);
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public bool IsBoolean()
    {
        return ValueType == typeof(bool);
    }

    internal abstract object? GetDefaultValue();
}

[PublicAPI]
public sealed class Argument<T> : Argument
{
    public Func<T>? DefaultValueFactory { get; set; }

    public Argument(string name)
        : base(typeof(T), name)
    {
    }

    internal override object? GetDefaultValue()
    {
        return DefaultValueFactory != null
            ? DefaultValueFactory()
            : default(T);
    }
}

[PublicAPI]
public static class ArgumentExtensions
{
    extension<T>(Argument<T> argument)
    {
        public void SetHandler(Func<InvocationContext, CancellationToken, Task<bool>> handler)
        {
            argument.Handler = async (ctx, ct) => await handler(ctx, ct);
        }

        public void SetHandler(Func<InvocationContext, CancellationToken, bool> handler)
        {
            argument.SetHandler((ctx, ct) =>
            {
                var result = handler(ctx, ct);
                return Task.FromResult(result);
            });
        }
    }
}