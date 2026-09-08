namespace Serenity.ComponentModel;

public class ServiceLookupEditorBaseAttributeTests
{
    private class TestServiceLookupEditorAttribute : ServiceLookupEditorBaseAttribute
    {
        public TestServiceLookupEditorAttribute() : base("TestLookup")
        {
        }
    }

    private static TestServiceLookupEditorAttribute NewAttribute()
    {
        return new TestServiceLookupEditorAttribute();
    }

    [Fact]
    public void Service_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.Service = "Northwind/Customer/List";
        Assert.Equal("Northwind/Customer/List", attr.Service);
    }

    [Fact]
    public void IdField_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.IdField = "ID";
        Assert.Equal("ID", attr.IdField);
    }

    [Fact]
    public void TextField_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.TextField = "Name";
        Assert.Equal("Name", attr.TextField);
    }

    [Fact]
    public void PageSize_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.PageSize = 50;
        Assert.Equal(50, attr.PageSize);
    }

    [Fact]
    public void Sort_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.Sort = "Name";
        Assert.Equal(new[] { "Name" }, attr.Sort);
    }

    [Fact]
    public void ColumnSelection_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.ColumnSelection = ColumnSelection.KeyOnly;
        Assert.Equal(ColumnSelection.KeyOnly, attr.ColumnSelection);
    }

    [Fact]
    public void IncludeColumns_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.IncludeColumns = ["A", "B"];
        Assert.Equal(new[] { "A", "B" }, attr.IncludeColumns);
    }

    [Fact]
    public void ExcludeColumns_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.ExcludeColumns = ["A"];
        Assert.Equal(new[] { "A" }, attr.ExcludeColumns);
    }

    [Fact]
    public void IncludeDeleted_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.IncludeDeleted = true;
        Assert.True(attr.IncludeDeleted);
    }

    [Fact]
    public void AutoComplete_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.AutoComplete = true;
        Assert.True(attr.AutoComplete);
    }

    [Fact]
    public void InplaceAdd_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.InplaceAdd = true;
        Assert.True(attr.InplaceAdd);
    }

    [Fact]
    public void InplaceAddPermission_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.InplaceAddPermission = "Permission";
        Assert.Equal("Permission", attr.InplaceAddPermission);
    }

    [Fact]
    public void DialogType_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.DialogType = "MyModule.MyDialog";
        Assert.Equal("MyModule.MyDialog", attr.DialogType);
    }

    [Fact]
    public void CascadeFrom_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.CascadeFrom = "Country";
        Assert.Equal("Country", attr.CascadeFrom);
    }

    [Fact]
    public void CascadeField_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.CascadeField = "CountryID";
        Assert.Equal("CountryID", attr.CascadeField);
    }

    [Fact]
    public void CascadeValue_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.CascadeValue = 5;
        Assert.Equal(5, attr.CascadeValue);
    }

    [Fact]
    public void FilterField_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.FilterField = "GroupID";
        Assert.Equal("GroupID", attr.FilterField);
    }

    [Fact]
    public void FilterValue_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.FilterValue = 7;
        Assert.Equal(7, attr.FilterValue);
    }

    [Fact]
    public void MinimumResultsForSearch_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.MinimumResultsForSearch = 10;
        Assert.Equal(10, attr.MinimumResultsForSearch);
    }

    [Fact]
    public void Multiple_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.Multiple = true;
        Assert.True(attr.Multiple);
    }

    [Fact]
    public void Delimited_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.Delimited = true;
        Assert.True(attr.Delimited);
    }

    [Fact]
    public void OpenDialogAsPanel_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.OpenDialogAsPanel = true;
        Assert.True(attr.OpenDialogAsPanel);
    }

    [Fact]
    public void ItemType_GetSet_Works()
    {
        var attr = NewAttribute();
        attr.ItemType = typeof(string);
        Assert.Equal(typeof(string), attr.ItemType);
    }

    [Fact]
    public void SetParams_SetsOptions()
    {
        var attr = NewAttribute();
        attr.Service = "Northwind/Customer/List";
        attr.PageSize = 50;

        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);

        Assert.Equal("Northwind/Customer/List", dict["service"]);
        Assert.Equal(50, dict["pageSize"]);
    }
}