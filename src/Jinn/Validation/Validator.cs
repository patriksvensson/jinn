namespace Jinn.Validation;

internal static class Validator
{
    public static Diagnostics Validate(RootCommandResult result, IReadOnlyList<Token> unmatched)
    {
        var context = new ValidationContext();
        Visitor.Shared.Visit(context, result);

        // Create errors for unmatched tokens
        foreach (var token in unmatched)
        {
            context.Diagnostics.Add(
                token,
                ValidationErrors.JINN1008_UnrecognizedCommandOrArgument(token));
        }

        return context.Diagnostics;
    }
}

file sealed class ValidationContext
{
    public Diagnostics Diagnostics { get; }

    public ValidationContext()
    {
        Diagnostics = new Diagnostics();
    }
}

file sealed class Visitor
{
    public static Visitor Shared { get; } = new();

    public void Visit(ValidationContext context, SymbolResult result)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();

        if (result is CommandResult root)
        {
            VisitCommand(context, root);
        }
        else if (result is ArgumentResult argument)
        {
            VisitArgument(context, argument);
        }
        else if (result is OptionResult option)
        {
            VisitOption(context, option);
        }
    }

    private void VisitCommand(ValidationContext context, CommandResult result)
    {
        // Do we have all required arguments?
        foreach (var argument in result.CommandSymbol.Arguments)
        {
            var argumentResult = result.FindImmediateResult<ArgumentResult>(argument);
            if (argument.IsRequired && argumentResult == null)
            {
                var diagnostic = context.Diagnostics.Add(
                    ValidationErrors.JINN1000_RequiredArgumentMissing(argument));

                result.AddDiagnostic(diagnostic);
            }
        }

        // Do we have all required options?
        foreach (var option in result.CommandSymbol.Options)
        {
            var argumentResult = result.FindImmediateResult<OptionResult>(option);
            if (option.IsRequired && argumentResult == null)
            {
                var diagnostic = context.Diagnostics.Add(
                    ValidationErrors.JINN1001_RequiredOptionMissing(option));

                result.AddDiagnostic(diagnostic);
            }
        }

        // Visit all children
        foreach (var child in result.Children)
        {
            Visit(context, child);
        }
    }

    private void VisitArgument(ValidationContext context, ArgumentResult result)
    {
        // Not enought tokens present?
        if (result.Tokens.Count < result.Arity.Minimum)
        {
            if (result.Arity.Minimum == result.Arity.Maximum)
            {
                // Expected an exact amount of tokens
                var diagnostic = context.Diagnostics.Add(
                    TextSpan.Between(result.Tokens),
                    ValidationErrors.JINN1002_ArgumentExpectedAnExactAmountOfTokens(result));

                result.AddDiagnostic(diagnostic);
            }
            else
            {
                // Expected at least an amount of tokens
                var diagnostic = context.Diagnostics.Add(
                    TextSpan.Between(result.Tokens),
                    ValidationErrors.JINN1003_ArgumentExpectedAtLeastAnAmountOfTokens(result));

                result.AddDiagnostic(diagnostic);
            }
        }

        // Too many tokens present?
        if (result.Tokens.Count > result.Arity.Maximum)
        {
            // Expected at least an amount of tokens
            var diagnostic = context.Diagnostics.Add(
                TextSpan.Between(result.Tokens),
                ValidationErrors.JINN1006_TooManyArgumentValues(result));

            result.AddDiagnostic(diagnostic);
        }
    }

    private void VisitOption(ValidationContext context, OptionResult result)
    {
        // Not enought tokens present?
        if (result.Tokens.Count < result.Arity.Minimum)
        {
            if (result.Arity.Minimum == result.Arity.Maximum)
            {
                // Expected an exact amount of tokens
                context.Diagnostics.Add(
                    TextSpan.Between([result.Token, .. result.Tokens]),
                    ValidationErrors.JINN1004_OptionExpectedAnExactAmountOfTokens(result));
            }
            else
            {
                // Expected at least an amount of tokens
                context.Diagnostics.Add(
                    TextSpan.Between([result.Token, .. result.Tokens]),
                    ValidationErrors.JINN1005_OptionExpectedAtLeastAnAmountOfTokens(result));
            }
        }

        // Too many tokens present?
        if (result.Tokens.Count > result.Arity.Maximum)
        {
            // Expected at least an amount of tokens
            var diagnostic = context.Diagnostics.Add(
                TextSpan.Between(result.Tokens),
                ValidationErrors.JINN1007_TooManyOptionValues(result));

            result.AddDiagnostic(diagnostic);
        }
    }
}