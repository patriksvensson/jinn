namespace Jinn;

internal static class StringExtensions
{
    extension(string text)
    {
        public int OrdinalIndexOf(char token)
        {
            return text.IndexOf(token, StringComparison.Ordinal);
        }
    }
}