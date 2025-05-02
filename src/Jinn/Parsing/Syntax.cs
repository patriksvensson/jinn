namespace Jinn;

internal abstract class Syntax
{
    private readonly List<Syntax> _children;

    public Token Token { get; }
    public Syntax? Parent { get; }
    public IReadOnlyList<Syntax> Children => _children;

    protected Syntax(Token token, Syntax? parent)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Parent = parent;
        _children = [];
    }

    public abstract void Accept<TContext>(
        SyntaxVisitor<TContext> visitor,
        TContext context);

    public void AddChild(Syntax child)
    {
        _children.Add(child);
    }
}

internal sealed class CommandSyntax : Syntax
{
    public Command Command { get; }

    public CommandSyntax(Command command, Token token, Syntax? parent)
        : base(token, parent)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public override void Accept<TContext>(SyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitCommand(this, context);
    }
}

internal sealed class ArgumentSyntax : Syntax
{
    public Argument Argument { get; }
    public CommandSyntax ParentCommand { get; }

    public ArgumentSyntax(Argument argument, Token token, CommandSyntax parent)
        : base(token, parent)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        ParentCommand = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public override void Accept<TContext>(SyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitArgument(this, context);
    }
}

internal sealed class OptionSyntax : Syntax
{
    public Option Option { get; }
    public CommandSyntax ParentCommand { get; }

    public OptionSyntax(Option option, Token token, CommandSyntax parent)
        : base(token, parent)
    {
        Option = option ?? throw new ArgumentNullException(nameof(option));
        ParentCommand = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public override void Accept<TContext>(SyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitOption(this, context);
    }
}

internal sealed class OptionArgumentSyntax : Syntax
{
    public Argument Argument { get; }
    public OptionSyntax ParentOption { get; }

    public OptionArgumentSyntax(Argument argument, Token token, OptionSyntax parent)
        : base(token, parent)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        ParentOption = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public override void Accept<TContext>(SyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitOptionArgument(this, context);
    }
}