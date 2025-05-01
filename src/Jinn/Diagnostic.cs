namespace Jinn;

public sealed class Diagnostic
{
    public string? Code { get; init; }
    public string Message { get; }
    public Severity Severity { get; }
    public TextSpan? Span { get; init; }

    internal Diagnostic(Severity severity, string message)
    {
        Severity = severity;
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

    public void Add(DiagnosticDescriptor descriptor)
    {
        Add(descriptor.ToDiagnostic(null));
    }

    public void Add(TextSpan location, DiagnosticDescriptor descriptor)
    {
        var diagnostic = descriptor.ToDiagnostic(location);
        Add(diagnostic);
    }

    public void Add(Token? token, DiagnosticDescriptor descriptor)
    {
        Add(descriptor.ToDiagnostic(token?.Span));
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
    public string Message { get; }
    public Severity Severity { get; }

    public DiagnosticDescriptor(Severity severity, string message)
        : this(null, severity, message)
    {
    }

    public DiagnosticDescriptor(string? code, Severity severity, string message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Severity = severity;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Diagnostic ToDiagnostic(TextSpan? span)
    {
        return new Diagnostic(Severity, Message) { Code = Code, Span = span, };
    }
}