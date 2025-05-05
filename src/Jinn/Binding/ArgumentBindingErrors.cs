namespace Jinn.Binding;

internal static class ArgumentBindingErrors
{
    public static DiagnosticDescriptor JINN2000_CannotFindConverter(Argument argument) =>
        new("JINN2000", Severity.Error, "Cannot find converter",
            $"Cannot find converter for argument {argument.Name}");

    public static DiagnosticDescriptor JINN2001_CannotConvertArgument(Argument argument, Token token) =>
        new("JINN2001", Severity.Error, "Cannot convert argument",
            $"Cannot bind argument {argument.Name} with value {token.Lexeme}");

    public static DiagnosticDescriptor JINN2002_CustomConverterNeeded(Argument argument) =>
        new("JINN2001", Severity.Error, "A custom converter is needed",
            $"Cannot bind argument {argument.Name}. A custom converter is needed");
}