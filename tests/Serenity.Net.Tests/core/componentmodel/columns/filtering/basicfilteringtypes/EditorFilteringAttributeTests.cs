namespace Serenity.ComponentModel;

public class EditorFilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Editor()
    {
        var attribute = new EditorFilteringAttribute();

        Assert.Equal("Editor", attribute.FilteringType);
    }

    [Fact]
    public void Constructor_ShouldThrow_InvalidCastException_ForInvalidType()
    {
        var invalidType = typeof(object);
        Assert.Throws<InvalidCastException>(() => new EditorFilteringAttribute(invalidType));
    }

    [Fact]
    public void Constructor_ShouldNotThrowException_ForValidType()
    {
        var validType = typeof(MyEditorAttribute);

        var attr = new EditorFilteringAttribute(validType);
        Assert.Equal("test", attr.EditorType);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullString()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new EditorFilteringAttribute((string)null));

        Assert.Equal("editorType", exception.ParamName);
    }

    [Fact]
    public void EditorType_IsNull_ByDefault()
    {
        var attribute = new EditorFilteringAttribute();
        Assert.Null(attribute.EditorType);
    }

    [Fact]
    public void EditorType_CanBeSet_ToNull()
    {
        var attribute = new EditorFilteringAttribute
        {
            EditorType = null
        };
        Assert.Null(attribute.EditorType);
    }

    [Fact]
    public void EditorType_CanBeSet()
    {
        var attribute = new EditorFilteringAttribute
        {
            EditorType = "test123"
        };
        Assert.Equal("test123", attribute.EditorType);
    }

    [Fact]
    public void UseRelative_IsFalse_ByDefault()
    {
        var attribute = new EditorFilteringAttribute();

        Assert.False(attribute.UseRelative);
    }

    [Fact]
    public void UseRelative_CanBeSet_ToTrue()
    {
        var attribute = new EditorFilteringAttribute
        {
            UseRelative = true
        };
        Assert.True(attribute.UseRelative);
    }

    [Fact]
    public void UseRelative_CanBeSet_ToFalse()
    {
        var attribute = new EditorFilteringAttribute
        {
            UseRelative = false
        };
        Assert.False(attribute.UseRelative);
    }

    [Fact]
    public void UseLike_IsFalse_ByDefault()
    {
        var attribute = new EditorFilteringAttribute();
        Assert.False(attribute.UseLike);
    }

    [Fact]
    public void UseLike_CanBe_SetToTrue()
    {
        var attribute = new EditorFilteringAttribute
        {
            UseLike = true
        };
        Assert.True(attribute.UseLike);
    }

    [Fact]
    public void UseLike_CanBeSet_ToFalse()
    {
        var attribute = new EditorFilteringAttribute
        {
            UseLike = false
        };
        Assert.False(attribute.UseLike);
    }

    class MyEditorAttribute : CustomEditorAttribute
    {
        public MyEditorAttribute()
            : base("test")
        {
        }
    }
}

