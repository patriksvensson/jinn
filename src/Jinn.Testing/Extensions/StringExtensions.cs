namespace Jinn.Testing;

internal static class StringExtensions
{
    public static string NormalizeLineEndings(this string value)
    {
        if (value != null)
        {
            value = value.Replace("\r\n", "\n");
            return value.Replace("\r", string.Empty);
        }

        return string.Empty;
    }
}