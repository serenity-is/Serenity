namespace Serenity.ComponentModel;

public class EnumEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Enum()
    {
        var attribute = new EnumEditorAttribute();
        Assert.Equal("Enum", attribute.EditorType);
    }

    [Fact]
    public void AllowClear_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            AllowClear = true
        };
        Assert.True(attribute.AllowClear);
    }

    [Fact]
    public void AllowClear_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            AllowClear = false
        };
        Assert.False(attribute.AllowClear);
    }

    [Fact]
    public void Delimited_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            Delimited = true
        };
        Assert.True(attribute.Delimited);
    }

    [Fact]
    public void Delimited_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            Delimited = false
        };
        Assert.False(attribute.Delimited);
    }

    [Fact]
    public void MinimumResultsForSearch_CanBeSet_ToInt()
    {
        var attribute = new EnumEditorAttribute()
        {
            MinimumResultsForSearch = 1
        };
        Assert.Equal(1, attribute.MinimumResultsForSearch);
    }

    [Fact]
    public void Multiple_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            Multiple = true
        };
        Assert.True(attribute.Multiple);
    }

    [Fact]
    public void Multiple_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            Multiple = false
        };
        Assert.False(attribute.Multiple);
    }

    [Fact]
    public void EnumKey_IsDetermined_FromTheTypeConstructor()
    {
        // for a property not typed with the enum, e.g. a string property holding StringEnum values
        var attribute = new EnumEditorAttribute(typeof(WidgetCategory));

        Assert.Equal("My.WidgetCategory", attribute.EnumKey);
    }

    [Fact]
    public void EnumKey_IsDetermined_FromTheFullName_WhenNoEnumKeyAttribute()
    {
        var attribute = new EnumEditorAttribute(typeof(WidgetCategoryExtended));

        Assert.Equal(typeof(WidgetCategoryExtended).FullName, attribute.EnumKey);
    }

    [Fact]
    public void TypeConstructor_Throws_ForNonEnumType()
    {
        Assert.Throws<ArgumentNullException>(() => new EnumEditorAttribute((Type)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EnumEditorAttribute(typeof(string)));
    }

    [Fact]
    public void StringConstructor_SetsEnumKey()
    {
        var attribute = new EnumEditorAttribute("My.WidgetCategory");

        Assert.Equal("My.WidgetCategory", attribute.EnumKey);
    }

    [Fact]
    public void StringConstructor_Throws_ForNullOrEmptyKey()
    {
        Assert.Throws<ArgumentNullException>(() => new EnumEditorAttribute((string)null!));
        Assert.Throws<ArgumentNullException>(() => new EnumEditorAttribute(""));
    }

    [Fact]
    public void EnumKey_CanBeSet_AsAnOption()
    {
        var attribute = new EnumEditorAttribute { EnumKey = "My.WidgetCategory" };

        Assert.Equal("My.WidgetCategory", attribute.EnumKey);
    }

    [Fact]
    public void SummaryConstants_Are_The_Same_On_All_The_Attributes()
    {
        // an attribute can not pass null, so -1 is the value that names all the selected items, as null does on the client
        Assert.Equal(-1, LookupEditorBaseAttribute.SummaryNamesAll);
        Assert.Equal(0, LookupEditorBaseAttribute.SummaryCountOnly);
        Assert.Equal(LookupEditorBaseAttribute.SummaryNamesAll, ServiceLookupEditorBaseAttribute.SummaryNamesAll);
        Assert.Equal(LookupEditorBaseAttribute.SummaryCountOnly, ServiceLookupEditorBaseAttribute.SummaryCountOnly);

        var attribute = new EnumEditorAttribute { MaxSummaryItems = EnumEditorAttribute.SummaryNamesAll };
        var options = new Dictionary<string, object?>();
        attribute.SetParams(options);
        Assert.Equal(-1, options["maxSummaryItems"]);
    }

    [Fact]
    public void SearchConstants_Are_The_Same_On_All_The_Attributes()
    {
        Assert.Equal(0, LookupEditorBaseAttribute.AlwaysShowSearch);
        Assert.Equal(-1, LookupEditorBaseAttribute.NeverShowSearch);
        Assert.Equal(LookupEditorBaseAttribute.AlwaysShowSearch, ServiceLookupEditorBaseAttribute.AlwaysShowSearch);
        Assert.Equal(LookupEditorBaseAttribute.NeverShowSearch, ServiceLookupEditorBaseAttribute.NeverShowSearch);

        var attribute = new EnumEditorAttribute { MinimumResultsForSearch = EnumEditorAttribute.NeverShowSearch };
        var options = new Dictionary<string, object?>();
        attribute.SetParams(options);
        Assert.Equal(-1, options["minimumResultsForSearch"]);
    }
}
