namespace Jinn;

internal static class StringSplitter
{
    public static IEnumerable<string> Split(string text)
    {
        return Split(new StringReader(text));
    }

    private static IEnumerable<string> Split(StringReader reader)
    {
        while (reader.Peek() != -1)
        {
            var character = (char)reader.Peek();
            switch (character)
            {
                case '\"':
                    yield return ReadQuote(reader);
                    break;
                case ' ':
                    reader.Read();
                    break;
                default:
                    yield return Read(reader);
                    break;
            }
        }
    }

    private static string ReadQuote(StringReader reader)
    {
        var accumulator = new StringBuilder();
        accumulator.Append((char)reader.Read());
        while (reader.Peek() != -1)
        {
            var character = (char)reader.Peek();
            if (character == '\"')
            {
                accumulator.Append((char)reader.Read());
                break;
            }

            reader.Read();
            accumulator.Append(character);
        }

        return accumulator.ToString();
    }

    private static string Read(StringReader reader)
    {
        var accumulator = new StringBuilder();
        accumulator.Append((char)reader.Read());
        while (reader.Peek() != -1)
        {
            if ((char)reader.Peek() == '\"')
            {
                accumulator.Append(ReadQuote(reader));
            }
            else if ((char)reader.Peek() == ' ')
            {
                break;
            }
            else
            {
                accumulator.Append((char)reader.Read());
            }
        }

        return accumulator.ToString();
    }
}