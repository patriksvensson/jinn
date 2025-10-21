namespace Jinn.Testing;

internal static class StringExtensions
{
    extension(string? value)
    {
        public string NormalizeLineEndings()
        {
            if (value != null)
            {
                value = value.Replace("\r\n", "\n");
                return value.Replace("\r", string.Empty);
            }

            return string.Empty;
        }
    }
}