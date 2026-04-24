namespace Jinn;

internal sealed class InvocationPipeline
{
    private readonly ParseResult _parseResult;

    public InvocationPipeline(ParseResult parseResult)
    {
        _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
    }

    public async Task<int> Invoke(
        Action<InvocationContext>? initialize,
        CancellationToken cancellationToken)
    {
        var context = new InvocationContext(_parseResult);
        if (initialize != null)
        {
            initialize(context);
        }

        var chain = BuildInvocationChain(context);
        await chain.Invoke(context, static (_, _) => Task.CompletedTask, cancellationToken);

        context.InvocationResult?.Invoke(context);
        return context.GetExitCode();
    }

    private InvocationMiddleware BuildInvocationChain(InvocationContext context)
    {
        List<InvocationMiddleware> middlewares =
        [
            ExceptionMiddleware.Invoke,
            HelpMiddleware.Invoke,
            ParseErrorMiddleware.Invoke,
            .. _parseResult.Configuration.Middlewares,
            async (invocationContext, _, ct) =>
            {
                // Invoke all options and arguments
                var current = (CommandResult)invocationContext.ParseResult.Root;
                while (true)
                {
                    var foundCommand = false;
                    foreach (var child in current.Children)
                    {
                        if (child is ArgumentResult { ArgumentSymbol.Handler: not null } argumentResult)
                        {
                            // Invoke
                            if (!await argumentResult.ArgumentSymbol.Handler.Invoke(invocationContext, ct))
                            {
                                return;
                            }
                        }
                        else if (child is OptionResult { OptionSymbol.Argument.Handler: not null } optionResult)
                        {
                            // Invoke
                            if (!await optionResult.OptionSymbol.Argument.Handler.Invoke(invocationContext, ct))
                            {
                                return;
                            }
                        }
                        else if (child is CommandResult commandResult)
                        {
                            // Proceed to the next command
                            current = commandResult;
                            foundCommand = true;
                        }
                    }

                    if (!foundCommand)
                    {
                        break;
                    }
                }

                // Call the command handler as the last step in the invocation chain.
                var handler = invocationContext.ParseResult.ParsedCommand.CommandSymbol.Handler;
                if (handler is not null)
                {
                    await handler(context, ct);
                }
            }
        ];

        return middlewares.Aggregate((first, second) =>
            (ctx, next, ct) =>
                first(ctx, (c, ct2) => second(c, next, ct2), ct));
    }
}