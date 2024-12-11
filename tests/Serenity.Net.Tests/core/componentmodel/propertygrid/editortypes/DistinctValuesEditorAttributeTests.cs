namespace Serenity.ComponentModel;

public class DistingValuesEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Lookup_ByDefault()
    {
        var attribute = new DistinctValuesEditorAttribute();
        Assert.Equal("Lookup", attribute.EditorType);
    }

    [Fact]
    public void Constructor_ShouldSet_RowType_And_PropertyName()
    {
        Type rowType = typeof(string);
        var attribute = new DistinctValuesEditorAttribute(rowType, "SomeProperty");
        Assert.Equal(rowType, attribute.RowType);
        Assert.Equal("SomeProperty", attribute.PropertyName);
    }

    [Fact]
    public void PropertyName_ArgumentNullException_ForNull()
    {
        Type rowType = typeof(string);
        string propertyName = null;
        Assert.Throws<ArgumentNullException>(() => new DistinctValuesEditorAttribute(rowType, propertyName));
    }

    [Fact]
    public void RowType_ArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DistinctValuesEditorAttribute(null, "propertyName"));
    }

    [Fact]
    public void Permission_CanBeSet()
    {
        var attribute = new DistinctValuesEditorAttribute()
        {
            Permission = "permission"
        };
        Assert.Equal("permission", attribute.Permission);
    }

    [Fact]
    public void Permission_CanBeSet_ToNull()
    {
        var attribute = new DistinctValuesEditorAttribute()
        {
            Permission = null
        };
        Assert.Null(attribute.Permission);
    }

    [Fact]
    public void Expiration_CanBeSet_ToInt()
    {
        var attribute = new DistinctValuesEditorAttribute()
        {
            Expiration = 2
        };
        Assert.Equal(2, attribute.Expiration);
    }
}
