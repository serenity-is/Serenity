namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private enum ConditionAction
    {
        SetFilter = 1,
        OpenLink = 2,
        OpenDialog = 3
    }

    private class ForLinkAttribute() : VisibleWhenAttribute(nameof(ConditionOptions.Action), ConditionAction.OpenLink)
    {
    }

    private class ConditionOptions
    {
        public ConditionAction? Action { get; set; }
        public bool ShowTrend { get; set; }
        public string DateColumn { get; set; }

        [VisibleWhen(nameof(Action), ConditionAction.OpenLink, ConditionAction.OpenDialog)]
        [RequiredWhen(nameof(Action), ConditionAction.OpenLink)]
        public string Url { get; set; }

        [VisibleWhen(nameof(ShowTrend)), VisibleWhen(nameof(DateColumn), IsNot = true, ClearWhenHidden = true)]
        public string Style { get; set; }

        [ReadOnlyWhen(nameof(ShowTrend), true)]
        public string Locked { get; set; }

        [ForLink]
        public bool NewTab { get; set; }

        public string Plain { get; set; }
    }

    private static PropertyItem ProcessConditions(string propertyName)
    {
        var item = new PropertyItem();
        new BasicPropertyProcessor().Process(new PropertyInfoSource(typeof(ConditionOptions).GetProperty(propertyName), null), item);
        return item;
    }

    [Fact]
    public void Conditions_Have_Enum_Values_As_Their_Numbers()
    {
        var item = ProcessConditions(nameof(ConditionOptions.Url));

        var visible = Assert.Single(item.VisibleWhen);
        Assert.Equal("Action", visible.Field);
        Assert.Equal([2, 3], visible.Values);
        Assert.False(visible.IsNot);

        var required = Assert.Single(item.RequiredWhen);
        Assert.Equal([2], required.Values);
        Assert.Null(item.ReadOnlyWhen);
        Assert.Null(item.ClearWhenHidden);
    }

    [Fact]
    public void Conditions_Are_All_Listed_With_IsNot_And_ClearWhenHidden()
    {
        var item = ProcessConditions(nameof(ConditionOptions.Style));

        Assert.Collection(item.VisibleWhen,
            x => { Assert.Equal("ShowTrend", x.Field); Assert.Null(x.Values); Assert.False(x.IsNot); },
            x => { Assert.Equal("DateColumn", x.Field); Assert.Null(x.Values); Assert.True(x.IsNot); });
        Assert.True(item.ClearWhenHidden);
    }

    [Fact]
    public void Conditions_Include_ReadOnlyWhen_And_Derived_Named_Rules()
    {
        var locked = ProcessConditions(nameof(ConditionOptions.Locked));
        Assert.Equal([true], Assert.Single(locked.ReadOnlyWhen).Values);

        var newTab = ProcessConditions(nameof(ConditionOptions.NewTab));
        var rule = Assert.Single(newTab.VisibleWhen);
        Assert.Equal("Action", rule.Field);
        Assert.Equal([2], rule.Values);
    }

    [Fact]
    public void Conditions_Are_Null_Without_Attributes_And_Left_Out_Of_Json()
    {
        var item = ProcessConditions(nameof(ConditionOptions.Plain));
        Assert.Null(item.VisibleWhen);
        Assert.Null(item.ReadOnlyWhen);
        Assert.Null(item.RequiredWhen);

        var json = System.Text.Json.JsonSerializer.Serialize(ProcessConditions(nameof(ConditionOptions.Style)).VisibleWhen);
        Assert.Equal("""[{"field":"ShowTrend"},{"field":"DateColumn","isNot":true}]""", json);
        Assert.Equal("""[{"field":"Action","values":[2,3]}]""",
            Newtonsoft.Json.JsonConvert.SerializeObject(ProcessConditions(nameof(ConditionOptions.Url)).VisibleWhen));
    }
}
