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
        var root = new RootCommand(Configuration, Commands.ToArray());
        root.Arguments.AddRange(Arguments);
        root.Options.AddRange(Options);
        return root.Parse(StringSplitter.Split(args));
    }

    public IReadOnlyList<Token> ParseAndReturnTokens(string args)
    {
        return Parse(args).Tokens;
    }
}