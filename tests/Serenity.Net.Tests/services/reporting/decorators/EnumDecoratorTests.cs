namespace Serenity.Reporting;

public class EnumDecoratorTests
{
    private enum TestEnum
    {
        A = 1,
        B = 2
    }

    [Fact]
    public void Decorate_Formats_Enum_Name()
    {
        var decorator = new EnumDecorator(typeof(TestEnum), NullTextLocalizer.Instance)
        {
            Value = TestEnum.B
        };

        decorator.Decorate();

        Assert.Equal("B", decorator.Value);
    }

    [Fact]
    public void Decorate_Does_Nothing_For_Null_Value()
    {
        var decorator = new EnumDecorator(typeof(TestEnum), NullTextLocalizer.Instance)
        {
            Value = null
        };

        decorator.Decorate();

        Assert.Null(decorator.Value);
    }

    [Fact]
    public void Decorate_Swallows_Format_Errors()
    {
        var decorator = new EnumDecorator(typeof(TestEnum), NullTextLocalizer.Instance)
        {
            Value = "notAnEnumValue"
        };

        decorator.Decorate();

        Assert.Equal("notAnEnumValue", decorator.Value);
    }
}

public class BaseCellDecoratorTests
{
    private class TestDecorator : BaseCellDecorator
    {
        public override void Decorate()
        {
        }
    }

    [Fact]
    public void Properties_Are_Settable()
    {
        var item = new object();
        var decorator = new TestDecorator
        {
            Item = item,
            Name = "Name",
            Value = 42,
            Background = "#fff",
            Foreground = "#000",
            Format = "0.00"
        };

        Assert.Same(item, decorator.Item);
        Assert.Equal("Name", decorator.Name);
        Assert.Equal(42, decorator.Value);
        Assert.Equal("#fff", decorator.Background);
        Assert.Equal("#000", decorator.Foreground);
        Assert.Equal("0.00", decorator.Format);
    }
}
