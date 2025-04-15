using Shouldly;

namespace Jinn.Tests;

public sealed class ArityTests
{
    [Fact]
    public void Should_Resolve_For_Bool()
    {
        // Given, When
        var result = Arity.Resolve(typeof(bool));

        // Then
        result.Minimum.ShouldBe(0);
        result.Maximum.ShouldBe(1);
    }

    [Fact]
    public void Should_Resolve_For_Single_Class()
    {
        // Given, When
        var result = Arity.Resolve(typeof(string));

        // Then
        result.Minimum.ShouldBe(1);
        result.Maximum.ShouldBe(1);
    }

    [Fact]
    public void Should_Resolve_For_Single_Struct()
    {
        // Given, When
        var result = Arity.Resolve(typeof(int));

        // Then
        result.Minimum.ShouldBe(1);
        result.Maximum.ShouldBe(1);
    }

    [Fact]
    public void Should_Resolve_For_Nullable_Single_Struct()
    {
        // Given, When
        var result = Arity.Resolve(typeof(int?));

        // Then
        result.Minimum.ShouldBe(0);
        result.Maximum.ShouldBe(1);
    }

    [Fact]
    public void Should_Resolve_For_Collection_Type()
    {
        // Given, When
        var result = Arity.Resolve(typeof(List<string>));

        // Then
        result.Minimum.ShouldBe(0);
        result.Maximum.ShouldBe(int.MaxValue);
    }
}
