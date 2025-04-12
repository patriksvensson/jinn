using Jinn;

// Specify command
var command = new Command("foo");
command.Options.Add(new Option<int>("-a"));
command.Options.Add(new Option<int>("-b"));
command.Options.Add(new Option<int>("-c"));

// Tokenize
Tokenizer.Tokenize("foo -ac", command);