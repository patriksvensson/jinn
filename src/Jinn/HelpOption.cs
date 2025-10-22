namespace Jinn;

internal sealed class HelpOption : Option
{
    public HelpOption()
        : base("--help|-h", new Argument<bool>("value"))
    {
        Handler = ctx =>
        {
            ctx.SetProperty(Constants.Invocation.ShowHelp, true);
            return Task.FromResult(false);
        };
    }
}