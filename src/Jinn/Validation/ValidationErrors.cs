using Jinn.Utilities;

namespace Jinn.Validation;

internal static class ValidationErrors
{
    public static DiagnosticDescriptor JINN1000_RequiredArgumentMissing(Argument argument) =>
        new("JINN1000", Severity.Error, $"The required argument {argument.Name} is missing");

    public static DiagnosticDescriptor JINN1001_RequiredOptionMissing(Option option) =>
        new("JINN1001", Severity.Error, $"The required argument {option.Name} is missing");

    public static DiagnosticDescriptor JINN1002_ArgumentExpectedAnExactAmountOfTokens(ArgumentResult result) =>
        new("JINN1002", Severity.Error,
            $"The argument {result.Argument.Name} expected exactly " +
            $"{result.Arity.Minimum.Pluralize("value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1003_ArgumentExpectedAtLeastAnAmountOfTokens(ArgumentResult result) =>
        new("JINN1003", Severity.Error,
            $"The argument {result.Argument.Name} expected at least " +
            $"{result.Arity.Minimum.Pluralize("value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1004_OptionExpectedAnExactAmountOfTokens(OptionResult result) =>
        new("JINN1004", Severity.Error,
            $"The option {result.Option.Name} expected exactly " +
            $"{result.Arity.Minimum.Pluralize("value", "values")}, " +
            $"got {result.Tokens.Count}");

    public static DiagnosticDescriptor JINN1005_OptionExpectedAtLeastAnAmountOfTokens(OptionResult result) =>
        new("JINN1005", Severity.Error,
            $"The option {result.Option.Name} expected at least " +
            $"{result.Arity.Minimum.Pluralize("value", "values")}, " +
            $"got {result.Tokens.Count}");
}