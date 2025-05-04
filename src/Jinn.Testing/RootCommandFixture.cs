namespace Jinn.Testing;

[PublicAPI]
public sealed class RootCommandFixture
{
    public List<Command> Commands { get; } = [];
    public List<Argument> Arguments { get; } = [];
    public List<Option> Options { get; } = [];
    public Configuration Configuration { get; }

    public RootCommandFixture(params Command[] commands)
    {
        Commands.AddRange(commands);
        Configuration = new Configuration { ExecutableName = "TestRunner", };
    }

    public void AddCommand(Command command)
    {
        Commands.Add(command);
    }

    public void AddArgument(Argument argument)
    {
        Arguments.Add(argument);
    }

    public void AddOption(Option option)
    {
        Options.Add(option);
    }

    public async Task<int> Invoke(string args)
    {
        var root = CreateRootCommand();
        return await root.Invoke(StringSplitter.Split(args));
    }

    public string ParseAndSerialize(
        string args,
        bool excludeExecutable = true,
        ParseResultParts parts = ParseResultParts.Parsed | ParseResultParts.Unmatched)
    {
        var options = new ParseResultSerializerOptions { Parts = parts, ExcludeExecutable = excludeExecutable, };

        return ParseAndSerialize(args, options);
    }

    public string ParseAndSerialize(string args, ParseResultSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(options);

        var result = Parse(args);
        return ParseResultSerializer.Serialize(result, options);
    }

    public ParseResult Parse(string args)
    {
        var root = CreateRootCommand();
        return root.Parse(StringSplitter.Split(args));
    }

    public string ParseAndSerializeDiagnostics(string args)
    {
        return Parse(args).ToErrata();
    }

    public IReadOnlyList<Token> ParseAndReturnTokens(string args)
    {
        return Parse(args).Tokens;
    }

    private RootCommand CreateRootCommand()
    {
        var root = new RootCommand(Configuration, Commands.ToArray());

        foreach (var argument in Arguments)
        {
            root.AddArgument(argument);
        }

        foreach (var option in Options)
        {
            root.AddOption(option);
        }

        return root;
    }
}