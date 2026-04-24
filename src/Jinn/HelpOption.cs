namespace Jinn;

internal sealed class HelpOption : Option
{
    public HelpOption()
        : base("--help|-h", new Argument<bool>("value"))
    {
        Handler = (ctx, _) =>
        {
            ctx.ShowHelp();
            return Task.FromResult(false);
        };
    }
}