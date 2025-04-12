namespace Jinn;

public static class Parser
{
    public static void Parse(Command root, List<Token> tokens)
    {
        var context = new ParserContext(root, tokens);
        Parse(context);
    }

    private static void Parse(ParserContext context)
    {
        while (!context.Reader.IsAtEnd)
        {
            var current = context.Reader.Current;
            if (current.Type == TokenType.Command)
            {
                ParseCommand(context);
            }
            else if (current.Type == TokenType.Argument)
            {
                // Parse argument
                ParseArgument(context);
            }
            else if (current.Type == TokenType.Option)
            {
                // Parse option
                ParseOption(context);
            }
            else
            {
                // Unmatched
                throw new NotImplementedException();
            }
        }
    }

    private static void ParseCommand(ParserContext context)
    {
        if (context.Reader.IsAtEnd)
        {
            return;
        }

        var token = context.Reader.Current;
        var symbol = token.Symbol as Command;
        if (symbol == null)
        {
            throw new InvalidOperationException("Expected to find command");
        }

        context.AddCommand(symbol);
        context.Reader.Consume();

        Parse(context);
    }

    private static void ParseArgument(ParserContext context)
    {
        while (!context.Reader.IsAtEnd)
        {
            var current = context.Reader.Current;
        }
    }

    private static void ParseOption(ParserContext context)
    {
        if (context.Reader.IsAtEnd)
        {
            return;
        }

        var token = context.Reader.Current;
        var symbol = token.Symbol as Option;
        if (symbol == null)
        {
            throw new InvalidOperationException("Expected to find option");
        }

        var result = default(OptionSymbolResult?);
        if (context.Tree.TryGetValue(symbol, out var symbolResult))
        {
            result = symbolResult as OptionSymbolResult;
            if (result == null)
            {
                throw new InvalidOperationException("Unexpected result type. Expected option");
            }
        }
        else
        {
            result = context.AddOption(symbol);
        }

        context.Reader.Consume();

        ParseOptionValues(context, result);
    }

    private static void ParseOptionValues(ParserContext context, OptionSymbolResult optionResult)
    {
        var argument = optionResult.Symbol.Argument;
        var argumentCount = 0;

        while (!context.Reader.IsAtEnd)
        {
            var current = context.Reader.Current;
            if (current.Type != TokenType.OptionArgument &&
                current.Type != TokenType.Argument)
            {
                break;
            }

            // Passed the numbers of wanted arguments?
            if (argumentCount >= argument.Arity.Maximum ||
                argument.Arity.Maximum == 0)
            {
                break;
            }

            // Is this a boolean?
            if (argument.ValueType == typeof(bool))
            {
                // If we fail to parse the next value,
                // just assume it's not part of the option since
                // booleans doesn't require a value.
                if (!bool.TryParse(current.Value, out _))
                {
                    break;
                }
            }

            if (!(context.Tree.TryGetValue(argument, out var result) &&
                  result is ArgumentSymbolResult argumentResult))
            {
                argumentResult = context.AddOptionArgument(optionResult, argument);
            }

            // Add the current token to the argument and option
            argumentResult.AddToken(context.Reader.Current);
            optionResult.AddToken(context.Reader.Current);

            argumentCount++;
            context.Reader.Consume();
        }

        // No parsed argument
        if (argumentCount == 0 && !context.Tree.ContainsKey(argument))
        {
            context.AddOptionArgument(optionResult, argument);
        }
    }
}