namespace Jinn.Validation;

internal static class ValidationErrors
{
    public static DiagnosticDescriptor JINN1000_RequiredArgumentMissing(Argument argument) =>
        new("JINN1000", Severity.Error, "Required argument missing",
            $"The required argument {argument.Name} is missing");

    public static DiagnosticDescriptor JINN1001_RequiredOptionMissing(Option option) =>
        new("JINN1001", Severity.Error, "Required option missing", $"The required option {option.Name} is missing");

    public static DiagnosticDescriptor JINN1002_ArgumentExpectedAnExactAmountOfTokens(ArgumentResult result) =>
        new("JINN1002", Severity.Error,
            "Not exact argument count",
            $"The argument {result.ArgumentSymbol.Name} expected exactly " +
            $"{Pluralize(result.Arity.Minimum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1003_ArgumentExpectedAtLeastAnAmountOfTokens(ArgumentResult result) =>
        new("JINN1003", Severity.Error,
            "Too few argument values",
            $"The argument {result.ArgumentSymbol.Name} expected at least " +
            $"{Pluralize(result.Arity.Minimum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1004_OptionExpectedAnExactAmountOfTokens(OptionResult result) =>
        new("JINN1004", Severity.Error,
            "Not exact amount of option values",
            $"The option {result.OptionSymbol.Name} expected exactly " +
            $"{Pluralize(result.Arity.Minimum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1005_OptionExpectedAtLeastAnAmountOfTokens(OptionResult result) =>
        new("JINN1005", Severity.Error,
            "Too few option values",
            $"The option {result.OptionSymbol.Name} expected at least " +
            $"{Pluralize(result.Arity.Minimum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1006_TooManyArgumentValues(ArgumentResult result) =>
        new("JINN1006", Severity.Error,
            "Too many argument values",
            $"The argument {result.ArgumentSymbol.Name} expected maximum " +
            $"{Pluralize(result.Arity.Maximum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1007_TooManyOptionValues(OptionResult result) =>
        new("JINN1005", Severity.Error,
            "Too many option values",
            $"The option {result.OptionSymbol.Name} expected maximum " +
            $"{Pluralize(result.Arity.Maximum, "value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1008_UnrecognizedCommandOrArgument(Token token) =>
        new("JINN1008", Severity.Error, "Unrecognized command or argument",
            $"Unrecognized command or argument '{token.Lexeme}'");

    private static string Pluralize(int count, string singular, string plural)
    {
        return count == 1
            ? $"{count} {singular}"
            : $"{count} {plural}";
    }
}