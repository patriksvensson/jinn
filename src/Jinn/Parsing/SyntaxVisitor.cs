namespace Jinn;

internal abstract class SyntaxVisitor<TContext>
{
    public abstract void VisitCommand(CommandSyntax syntax, TContext context);
    public abstract void VisitArgument(ArgumentSyntax syntax, TContext context);
    public abstract void VisitOption(OptionSyntax syntax, TContext context);
    public abstract void VisitOptionArgument(OptionArgumentSyntax syntax, TContext context);

    [DebuggerStepThrough]
    protected void Visit(IEnumerable<Syntax> syntaxes, TContext context)
    {
        foreach (var node in syntaxes)
        {
            Visit(node, context);
        }
    }

    [DebuggerStepThrough]
    private void Visit(Syntax? syntax, TContext context)
    {
        if (syntax == null)
        {
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        syntax.Accept(this, context);
    }
}