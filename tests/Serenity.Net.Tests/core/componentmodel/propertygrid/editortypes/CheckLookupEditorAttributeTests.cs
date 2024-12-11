namespace Serenity.ComponentModel;

public class CheckLookupEditorAttributeTests
{
    [Fact]
    public void LookupKey_CanBe_PassedVia_FirstArgument()
    {
        var attribute = new CheckLookupEditorAttribute("myLookupKey");
        Assert.Equal("myLookupKey", attribute.LookupKey);
    }

    [Fact]
    public void Constructor_WithType_SetsLookupKeyOption_FromLookupScriptAttribute()
    {
        var mockType = typeof(MockLookupType);
        var attribute = new CheckLookupEditorAttribute(mockType);
        Assert.Equal("MockLookupKey", attribute.LookupKey); 
    }

    [Fact]
    public void Constructor_WithType_ThrowsArgumentNullException_WhenTypeIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CheckLookupEditorAttribute((Type)null!));
    }

    [Fact]
    public void Constructor_WithType_ThrowsArgumentException_WhenTypeDoesNotHaveLookupScriptAttribute()
    {
        var mockTypeWithoutLookupScript = typeof(MockTypeWithoutLookupScript);
        Assert.Throws<ArgumentException>(() => new CheckLookupEditorAttribute(mockTypeWithoutLookupScript));
    }

    [Fact]
    public void CascadeFrom_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            CascadeFrom = "SomeCascadeFromValue"
        };
        Assert.Equal("SomeCascadeFromValue", attribute.CascadeFrom);
    }

    [Fact]
    public void CascadeField_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            CascadeField = "SomeCascadeField"
        };
        Assert.Equal("SomeCascadeField", attribute.CascadeField);
    }

    [Fact]
    public void CascadeValue_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            CascadeValue = "SomeCascadeValue"
        };
        Assert.Equal("SomeCascadeValue", attribute.CascadeValue);
    }

    [Fact]
    public void FilterField_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            FilterField = "SomeFilterField"
        };
        Assert.Equal("SomeFilterField", attribute.FilterField);
    }

    [Fact]
    public void FilterValue_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            FilterValue = "SomeFilterValue"
        };
        Assert.Equal("SomeFilterValue", attribute.FilterValue);
    }

    [Fact]
    public void Delimited_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            Delimited = true
        };
        Assert.True(attribute.Delimited);
    }

    [Fact]
    public void CheckedOnTop_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            CheckedOnTop = true
        };
        Assert.True(attribute.CheckedOnTop);
    }

    [Fact]
    public void ShowSelectAll_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            ShowSelectAll = true
        };
        Assert.True(attribute.ShowSelectAll);
    }

    [Fact]
    public void HideSearch_SetAndGet_WorksCorrectly()
    {
        var attribute = new CheckLookupEditorAttribute("lookupKey")
        {
            HideSearch = true
        };
        Assert.True(attribute.HideSearch);
    }
}

[LookupScript("MockLookupKey")]

public class MockLookupType { }

public class MockTypeWithoutLookupScript { }
