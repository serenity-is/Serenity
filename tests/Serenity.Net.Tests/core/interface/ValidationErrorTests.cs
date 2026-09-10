namespace Serenity.Services;

public class ValidationErrorTests
{
    [Fact]
    public void Ctor_Default_Works()
    {
        var ex = new ValidationError();
        Assert.Null(ex.ErrorCode);
    }

    [Fact]
    public void Ctor_Message_SetsMessage()
    {
        var ex = new ValidationError("message");
        Assert.Equal("message", ex.Message);
    }

    [Fact]
    public void Ctor_LocalizerAndMessage_SetsMessage()
    {
        var ex = new ValidationError(NullTextLocalizer.Instance, new LocalText("key"));
        Assert.Equal("key", ex.Message);
    }

    [Fact]
    public void Ctor_MessageAndInnerException_SetsBoth()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ValidationError("message", inner);
        Assert.Equal("message", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void Ctor_ErrorCodeAndMessage_SetsProperties()
    {
        var ex = new ValidationError("Code", "message");
        Assert.Equal("Code", ex.ErrorCode);
        Assert.Equal("message", ex.Message);
    }

    [Fact]
    public void Ctor_LocalizerErrorCodeAndMessage_SetsProperties()
    {
        var ex = new ValidationError(NullTextLocalizer.Instance, "Code", new LocalText("key"));
        Assert.Equal("Code", ex.ErrorCode);
        Assert.Equal("key", ex.Message);
    }

    [Fact]
    public void Ctor_ErrorCodeArgumentsAndMessage_SetsProperties()
    {
        var ex = new ValidationError("Code", "args", "message");
        Assert.Equal("Code", ex.ErrorCode);
        Assert.Equal("args", ex.Arguments);
        Assert.Equal("message", ex.Message);
    }

    [Fact]
    public void Ctor_LocalizerErrorCodeArgumentsAndMessage_SetsProperties()
    {
        var ex = new ValidationError(NullTextLocalizer.Instance, "Code", "args", new LocalText("key"));
        Assert.Equal("Code", ex.ErrorCode);
        Assert.Equal("args", ex.Arguments);
        Assert.Equal("key", ex.Message);
    }

    [Fact]
    public void Ctor_ErrorCodeArgumentsFormatAndArgs_FormatsMessage()
    {
        var ex = new ValidationError("Code", "args", "Value {0} is invalid", 42);
        Assert.Equal("Code", ex.ErrorCode);
        Assert.Equal("args", ex.Arguments);
        Assert.Equal("Value 42 is invalid", ex.Message);
    }

    [Fact]
    public void IsSensitiveMessage_DefaultsToFalse()
    {
        var ex = new ValidationError("Code", "message");
        Assert.False(ex.IsSensitiveMessage);
    }

    [Fact]
    public void ErrorCode_And_Arguments_AreSettable()
    {
        var ex = new ValidationError
        {
            ErrorCode = "NewCode",
            Arguments = "NewArgs"
        };
        Assert.Equal("NewCode", ex.ErrorCode);
        Assert.Equal("NewArgs", ex.Arguments);
    }
}