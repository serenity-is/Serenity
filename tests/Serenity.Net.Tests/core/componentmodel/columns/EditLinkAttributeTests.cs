namespace Serenity.ComponentModel;

public class EditLinkAttributeTests()
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new EditLinkAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new EditLinkAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void ItemType_CanBeSet()
    {
        var attribute = new EditLinkAttribute()
        {
            ItemType = "itemType"
        };
        Assert.Equal("itemType", attribute.ItemType);
    }

    [Fact]
    public void ItemType_CanBe_SetTo_NullValue()
    {
        var attribute = new EditLinkAttribute()
        {
            ItemType = null
        };
        Assert.Null(attribute.ItemType);
    }

    [Fact]
    public void ItemType_IsNull_ByDefault()
    {
        var attribute = new EditLinkAttribute();
        Assert.Null(attribute.ItemType);
    }

    [Fact]
    public void IdField_CanBeSet()
    {
        var attribute = new EditLinkAttribute()
        {
            IdField = "idField"
        };
        Assert.Equal("idField", attribute.IdField);
    }

    [Fact]
    public void IdField_CanBe_SetTo_NullValue()
    {
        var attribute = new EditLinkAttribute()
        {
            IdField = null
        };
        Assert.Null(attribute.IdField);
    }

    [Fact]
    public void IdField_IsNull_ByDefault()
    {
        var attribute = new EditLinkAttribute();
        Assert.Null(attribute.IdField);
    }

    [Fact]
    public void CssClass_CanBeSet()
    {
        var attribute = new EditLinkAttribute()
        {
            CssClass = "some-class"
        };
        Assert.Equal("some-class", attribute.CssClass);
    }

    [Fact]
    public void CssClass_CanBe_SetTo_NullValue()
    {
        var attribute = new EditLinkAttribute()
        {
            CssClass = null
        };
        Assert.Null(attribute.CssClass);
    }

    [Fact]
    public void CssClass_IsNullByDefault()
    {
        var attribute = new EditLinkAttribute();
        Assert.Null(attribute.CssClass);
    }
}