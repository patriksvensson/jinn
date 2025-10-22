using Jinn.Exceptions;

namespace Jinn.Tests;

public sealed class OptionTests
{
    [Fact]
    public void Should_Throw_If_Option_Name_Contains_Invalid_Characters()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("foo $"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Unexpected character '$'");
                ex.Template.ShouldBe("foo $");
                ex.Position.ShouldBe(4);
            });
    }

    [Theory]
    [InlineData("--foo|-")]
    [InlineData("--foo|--")]
    public void Should_Throw_If_Option_Has_No_Name(string template)
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>(template));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Options without name are not allowed");
                ex.Template.ShouldBe(template);
                ex.Position.ShouldBe(6);
            });
    }

    [Fact]
    public void Should_Throw_If_Short_Option_Starts_With_Digit()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("--foo|-4"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Option names cannot start with a digit");
                ex.Template.ShouldBe("--foo|-4");
                ex.Position.ShouldBe(7);
            });
    }

    [Fact]
    public void Should_Throw_If_Long_Option_Starts_With_Digit()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("--foo|--42"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Option names cannot start with a digit");
                ex.Template.ShouldBe("--foo|--42");
                ex.Position.ShouldBe(8);
            });
    }

    [Fact]
    public void Should_Throw_If_Long_Option_Name_Contains_Invalid_Character()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("--hello$"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Encountered invalid character '$' in option name");
                ex.Template.ShouldBe("--hello$");
                ex.Position.ShouldBe(7);
            });
    }

    [Fact]
    public void Should_Throw_If_Short_Option_Name_Contains_Invalid_Character()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("-$"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Encountered invalid character '$' in option name");
                ex.Template.ShouldBe("-$");
                ex.Position.ShouldBe(1);
            });
    }

    [Fact]
    public void Should_Throw_If_Long_Option_Contains_Less_Than_Two_Characters()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("--a"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Long option names must consist of more than one character");
                ex.Template.ShouldBe("--a");
                ex.Position.ShouldBe(2);
            });
    }

    [Fact]
    public void Should_Throw_If_Short_Option_Contains_More_Than_One_Characters()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>("-ab"));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("Short option names can not be longer than one character");
                ex.Template.ShouldBe("-ab");
                ex.Position.ShouldBe(1);
            });
    }

    [Fact]
    public void Should_Throw_If_Option_Template_Contains_No_Options()
    {
        // Given, When
        var result = Record.Exception(() => new Option<bool>(""));

        // Then
        result.ShouldBeOfType<JinnTemplateException>()
            .And(ex =>
            {
                ex.Message.ShouldBe("No long or short name for option has been specified");
                ex.Template.ShouldBe("");
                ex.Position.ShouldBeNull();
            });
    }
}