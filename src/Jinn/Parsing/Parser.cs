namespace Jinn;

internal static class Parser
{
    public static ParseResult Parse(IEnumerable<string> args, Command root)
    {
        var tokenizationResult = Tokenizer.Tokenize(args, root);
        var result = Parse(new ParserContext(tokenizationResult));

        return result;
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
        var command = context.ExpectCurrentSymbol<CommandSymbol>();
        command.Result ??= new CommandResult(command);

        context.AddCommand(command);
        context.ConsumeToken();

        // Continue and parse children of the command
        Parse(context);
    }

    private static void ParseArgument(ParserContext context)
    {
        while (!context.IsAtEnd && context.CurrentToken.TokenType == TokenType.Argument)
        {
            while (context.CurrentCommand.HasArguments &&
                   context.CurrentArgumentIndex < context.CurrentCommand.Arguments.Count)
            {
                // Current argument still accept values?
                var argument = context.CurrentCommand.Arguments[context.CurrentArgumentIndex];
                if (context.CurrentArgumentCount < argument.Arity.Maximum)
                {
                    context.CurrentToken.Symbol ??= argument;

                    argument.Parent ??= context.CurrentCommand;
                    argument.Result ??= new ArgumentResult(argument);
                    argument.Result.AddToken(context.CurrentToken);

                    context.CurrentCommand.Result ??= new CommandResult(context.CurrentCommand);
                    context.CurrentCommand.Result.AddToken(context.CurrentToken);

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
        var option = context.ExpectCurrentSymbol<OptionSymbol>();

        // Try get the result for the symbol
        // TODO: Set current token as identifier
        option.Parent ??= context.CurrentCommand;
        option.Result ??= new OptionResult(option, context.CurrentToken);

        context.ConsumeToken();

        // Parse option values
        ParseOptionValues(context, option);
    }

    private static void ParseOptionValues(ParserContext context, OptionSymbol option)
    {
        var argumentCount = 0;

        while (context.IsMatch(TokenType.Argument))
        {
            if (argumentCount >= option.Arity.Maximum)
            {
                if (argumentCount > 0)
                {
                    break;
                }

                // This option want no values
                if (option.Arity.Maximum == 0)
                {
                    break;
                }
            }
            else if (option.IsBoolean() && !bool.TryParse(context.CurrentToken.Value, out _))
            {
                // The option value was not a valid boolean identifier.
                // Since we allow booleans to act as a flag, simply skip it.
                break;
            }

            Debug.Assert(option.Result != null, "Option result should not be null when parsing value");
            option.Result.AddToken(context.CurrentToken);
            argumentCount++;

            context.ConsumeToken();
        }
    }
}