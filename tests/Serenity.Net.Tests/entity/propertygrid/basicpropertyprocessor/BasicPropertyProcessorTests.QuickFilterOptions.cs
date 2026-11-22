namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class QuickFilterOptionsColumns
    {
        [QuickFilter]
        public string NotSet { get; set; }

        [QuickFilter(Multiple = true, CheckList = true)]
        public string Set { get; set; }

        [QuickFilter(Multiple = false, CheckList = false), LookupEditor("Test", Multiple = true, CheckList = true)]
        public string TurnedOff { get; set; }

        [QuickFilter(Multiple = true), QuickFilterOption("multiple", false)]
        public string Overridden { get; set; }
    }

    private static PropertyItem ProcessQuickFilterOptions(string propertyName)
    {
        var item = new PropertyItem();
        new BasicPropertyProcessor().Process(new PropertyInfoSource(
            typeof(QuickFilterOptionsColumns).GetProperty(propertyName), null), item);
        return item;
    }

    [Fact]
    public void QuickFilter_Multiple_And_CheckList_Are_Not_Passed_When_Not_Set_So_The_Editor_Decides()
    {
        var item = ProcessQuickFilterOptions(nameof(QuickFilterOptionsColumns.NotSet));
        Assert.False(item.QuickFilterParams.ContainsKey("multiple"));
        Assert.False(item.QuickFilterParams.ContainsKey("checkList"));
    }

    [Fact]
    public void QuickFilter_Multiple_And_CheckList_Are_Passed_When_Set()
    {
        var item = ProcessQuickFilterOptions(nameof(QuickFilterOptionsColumns.Set));
        Assert.Equal(true, item.QuickFilterParams["multiple"]);
        Assert.Equal(true, item.QuickFilterParams["checkList"]);
    }

    [Fact]
    public void QuickFilter_Multiple_And_CheckList_Can_Turn_Off_The_Options_Of_The_Editor()
    {
        var item = ProcessQuickFilterOptions(nameof(QuickFilterOptionsColumns.TurnedOff));
        Assert.Equal(false, item.QuickFilterParams["multiple"]);
        Assert.Equal(false, item.QuickFilterParams["checkList"]);
        // the editor keeps its own, which the quick filter params override on the client
        Assert.Equal(true, item.EditorParams["multiple"]);
    }

    [Fact]
    public void QuickFilterOption_Overrides_The_Properties_Of_QuickFilter()
    {
        var item = ProcessQuickFilterOptions(nameof(QuickFilterOptionsColumns.Overridden));
        Assert.Equal(false, item.QuickFilterParams["multiple"]);
    }

    [Fact]
    public void QuickFilter_GetMultiple_And_GetCheckList_Tell_Whether_They_Are_Set()
    {
        var attr = new QuickFilterAttribute();
        Assert.Null(attr.GetMultiple());
        Assert.Null(attr.GetCheckList());
        Assert.False(attr.Multiple);

        attr.Multiple = false;
        attr.CheckList = true;
        Assert.Equal(false, attr.GetMultiple());
        Assert.Equal(true, attr.GetCheckList());
    }
}
