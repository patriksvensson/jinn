namespace Jinn.Exceptions;

public class JinnException : Exception
{
    public JinnException(string message)
        : base(message)
    {
    }
}

public class JinnConfigurationException : JinnException
{
    public JinnConfigurationException(string message)
        : base(message)
    {
    }
}

public sealed class JinnTemplateException : JinnConfigurationException
{
    public required string Template { get; init; }
    public string? Hint { get; set; }
    public int? Position { get; set; }
    public string? Lexeme { get; set; }

    public JinnTemplateException(string message, string? hint = null)
        : base(message)
    {
        Hint = hint;
    }
}