namespace Jinn;

internal static class Tokenizer
{
    public static TokenizeResult Tokenize(IEnumerable<string> args, Command root)
    {
        return Tokenize(args, root.CreateSymbol());
    }

    private static TokenizeResult Tokenize(IEnumerable<string> args, CommandSymbol root)
    {
        var context = new TokenizerContext(root, args);

        while (context.Read(out var arg))
        {
            // Have we encountered a double dash (--)?
            // In that case, treat everything else as an argument
            if (context.HasEncounteredDoubleDash)
            {
                context.AddToken(TokenType.Argument, null, arg);
                continue;
            }

            // Is this a double dash?
            if (arg == "--")
            {
                context.AddToken(TokenType.DoubleDash, null, arg);
                continue;
            }

            // Is the current argument a known symbol?
            if (context.TryGetSymbol(arg, out var symbol))
            {
                switch (symbol)
                {
                    case CommandSymbol command:
                        context.AddToken(TokenType.Command, command, arg);
                        context.SetCurrentCommand(command);
                        continue;

                    case OptionSymbol:
                        context.AddToken(TokenType.Option, symbol, arg);
                        continue;

                    default:
                        throw new InvalidOperationException("Unhandled symbol");
                }
            }

            // A valid option in the foo=bar or foo:bar form?
            if (TrySplitArgumentIntoTokens(context, arg))
            {
                continue;
            }

            // A valid option in -abc form that needs to be unbundled?
            if (TryUnbundleOptionsNew(context, arg))
            {
                continue;
            }

            // Previous argument was an option requiring one or more values?
            if (TryAddOptionValue(context, arg))
            {
                continue;
            }

            // Not known at all, so treat it as an argument
            context.AddToken(TokenType.Argument, null, arg);
        }

        return context.CreateResult();
    }

    private static bool TrySplitArgumentIntoTokens(
        TokenizerContext context, string arg)
    {
        var index = arg.AsSpan().IndexOfAny(':', '=');
        if (index == -1)
        {
            return false;
        }

        var option = arg.Substring(0, index);
        if (!context.TryGetSymbol<OptionSymbol>(option, out var optionSymbol))
        {
            return false;
        }

        context.AddToken(TokenType.Option, optionSymbol, option);

        var value = arg[(index + 1)..];
        if (value.Length != 0)
        {
            context.AddToken(TokenType.Argument, optionSymbol, value);
        }

        return true;
    }

    private static bool TryUnbundleOptionsNew(
        TokenizerContext context, string arg)
    {
        if (arg.Length <= 1 || arg[0] != '-')
        {
            return false;
        }

        if (arg[1] == '-')
        {
            return false;
        }

        foreach (var (character, index) in arg.Skip(1).Select((a, b) => (a, b + 1)))
        {
            var optionName = $"-{character}";
            if (!context.TryGetSymbol<OptionSymbol>(optionName, out var optionSymbol))
            {
                var argument = arg[index..];
                var argumentSpan = new TextSpan(context.Position + index, argument.Length);
                context.AddToken(TokenType.Argument, null, argument, argumentSpan);
                return true;
            }

            var optionSpan = new TextSpan(context.Position + index, 1);
            context.AddToken(TokenType.Option, optionSymbol, optionName, optionSpan);
        }

        return true;
    }

    private static bool TryAddOptionValue(TokenizerContext context, string arg)
    {
        if (context.Tokens.Count == 0 ||
            context.Tokens[^1].TokenType != TokenType.Option ||
            context.Tokens[^1].Symbol is not OptionSymbol { Argument.Arity.Minimum: > 0 } option)
        {
            return false;
        }

        context.AddToken(TokenType.Argument, option, arg);
        return true;
    }
}