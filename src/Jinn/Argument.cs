namespace Jinn;

[PublicAPI]
public abstract class Argument : Symbol
{
    public string Name { get; }
    public Arity Arity { get; set; }
    public Type ValueType { get; }
    public bool IsRequired { get; set; }
    public Func<InvocationContext, Task<bool>>? Handler { get; set; }

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
    public static void SetHandler<T>(this Argument<T> argument, Func<InvocationContext, Task<bool>> handler)
    {
        argument.Handler = async (ctx) => await handler(ctx);
    }

    public static void SetHandler<T>(this Argument<T> argument, Func<InvocationContext, bool> handler)
    {
        argument.SetHandler(ctx =>
        {
            var result = handler(ctx);
            return Task.FromResult(result);
        });
    }

    public static Argument<T> HasArity<T>(this Argument<T> argument, Arity arity)
    {
        ArgumentNullException.ThrowIfNull(argument);
        argument.Arity = arity;
        return argument;
    }

    public static Argument<T> HasArity<T>(this Argument<T> argument, int min, int max)
    {
        ArgumentNullException.ThrowIfNull(argument);
        argument.Arity = new Arity(min, max);
        return argument;
    }

    public static Argument<T> Required<T>(this Argument<T> argument, bool isRequired = true)
    {
        ArgumentNullException.ThrowIfNull(argument);
        argument.IsRequired = isRequired;
        return argument;
    }
}