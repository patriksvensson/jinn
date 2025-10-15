namespace Jinn;

internal sealed class HelpOption : Option
{
    public HelpOption()
        : base("--help", new Argument<bool>("value"))
    {
        Aliases.Add("-h");
        Handler = ctx =>
        {
            ctx.SetProperty(Constants.Invocation.ShowHelp, true);
            return Task.FromResult(false);
        };
    }
}