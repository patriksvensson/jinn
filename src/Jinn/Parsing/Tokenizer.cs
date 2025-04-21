namespace Jinn;

internal static class Tokenizer
{
    public static IReadOnlyList<Token> Tokenize(IEnumerable<string> args, Command root)
    {
        var context = new TokenizerContext(root, NormalizeArguments(args, root));

        while (context.Read(out var arg))
        {
            // Have we encountered a double dash (--)?
            // In that case, treat everything else as an argument
            if (context.HasEncounteredDoubleDash)
            {
                context.AddToken(TokenKind.Argument, null, arg);
                continue;
            }

            // Encountered a double dash?
            if (arg == "--")
            {
                context.AddToken(TokenKind.DoubleDash, null, arg);
                continue;
            }

            // Is the current argument a known symbol?
            if (context.TryGetSymbol(arg, out var symbol))
            {
                switch (symbol)
                {
                    case Command command:
                        context.AddToken(TokenKind.Command, command, arg);
                        context.SetCurrentCommand(command);
                        continue;

                    case Option:
                        context.AddToken(TokenKind.Option, symbol, arg);
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
            context.AddToken(TokenKind.Argument, null, arg);
        }

        return context.Tokens;
    }

    private static List<string> NormalizeArguments(IEnumerable<string> args, Command command)
    {
        var result = new List<string>(args);

        if (result.Count > 0)
        {
            if (result[0] == command.Name)
            {
                return result;
            }
        }

        result.Insert(0, command.Name);
        return result;
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
        if (!context.TryGetSymbol<Option>(option, out var optionSymbol))
        {
            return false;
        }

        context.AddToken(TokenKind.Option, optionSymbol, option);

        var value = arg[(index + 1)..];
        if (value.Length != 0)
        {
            context.AddToken(TokenKind.Argument, optionSymbol, value);
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
            if (!context.TryGetSymbol<Option>(optionName, out var optionSymbol))
            {
                var argument = arg[index..];
                var argumentSpan = new TextSpan(context.Position + index, argument.Length);
                context.AddToken(TokenKind.Argument, null, argument, argumentSpan);
                return true;
            }

            var optionSpan = new TextSpan(context.Position + index, 1);
            context.AddToken(TokenKind.Option, optionSymbol, optionName, optionSpan);
        }

        return true;
    }

    private static bool TryAddOptionValue(TokenizerContext context, string arg)
    {
        if (context.Tokens.Count == 0 ||
            context.Tokens[^1].Kind != TokenKind.Option ||
            context.Tokens[^1].Symbol is not Option { Argument.Arity.Minimum: > 0 } option)
        {
            return false;
        }

        context.AddToken(TokenKind.Argument, option, arg);
        return true;
    }
}