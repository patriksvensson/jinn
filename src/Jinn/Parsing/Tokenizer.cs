namespace Jinn;

public static class Tokenizer
{
    public static IReadOnlyList<Token> Tokenize(string args, RootCommand root)
    {
        return Tokenize(StringSplitter.Split(args), root.CreateSymbol());
    }

    public static IReadOnlyList<Token> Tokenize(IEnumerable<string> args, RootCommand root)
    {
        return Tokenize(args, root.CreateSymbol());
    }

    private static IReadOnlyList<Token> Tokenize(IEnumerable<string> args, CommandSymbol root)
    {
        var context = new TokenizerContext(root, args);

        while (context.Read(out var arg))
        {
            // Is the current argument a known symbol?
            if (context.TryGetSymbol(arg, out var symbol))
            {
                if (symbol is CommandSymbol command)
                {
                    context.AddToken(TokenType.Command, command, arg);
                    context.SetCurrentCommand(command);
                    continue;
                }

                if (symbol is OptionSymbol)
                {
                    context.AddToken(TokenType.Option, symbol, arg);
                    continue;
                }

                throw new InvalidOperationException("Unhandled symbol");
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

        return context.Tokens;
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

        var value = arg.Substring(index + 1);
        if (value.Length != 0)
        {
            context.AddToken(TokenType.OptionArgument, optionSymbol, value);
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
            if (context.TryGetSymbol<OptionSymbol>(optionName, out var optionSymbol))
            {
                context.AddToken(TokenType.Option, optionSymbol, optionName);
                continue;
            }

            context.AddToken(TokenType.Argument, null, arg.Substring(index));
            return true;
        }

        return true;
    }

    private static bool TryAddOptionValue(TokenizerContext context, string arg)
    {
        if (context.Tokens.Count > 0 &&
            context.Tokens[^1].Type == TokenType.Option &&
            context.Tokens[^1].Symbol is OptionSymbol { Argument.Arity.Minimum: > 0 } option)
        {
            context.AddToken(TokenType.OptionArgument, option, arg);
            return true;
        }

        return false;
    }
}