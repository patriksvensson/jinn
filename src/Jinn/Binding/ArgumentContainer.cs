namespace Jinn.Binding;

internal sealed class ArgumentContainer
{
    private readonly ArgumentResult _result;
    private readonly IList _container;
    private readonly bool _isArray;

    public Type ElementType { get; }

    private ArgumentContainer(ArgumentResult result, IList container, Type elementType, bool isArray)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _isArray = isArray;
        ElementType = elementType;
    }

    public void Add(int index, object? value)
    {
        if (_isArray)
        {
            _container[index] = value;
        }
        else
        {
            _container.Add(value);
        }
    }

    public ArgumentResultValue GetResult()
    {
        return new ArgumentResultValue.Success(_result.Argument, _container);
    }

    public static ArgumentContainer? CreateContainer(ArgumentResult result)
    {
        var type = result.Argument.ValueType;

        var capacity = result.Tokens.Count;
        var elementType = GetContainerElementType(result.Argument.ValueType);
        if (elementType == null)
        {
            return null;
        }

        if (type.IsArray)
        {
            var list = Array.CreateInstance(elementType, capacity);
            return new ArgumentContainer(result, list, elementType, true);
        }

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();

            if (typeof(IEnumerable<>).IsAssignableFrom(def) ||
                typeof(IList<>).IsAssignableFrom(def) ||
                typeof(ICollection<>).IsAssignableFrom(def))
            {
                var list = Array.CreateInstance(elementType, capacity);
                return new ArgumentContainer(result, list, elementType, false);
            }
            else if (def == typeof(List<>))
            {
#pragma warning disable IL2075
                // IL2075: The type SHOULD be known :)
                var ctor = type.GetConstructor(Type.EmptyTypes);
#pragma warning restore IL2070
                if (ctor != null)
                {
                    var container = ctor.Invoke(null) as IList;
                    if (container != null)
                    {
                        return new ArgumentContainer(result, container, elementType, false);
                    }
                }
            }
        }

        return null;
    }

    private static Type? GetContainerElementType(Type type)
    {
        if (type == typeof(string))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (IsEnumerable(type))
        {
            var args = type?.GenericTypeArguments;
            if (args?.Length == 1)
            {
                return args[0];
            }
        }

        return null;
    }

    private static bool IsEnumerable(Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
    }
}