namespace Serenity.ComponentModel;

public class RadioButtonEditorAttributeTests
{
    private enum TestEnum
    {
        First = 1,
        Second = 2
    }

    [EnumKey("CustomEnumKey")]
    private enum KeyedEnum
    {
        First = 1
    }

    [LookupScript("MyLookup")]
    private class LookupType
    {
    }

    private class PlainType
    {
    }

    [Fact]
    public void Ctor_WithEnum_SetsEnumKeyToFullName()
    {
        var attr = new RadioButtonEditorAttribute(typeof(TestEnum));
        Assert.Equal(typeof(TestEnum).FullName, attr.EnumKey);
    }

    [Fact]
    public void Ctor_WithEnumKeyAttribute_UsesKey()
    {
        var attr = new RadioButtonEditorAttribute(typeof(KeyedEnum));
        Assert.Equal("CustomEnumKey", attr.EnumKey);
    }

    [Fact]
    public void Ctor_WithLookupType_SetsLookupKey()
    {
        var attr = new RadioButtonEditorAttribute(typeof(LookupType));
        Assert.Equal("MyLookup", attr.LookupKey);
    }

    [Fact]
    public void Ctor_WithPlainType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RadioButtonEditorAttribute(typeof(PlainType)));
    }

    [Fact]
    public void Ctor_WithNullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RadioButtonEditorAttribute(null));
    }

    [Fact]
    public void Ctor_Default_Works()
    {
        var attr = new RadioButtonEditorAttribute();
        Assert.Null(attr.EnumKey);
        Assert.Null(attr.LookupKey);
    }

    [Fact]
    public void EnumKey_GetSet_Works()
    {
        var attr = new RadioButtonEditorAttribute
        {
            EnumKey = "MyEnum"
        };
        Assert.Equal("MyEnum", attr.EnumKey);
    }

    [Fact]
    public void LookupKey_GetSet_Works()
    {
        var attr = new RadioButtonEditorAttribute
        {
            LookupKey = "MyLookup"
        };
        Assert.Equal("MyLookup", attr.LookupKey);
    }
}