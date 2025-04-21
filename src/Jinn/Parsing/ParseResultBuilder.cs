namespace Jinn;

internal static class ParseResultBuilder
{
    public static ParseResult Build(SyntaxTree tree)
    {
        var ctx = new Context(tree);
        Visitor.Shared.VisitCommand(tree.Root, ctx);
        return ctx.CreateResult();
    }

    private sealed class Context
    {
        private readonly SyntaxTree _tree;

        public RootCommandResult? RootCommand { get; set; }
        public CommandResult? CurrentCommand { get; set; }
        public Dictionary<Symbol, SymbolResult> Lookup { get; } = [];

        public Context(SyntaxTree tree)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        }

        public ParseResult CreateResult()
        {
            return new ParseResult
            {
                ParsedCommand = RootCommand ?? throw new InvalidOperationException("Could not find root command"),
                Configuration = _tree.Configuration,
                Lookup = Lookup,
                Unmatched = _tree.Unmatched,
                Tokens = _tree.Tokens,
            };
        }
    }

    private sealed class Visitor : SyntaxVisitor<Context>
    {
        public static Visitor Shared { get; } = new();

        public override void VisitCommand(CommandSyntax syntax, Context context)
        {
            if (syntax.Parent == null)
            {
                if (syntax.Command is not RootCommand command)
                {
                    throw new InvalidOperationException("Expected root command");
                }

                // Set the current command
                context.RootCommand = new RootCommandResult(command);
                context.CurrentCommand = context.RootCommand;
            }
            else
            {
                var result = new SubCommandResult(syntax.Command, syntax.Token, context.CurrentCommand);
                context.CurrentCommand?.AddChild(result);
                context.Lookup[result.Symbol] = result;

                // Set the current command
                context.CurrentCommand = result;
            }

            // Visit children
            Visit(syntax.Children, context);
        }

        public override void VisitArgument(ArgumentSyntax syntax, Context context)
        {
            context.Lookup.TryGetValue(syntax.Argument, out var result);

            if (result is not ArgumentResult argumentResult)
            {
                argumentResult = new ArgumentResult(syntax.Argument, context.CurrentCommand);

                context.CurrentCommand?.AddChild(argumentResult);
                context.Lookup[argumentResult.Argument] = argumentResult;
            }

            syntax.Token.Symbol ??= argumentResult.Argument;
            argumentResult.AddToken(syntax.Token);
            context.CurrentCommand?.AddToken(syntax.Token);
        }

        public override void VisitOption(OptionSyntax syntax, Context context)
        {
            context.Lookup.TryGetValue(syntax.Option, out var result);

            if (result is not OptionResult)
            {
                var optionResult = new OptionResult(
                    syntax.Option,
                    syntax.Token,
                    context.CurrentCommand);

                context.CurrentCommand?.AddChild(optionResult);
                context.Lookup[optionResult.Option] = optionResult;
            }

            // Visit children
            Visit(syntax.Children, context);
        }

        public override void VisitOptionArgument(OptionArgumentSyntax syntax, Context context)
        {
            context.Lookup.TryGetValue(
                syntax.ParentOption.Option,
                out var optionResult);

            if (optionResult is not OptionResult)
            {
                return;
            }

            var argument = syntax.Argument;

            if (!context.Lookup.TryGetValue(argument, out var argumentResult))
            {
                argumentResult =
                    new ArgumentResult(
                        syntax.Argument,
                        optionResult);
                optionResult.AddChild(argumentResult);
                context.Lookup[argument] = argumentResult;
            }

            argumentResult.AddToken(syntax.Token);
            optionResult.AddToken(syntax.Token);
        }
    }
}