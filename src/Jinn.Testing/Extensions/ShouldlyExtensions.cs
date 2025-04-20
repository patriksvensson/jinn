namespace Jinn.Testing;

public static class ShouldlyExtensions
{
    public static T And<T>(this T obj)
    {
        return obj;
    }

    public static T And<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}