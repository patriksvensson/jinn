using System.Runtime.CompilerServices;

namespace Jinn.Testing;

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
    public ParseResultParts Parts { get; set; } = ParseResultParts.All;
    public bool ExcludeExecutable { get; set; } = true;
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
            // Calculate the token offset
            var offset = 0;
            if (options.ExcludeExecutable)
            {
                offset = result.Tokens[0].Lexeme.Length + 1;
            }

            var context = new Context { Options = options, Result = result, Writer = writer, Offset = offset };

            // Write
            writer.WriteElement("ParseResult", () =>
            {
                if (options.Parts.HasFlag(ParseResultParts.Parsed))
                {
                    writer.WriteElement("ParsedCommand", () =>
                    {
                        Visitor.Shared.Visit(result.ParsedCommand, context);
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
    public int Offset { get; init; }
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

    public void VisitRootCommand(RootCommandResult root, Context context)
    {
        if (context.Options.ExcludeExecutable)
        {
            foreach (var child in root.Children)
            {
                Visit(child, context);
            }
        }
        else
        {
            context.Writer.WriteElement("RootCommand", () =>
            {
                context.Writer.WriteAttribute("Name", root.Command.Name);

                foreach (var child in root.Children)
                {
                    Visit(child, context);
                }
            });
        }
    }

    public void VisitSubCommand(SubCommandResult command, Context context)
    {
        context.Writer.WriteElement("Command", () =>
        {
            context.Writer.WriteAttribute("Name", command.Token.Lexeme);

            foreach (var child in command.Children)
            {
                Visit(child, context);
            }
        });
    }

    public void VisitArgument(ArgumentResult argument, Context context)
    {
        context.Writer.WriteElement("Argument", () =>
        {
            VisitTokens(argument.Tokens, context);
        });
    }

    public void VisitOption(OptionResult option, Context context)
    {
        context.Writer.WriteElement("Option", () =>
        {
            context.Writer.WriteAttribute("Name", option.Token.Lexeme);

            VisitTokens(option.Tokens, context);
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
        if (token.Span.Position < context.Offset)
        {
            return;
        }

        context.Writer.WriteElement("Token", () =>
        {
            context.Writer.WriteAttribute("Lexeme", token.Lexeme);
            context.Writer.WriteAttribute("Kind", token.Kind);

            // Calculate the span
            var span = new TextSpan(token.Span.Position - context.Offset, token.Span.Length);
            context.Writer.WriteAttribute("Span", span);
        });
    }
}