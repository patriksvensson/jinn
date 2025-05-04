namespace Jinn;

public sealed class Diagnostic
{
    public string? Code { get; init; }
    public string Summary { get; }
    public string Message { get; }
    public Severity Severity { get; }
    public TextSpan? Span { get; init; }

    internal Diagnostic(Severity severity, string summary, string message)
    {
        Severity = severity;
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}

public sealed class Diagnostics : List<Diagnostic>
{
    public bool HasErrors => this.Any(x => x.Severity == Severity.Error);

    public Diagnostics()
    {
    }

    public Diagnostics(IEnumerable<Diagnostic> diagnostics)
        : base(diagnostics)
    {
    }

    public Diagnostic Add(DiagnosticDescriptor descriptor)
    {
        var diagnostic = descriptor.ToDiagnostic(null);
        Add(diagnostic);
        return diagnostic;
    }

    public Diagnostic Add(TextSpan? location, DiagnosticDescriptor descriptor)
    {
        var diagnostic = descriptor.ToDiagnostic(location);
        Add(diagnostic);
        return diagnostic;
    }

    public Diagnostic Add(Token? token, DiagnosticDescriptor descriptor)
    {
        var diagnostic = descriptor.ToDiagnostic(token?.Span);
        Add(diagnostic);
        return diagnostic;
    }

    public Diagnostics Merge(Diagnostics? other)
    {
        return other == null
            ? this
            : new Diagnostics(this.Concat(other));
    }
}

public sealed class DiagnosticDescriptor
{
    public string? Code { get; }
    public string Summary { get; }
    public string Message { get; }
    public Severity Severity { get; }

    public DiagnosticDescriptor(string? code, Severity severity, string summary, string message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Severity = severity;
        Summary = summary;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Diagnostic ToDiagnostic(TextSpan? span)
    {
        return new Diagnostic(Severity, Summary, Message) { Code = Code, Span = span, };
    }
}