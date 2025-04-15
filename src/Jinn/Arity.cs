namespace Jinn;

[PublicAPI]
public readonly struct Arity : IEquatable<Arity>
{
    public int Minimum { get; }
    public int Maximum { get; }

    public static Arity Zero => new(0, 0);
    public static Arity ZeroOrOne => new(0, 1);
    public static Arity ExactlyOne => new(1, 1);
    public static Arity ZeroOrMore => new(0, int.MaxValue);
    public static Arity OneOrMore => new(1, int.MaxValue);

    public Arity(int minimum, int maximum)
    {
        if (minimum < 0)
        {
            throw new ArgumentException("Minimum arity must be greater than zero", nameof(minimum));
        }

        if (maximum < minimum)
        {
            throw new ArgumentException("Maximum arity must be greater than minimum arity", nameof(minimum));
        }

        Minimum = minimum;
        Maximum = maximum;
    }

    public static Arity Exactly(int count)
    {
        return new Arity(count, count);
    }

    public static Arity Resolve(Type type)
    {
        if (type == typeof(bool))
        {
            return ZeroOrOne;
        }

        if (Nullable.GetUnderlyingType(type) != null)
        {
            return ZeroOrOne;
        }

        if (type.IsAssignableTo(typeof(ICollection)))
        {
            return ZeroOrMore;
        }

        return ExactlyOne;
    }

    public bool Equals(Arity other)
    {
        return Minimum == other.Minimum
               && Maximum == other.Maximum;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Minimum, Maximum);
    }
}