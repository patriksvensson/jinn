namespace Jinn.Utilities;

internal static class IntExtensions
{
    public static string Pluralize(this int count, string singular, string plural)
    {
        return count == 1
            ? $"{count} {singular}"
            : $"{count} {plural}";
    }
}