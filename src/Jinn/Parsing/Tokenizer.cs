using System.Diagnostics.CodeAnalysis;

namespace Jinn;

public static class Tokenizer
{
    public static List<Token> Tokenize(
        string args, CommandContainer root)
    {
        return Tokenize(
            StringSplitter.Split(args).ToArray(),
            root);
    }

    public static List<Token> Tokenize(
        string[] args,
        CommandContainer root)
    {
        var currentSymbol = (Symbol)root;
        var knownSymbols = root.GetOwnedSymbols().GetSymbolDictionary();
        var tokens = new List<Token>();

        foreach (var (arg, index) in args.Select((a, b) => (a, b)))
        {
            // Something known?
            if (knownSymbols.TryGetValue(arg, out var symbol))
            {
                if (symbol is Command)
                {
                    tokens.Add(new Token(TokenType.Command, symbol, index, arg));
                    knownSymbols = symbol.GetOwnedSymbols().GetSymbolDictionary();
                    currentSymbol = symbol;
                }
                else if (symbol is Option)
                {
                    tokens.Add(new Token(TokenType.Option, symbol, index, arg));
                }
                else
                {
                    throw new InvalidOperationException("Unhandled symbol");
                }
            }
            else
            {
                if (TrySplitArgumentIntoOption(knownSymbols, arg, out var splitResult))
                {
                    // A valid option in the foo=bar or foo:bar form
                    tokens.AddRange(splitResult);
                }
                else if (TryUnbundleOptions(knownSymbols, arg, index, out var unbundleResult))
                {
                    // A valid option in the foo=bar or foo:bar form
                    tokens.AddRange(unbundleResult);
                }
                else if (tokens.Count > 0 && tokens[^1].Type == TokenType.Option &&
                         tokens[^1].Symbol is Option { Arity.Minimum: > 0 } option)
                {
                    // Previous argument was an option requiring one or more values?
                    tokens.Add(new Token(TokenType.OptionArgument, option, index, arg));
                }
                else
                {
                    // Not known at all, so treat it as an argument
                    tokens.Add(new Token(TokenType.Argument, null, index, arg));
                }
            }
        }

        return tokens;
    }

    private static bool TrySplitArgumentIntoOption(
        IDictionary<string, Symbol> knownSymbols, string arg,
        [NotNullWhen(true)] out List<Token>? result)
    {
        var index = arg.AsSpan().IndexOfAny(':', '=');
        if (index == -1)
        {
            result = null;
            return false;
        }

        var option = arg.Substring(0, index);
        if (!knownSymbols.TryGetValue(option, out var optionSymbol))
        {
            result = null;
            return false;
        }

        result = new List<Token>();
        result.Add(new Token(TokenType.Option, optionSymbol, index, option));

        var value = arg.Substring(index + 1);
        if (value.Length != 0)
        {
            result.Add(new Token(TokenType.OptionArgument, optionSymbol, index, value));
        }

        return true;
    }

    private static bool TryUnbundleOptions(
        IDictionary<string, Symbol> knownSymbols,
        string arg, int position,
        [NotNullWhen(true)] out List<Token>? result)
    {
        if (arg.Length <= 1 || arg[0] != '-')
        {
            result = null;
            return false;
        }

        if (arg[1] == '-')
        {
            result = null;
            return false;
        }

        result = new List<Token>();
        foreach (var (character, index) in arg.Skip(1).Select((a, b) => (a, b + 1)))
        {
            if (knownSymbols.TryGetValue($"-{character}", out var optionSymbol))
            {
                result.Add(new Token(TokenType.Option, optionSymbol, position, $"-{character}"));
            }
            else
            {
                result.Add(new Token(TokenType.Argument, null, position, arg.Substring(index)));
                return true;
            }
        }

        return true;
    }
}

internal sealed class TokenizerContext
{
    private readonly string[] _args;
    private readonly List<Token> _result;

    public TokenizerContext(string[] args)
    {
        _args = args ?? [];
        _result = [];
    }

    public IEnumerable<(string Argument, int Index)> GetArguments()
    {
        return _args.Select((a, b) => (Argument: a, Index: b));
    }
}