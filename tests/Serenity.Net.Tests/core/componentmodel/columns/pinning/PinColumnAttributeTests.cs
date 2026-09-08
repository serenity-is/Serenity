namespace Serenity.ComponentModel;

public class PinColumnAttributeTests
{
    [Fact]
    public void Ctor_WithStart_SetsValueToStart()
    {
        var attr = new PinColumnAttribute("start");
        Assert.Equal("start", attr.Value);
    }

    [Fact]
    public void Ctor_WithLeft_SetsValueToStart()
    {
        var attr = new PinColumnAttribute("left");
        Assert.Equal("start", attr.Value);
    }

    [Fact]
    public void Ctor_WithEnd_SetsValueToEnd()
    {
        var attr = new PinColumnAttribute("end");
        Assert.Equal("end", attr.Value);
    }

    [Fact]
    public void Ctor_WithRight_SetsValueToEnd()
    {
        var attr = new PinColumnAttribute("right");
        Assert.Equal("end", attr.Value);
    }

    [Fact]
    public void Ctor_WithInvalidSide_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PinColumnAttribute("invalid"));
    }

    [Fact]
    public void Ctor_WithTrue_SetsValueToStart()
    {
        var attr = new PinColumnAttribute(true);
        Assert.Equal("start", attr.Value);
    }

    [Fact]
    public void Ctor_WithFalse_SetsValueToNull()
    {
        var attr = new PinColumnAttribute(false);
        Assert.Null(attr.Value);
    }

    [Fact]
    public void Ctor_Default_PinsToStart()
    {
        var attr = new PinColumnAttribute();
        Assert.Equal("start", attr.Value);
    }
}

public class PinToStartAttributeTests
{
    [Fact]
    public void Value_IsStart()
    {
        var attr = new PinToStartAttribute();
        Assert.Equal("start", attr.Value);
    }
}

public class PinToEndAttributeTests
{
    [Fact]
    public void Value_IsEnd()
    {
        var attr = new PinToEndAttribute();
        Assert.Equal("end", attr.Value);
    }
}