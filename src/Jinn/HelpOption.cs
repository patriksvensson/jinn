namespace Jinn;

internal sealed class HelpOption : Option
{
    public HelpOption()
        : base("--help", new Argument<bool>("value"))
    {
        Aliases.Add("-h");
        Argument.SetHandler(ctx =>
        {
            ctx.ShowHelp = true;
        });
    }
}