namespace Jinn.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        var command = new RootCommand();
        command.Arguments.Add(new Argument<string>("VALUE"));
        command.Options.Add(new Option<int>("--foo"));
        command.Options.Add(new Option<string>("--bar"));
    }
}