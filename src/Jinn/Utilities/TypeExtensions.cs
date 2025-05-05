namespace Jinn;

internal static class TypeExtensions
{
    // public static Type? GetContainerElementType(this Type type)
    // {
    //     if (type == typeof(string))
    //     {
    //         return null;
    //     }
    //
    //     if (type.IsArray)
    //     {
    //         return type.GetElementType();
    //     }
    //
    //     if (type.IsEnumerable())
    //     {
    //         var args = type?.GenericTypeArguments;
    //         if (args?.Length == 1)
    //         {
    //             return args[0];
    //         }
    //     }
    //
    //     return null;
    // }
    //
    // private static bool IsEnumerable(this Type type)
    // {
    //     if (type == typeof(string))
    //     {
    //         return false;
    //     }
    //
    //     return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
    // }
}