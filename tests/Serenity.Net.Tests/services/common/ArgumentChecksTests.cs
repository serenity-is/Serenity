namespace Serenity;

public class ArgumentChecksTests
{
    [Fact]
    public void NotNull_Returns_Value_When_Not_Null()
    {
        string value = "test";
        Assert.Same(value, ArgumentChecks.NotNull(value));
    }

    [Fact]
    public void NotNull_Throws_With_Caller_Expression_When_Null()
    {
        string? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => ArgumentChecks.NotNull(value));
        Assert.Equal(nameof(value), ex.ParamName);
    }

    [Fact]
    public void NotNull_Throws_With_Explicit_ParamName_When_Null()
    {
        string? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => ArgumentChecks.NotNull(value, "customName"));
        Assert.Equal("customName", ex.ParamName);
    }

    [Fact]
    public void NotNull_Struct_Returns_Value_When_Not_Null()
    {
        int? value = 42;
        Assert.Equal(42, ArgumentChecks.NotNull(value));
    }

    [Fact]
    public void NotNull_Struct_Throws_With_Caller_Expression_When_Null()
    {
        int? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => ArgumentChecks.NotNull(value));
        Assert.Equal(nameof(value), ex.ParamName);
    }

    [Fact]
    public void NotNull_Struct_Throws_With_Explicit_ParamName_When_Null()
    {
        int? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => ArgumentChecks.NotNull(value, "customName"));
        Assert.Equal("customName", ex.ParamName);
    }
}

public class ArgumentExceptionsTests
{
    [Fact]
    public void OutOfRange_Uses_Explicit_ParamName()
    {
        var ex = ArgumentExceptions.OutOfRange("test", "myParam");
        Assert.Equal("myParam", ex.ParamName);
    }

    [Fact]
    public void OutOfRange_Uses_Caller_Expression_As_ParamName()
    {
        var value = "test";
        var ex = ArgumentExceptions.OutOfRange(value);
        Assert.Equal(nameof(value), ex.ParamName);
    }
}
