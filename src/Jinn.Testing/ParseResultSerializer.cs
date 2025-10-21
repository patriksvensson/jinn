using System.Runtime.CompilerServices;

namespace Jinn.Testing;

[Flags]
public enum ParseResultParts
{
    None = 0,
    Tokens = 1 << 0,
    Unmatched = 1 << 1,
    Parsed = 1 << 2,
    All = Tokens | Unmatched | Parsed,
}

public sealed class ParseResultSerializerOptions
{
    public ParseResultParts Parts { get; init; } = ParseResultParts.All;
    public bool ExcludeExecutable { get; init; } = true;
}

public static class ParseResultSerializer
{
    public static string Serialize(
        ParseResult result,
        ParseResultSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        options ??= new ParseResultSerializerOptions();

        var output = new Utf8StringWriter();
        using (var writer = XmlWriterEx.Create(output))
        {
            var context = new Context { Options = options, Result = result, Writer = writer };

            // Write
            writer.WriteElement("ParseResult", () =>
            {
                if (options.Parts.HasFlag(ParseResultParts.Parsed))
                {
                    writer.WriteElement("ParsedCommand", () =>
                    {
                        Visitor.Shared.Visit(result.Root, context);
                    });
                }

                if (options.Parts.HasFlag(ParseResultParts.Tokens))
                {
                    writer.WriteElement("Tokens", () =>
                    {
                        Visitor.Shared.VisitTokens(result.Tokens, context);
                    });
                }

                if (options.Parts.HasFlag(ParseResultParts.Unmatched))
                {
                    writer.WriteElement("Unmatched", () =>
                    {
                        Visitor.Shared.VisitTokens(result.Unmatched, context);
                    });
                }
            });
        }

        return output.ToString();
    }
}

file sealed class Context
{
    public required ParseResult Result { get; init; }
    public required ParseResultSerializerOptions Options { get; init; }
    public required XmlWriterEx Writer { get; init; }
}

file sealed class Visitor
{
    public static Visitor Shared { get; } = new();

    public void Visit(SymbolResult result, Context context)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();

        if (result is RootCommandResult root)
        {
            VisitRootCommand(root, context);
        }
        else if (result is SubCommandResult command)
        {
            VisitSubCommand(command, context);
        }
        else if (result is ArgumentResult argument)
        {
            VisitArgument(argument, context);
        }
        else if (result is OptionResult option)
        {
            VisitOption(option, context);
        }
        else
        {
            throw new InvalidOperationException("Unknown result type");
        }
    }

    public void VisitRootCommand(RootCommandResult result, Context context)
    {
        context.Writer.WriteElement("RootCommand", () =>
        {
            if (!context.Options.ExcludeExecutable)
            {
                context.Writer.WriteAttribute("Name", result.CommandSymbol.Name);
            }

            foreach (var child in result.Children)
            {
                Visit(child, context);
            }
        });
    }

    public void VisitSubCommand(SubCommandResult result, Context context)
    {
        context.Writer.WriteElement("Command", () =>
        {
            context.Writer.WriteAttribute("Name", result.Token.Lexeme);

            foreach (var child in result.Children)
            {
                Visit(child, context);
            }
        });
    }

    public void VisitArgument(ArgumentResult result, Context context)
    {
        context.Writer.WriteElement("Argument", () =>
        {
            context.Writer.WriteAttribute("Name", result.ArgumentSymbol.Name);
            VisitTokens(result.Tokens, context);
        });
    }

    public void VisitOption(OptionResult result, Context context)
    {
        context.Writer.WriteElement("Option", () =>
        {
            context.Writer.WriteAttribute("Name", result.Token.Lexeme);

            VisitTokens(result.Tokens, context);
        });
    }

    public void VisitTokens(IReadOnlyList<Token> tokens, Context context)
    {
        foreach (var token in tokens)
        {
            VisitToken(token, context);
        }
    }

    public void VisitToken(Token token, Context context)
    {
        context.Writer.WriteElement("Token", () =>
        {
            context.Writer.WriteAttribute("Lexeme", token.Lexeme);
            context.Writer.WriteAttribute("Kind", token.Kind);

            // Calculate the span
            if (token.Span != null)
            {
                context.Writer.WriteAttribute("Span", token.Span.Value);
            }
        });
    }
}