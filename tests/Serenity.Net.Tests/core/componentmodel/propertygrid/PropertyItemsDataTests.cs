namespace Serenity.ComponentModel;

public class PropertyItemsDataTests
{

    [Fact]
    public void Items_ShouldBeInitializedCorrectly()
    {
        var items = new List<PropertyItem> { new(), new() };
        var data = new PropertyItemsData
        {
            Items = items
        };
        Assert.Equal(items, data.Items);
    }

    [Fact]
    public void AdditionalItems_ShouldBeInitializedCorrectly()
    {
        var additionalItems = new List<PropertyItem> { new(), new() };
        var data = new PropertyItemsData
        {
            AdditionalItems = additionalItems
        };
        Assert.Equal(additionalItems, data.AdditionalItems);
    }

    [Fact]
    public void Items_IsNull_ByDefault()
    {
        var data = new PropertyItemsData();
        Assert.Null(data.Items);
    }

    [Fact]
    public void AdditionalItems_IsNull_ByDefault()
    {
        var data = new PropertyItemsData();
        Assert.Null(data.AdditionalItems);
    }
}

