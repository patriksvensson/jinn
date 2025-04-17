namespace Jinn;

public static class Parser
{
    public static ParseResult Parse(IEnumerable<string> args, Command root)
    {
        var tokenizationResult = Tokenizer.Tokenize(args, root);
        return Parse(new ParserContext(tokenizationResult));
    }

    private static ParseResult Parse(ParserContext context)
    {
        while (!context.IsAtEnd)
        {
            var current = context.CurrentToken;

            switch (current.TokenType)
            {
                case TokenType.Command:
                    ParseCommand(context);
                    break;
                case TokenType.Argument:
                    ParseArgument(context);
                    break;
                case TokenType.Option:
                    ParseOption(context);
                    break;
                default:
                    context.AddCurrentTokenToUnmatched();
                    context.ConsumeToken();
                    break;
            }
        }

        return context.CreateResult();
    }

    private static void ParseCommand(ParserContext context)
    {
        // Get the symbol from the current token,
        // which we expect to be a command (due to the token type)
        var command = context.Expect<CommandSymbol>();

        context.AddCommand(command);
        context.ConsumeToken();

        // Continue and parse children of the command
        Parse(context);
    }

    private static void ParseArgument(ParserContext context)
    {
        while (!context.IsAtEnd && context.CurrentToken.TokenType == TokenType.Argument)
        {
            while (context.CurrentCommand.Command.HasArguments &&
                   context.CurrentArgumentIndex < context.CurrentCommand.Command.Arguments.Count)
            {
                // Current argument still accept values?
                var argument = context.CurrentCommand.Command.Arguments[context.CurrentArgumentIndex];
                if (context.CurrentArgumentCount < argument.Arity.Maximum)
                {
                    context.CurrentToken.Symbol ??= argument;

                    if (!context.Tree.TryGetResult<ArgumentResult>(argument, out var argumentResult))
                    {
                        argumentResult = new ArgumentResult(argument, context.Tree, context.CurrentCommand);
                        context.Tree.Add(argument, argumentResult);
                    }

                    argumentResult.AddToken(context.CurrentToken);
                    context.CurrentCommand.AddToken(context.CurrentToken);

                    context.CurrentArgumentCount++;
                    context.ConsumeToken();

                    break;
                }

                // Move on to the next argument
                context.CurrentArgumentIndex++;
                context.CurrentArgumentCount = 0;
            }

            if (context.CurrentArgumentCount == 0)
            {
                context.AddCurrentTokenToUnmatched();
                context.ConsumeToken();
            }
        }
    }

    private static void ParseOption(ParserContext context)
    {
        // Get the symbol from the current token,
        // which we expect to be an option (due to the token type)
        var optionSymbol = context.Expect<OptionSymbol>();

        // Try get the result for the symbol
        if (!context.Tree.TryGetResult<OptionResult>(optionSymbol, out var optionResult))
        {
            optionResult = new OptionResult(optionSymbol, context.CurrentToken, context.Tree, context.CurrentCommand);
            context.Tree.Add(optionSymbol, optionResult);
        }

        context.ConsumeToken();

        // Parse option values
        ParseOptionValues(context, optionResult);
    }

    private static void ParseOptionValues(ParserContext context, OptionResult optionResult)
    {
        var argument = optionResult.Option.Argument;
        var argumentCount = 0;

        while (context.IsMatch(TokenType.Argument))
        {
            if (argumentCount >= argument.Arity.Maximum)
            {
                if (argumentCount > 0)
                {
                    break;
                }

                // This option want no values
                if (argument.Arity.Maximum == 0)
                {
                    break;
                }
            }
            else if (argument.IsBoolean() && !bool.TryParse(context.CurrentToken.Value, out _))
            {
                // The option value was not a valid boolean identifier.
                // Since we allow booleans to act as a flag, simply skip it.
                break;
            }

            if (!context.Tree.TryGetResult<ArgumentResult>(argument, out var argumentResult))
            {
                argumentResult = new ArgumentResult(argument, context.Tree, optionResult);
                context.Tree.Add(argument, argumentResult);
            }

            argumentResult.AddToken(context.CurrentToken);
            optionResult.AddToken(context.CurrentToken);

            argumentCount++;

            context.ConsumeToken();
        }

        // No arguments found? Still create a result for it
        if (argumentCount == 0 && !context.Tree.ContainsKey(argument))
        {
            var argumentResult = new ArgumentResult(argument, context.Tree, optionResult);
            context.Tree.Add(argument, argumentResult);
        }
    }
}