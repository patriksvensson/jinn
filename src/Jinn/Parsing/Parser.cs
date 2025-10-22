namespace Jinn;

internal static class Parser
{
    public static ParseResult Parse(
        Configuration configuration,
        IEnumerable<string> args,
        Command command)
    {
        var tokens = Tokenizer.Tokenize(args, command);
        var context = new ParserContext(command, tokens);
        var syntax = Parse(context, command);

        // The synthetic tokens are not needed anymore,
        // so let's remove them from the collections.
        tokens.RemoveAll(token => token.IsSynthetic);
        context.Unmatched.RemoveAll(token => token.IsSynthetic);

        return ParseResultBuilder.Build(
            new SyntaxTree
            {
                Root = syntax,
                Configuration = configuration,
                Unmatched = context.Unmatched,
                Tokens = tokens,
            });
    }

    private static CommandSyntax Parse(ParserContext context, Command command)
    {
        if (command is RootCommand)
        {
            context.CurrentToken.Symbol = command;
        }

        var syntax = new CommandSyntax(command, context.CurrentToken, null);
        ParseCommandChildren(context, syntax);
        return syntax;
    }

    private static void ParseCommandChildren(ParserContext context, CommandSyntax parent)
    {
        while (!context.IsAtEnd)
        {
            var current = context.CurrentToken;

            switch (current.Kind)
            {
                case TokenKind.Executable:
                    context.ConsumeToken();
                    break;
                case TokenKind.Command:
                    ParseCommand(context, parent);
                    break;
                case TokenKind.Argument:
                    ParseArgument(context, parent);
                    break;
                case TokenKind.Option:
                    ParseOption(context, parent);
                    break;
                default:
                    context.AddCurrentTokenToUnmatched();
                    context.ConsumeToken();
                    break;
            }
        }
    }

    private static void ParseCommand(ParserContext context, CommandSyntax parent)
    {
        // Get the symbol from the current token,
        // which we expect to be a command (due to the token type)
        var command = context.ExpectCurrentSymbol<Command>();

        var syntax = new CommandSyntax(command, context.CurrentToken, parent);
        parent.AddChild(syntax);

        context.SetCurrentCommand(command);
        context.ConsumeToken();

        // Continue and parse children of the command
        ParseCommandChildren(context, syntax);
    }

    private static void ParseArgument(ParserContext context, CommandSyntax parent)
    {
        while (context is { IsAtEnd: false, CurrentToken.Kind: TokenKind.Argument })
        {
            while (context.CurrentCommand.HasArguments &&
                   context.CurrentArgumentIndex < context.CurrentCommand.Arguments.Count)
            {
                // Current argument still accept values?
                var argument = context.CurrentCommand.Arguments[context.CurrentArgumentIndex];
                if (context.CurrentArgumentCount < argument.Arity.Maximum)
                {
                    // Assign a symbol to the current token
                    context.CurrentToken.Symbol ??= argument;

                    // Create a syntax node and add it to the command
                    var syntax = new ArgumentSyntax(argument, context.CurrentToken, parent);
                    parent.AddChild(syntax);

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

    private static void ParseOption(ParserContext context, CommandSyntax parent)
    {
        // Get the symbol from the current token,
        // which we expect to be an option (due to the token type)
        var option = context.ExpectCurrentSymbol<Option>();

        // Create a syntax node and add it to the command
        var syntax = new OptionSyntax(option, context.CurrentToken, parent);
        parent.AddChild(syntax);

        // Consume the token
        context.ConsumeToken();

        // Parse option values
        ParseOptionValues(context, syntax);
    }

    private static void ParseOptionValues(ParserContext context, OptionSyntax parent)
    {
        var argumentCount = 0;

        while (context.IsMatch(TokenKind.Argument))
        {
            if (argumentCount >= parent.Option.Argument.Arity.Maximum)
            {
                if (argumentCount > 0)
                {
                    break;
                }

                // This option want no values
                if (parent.Option.Argument.Arity.Maximum == 0)
                {
                    break;
                }
            }
            else if (parent.Option.Argument.IsBoolean() && !bool.TryParse(context.CurrentToken.Lexeme, out _))
            {
                // The option value was not a valid boolean identifier.
                // Since we allow booleans to act as a flag, simply skip it.
                break;
            }

            // Create a syntax node and add it to the option
            var syntax = new OptionArgumentSyntax(parent.Option.Argument, context.CurrentToken, parent);
            parent.AddChild(syntax);

            argumentCount++;
            context.ConsumeToken();
        }
    }
}