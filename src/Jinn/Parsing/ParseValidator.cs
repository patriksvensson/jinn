namespace Jinn;

internal static class ParseValidator
{
    public static List<ParseError> Validate(CommandSymbol command)
    {
        var context = new ValidationContext();

        var currentCommand = command;
        while (currentCommand != null)
        {
            foreach (var option in currentCommand.Options)
            {
                if (option.Result == null && option.IsRequired)
                {
                    context.Errors.Add(new ParseError(
                        $"The option '{option.Aliases.First()}' is required"));
                }

                if (option.Result != null)
                {
                    var arityError = Arity.Validate(option) switch
                    {
                        null => null,
                        ArityViolation.Missing => "Required argument missing for option",
                        ArityViolation.TooMany => "Option expects a single argument",
                        _ => throw new ArgumentOutOfRangeException("Unknown violation"),
                    };

                    if (arityError != null)
                    {
                        context.Errors.Add(
                            new ParseError(
                                arityError, option.Result.Identifier.Span));
                    }
                }
            }

            foreach (var argument in currentCommand.Arguments)
            {
                if (argument.Result == null && argument.IsRequired)
                {
                    context.Errors.Add(new ParseError(
                        $"The argument '{argument.Name}' is required"));
                }

                if (argument.Result != null)
                {
                    var arityError = Arity.Validate(argument) switch
                    {
                        null => null,
                        ArityViolation.Missing => "Required argument missing",
                        ArityViolation.TooMany => "Command expects a single argument",
                        _ => throw new ArgumentOutOfRangeException("Unknown violation"),
                    };

                    if (arityError != null)
                    {
                        context.Errors.Add(
                            new ParseError(arityError));
                    }
                }
            }

            currentCommand = currentCommand.Parent as CommandSymbol;
        }

        return context.Errors;
    }
}

internal sealed class ValidationContext
{
    public List<ParseError> Errors { get; } = [];
}