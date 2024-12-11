namespace Serenity.ComponentModel;

public class LookupFilteringAttributeTests
{

    [Fact]
    public void FilteringType_ShouldBe_Lookup()
    {
        var attribute = new LookupFilteringAttribute("key");
        Assert.Equal("Lookup", attribute.FilteringType);
    }

    [Fact]
    public void LookupFiltering_Constructor_ShouldThrowArgumentNullException_ForNullType()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupFilteringAttribute((Type)null));
    }

    [Fact]
    public void Constructor_ShouldThrow_ArgumentOutOfRangeException_ForInvalidLookupType()
    {

        Type invalidLookupType = typeof(InvalidLookupClass);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new LookupFilteringAttribute(invalidLookupType));
    }

    [Fact]
    public void Constructor_WithValidLookupAttr_ReadsKey_FromAttribute()
    {
        var attr = new LookupFilteringAttribute(typeof(WithLookupAttr));
        Assert.Equal("myKey", attr.LookupKey);
    }

    [Fact]
    public void Constructor_AutoGeneratesLookupKey_IfLookupKeyIsNull()
    {
        var attr = new LookupFilteringAttribute(typeof(WithNullKey));
        Assert.Equal("ComponentModel.WithNullKey", attr.LookupKey);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullString()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LookupFilteringAttribute((string)null));
    }

    [Fact]
    public void IdField_CanBeSet()
    {
        var attribute = new LookupFilteringAttribute("id")
        {
            IdField = "id"
        };
        Assert.Equal("id", attribute.IdField);
    }

    [Fact]
    public void IdField_CanBeSet_ToNull()
    {

        var attribute = new LookupFilteringAttribute("id")
        {
            IdField = null
        };
        Assert.Null(attribute.IdField);
    }

    [LookupScript("myKey")]
    private class WithLookupAttr { }
    private class InvalidLookupClass { }

    [LookupScript]
    private class WithNullKey { }
}


